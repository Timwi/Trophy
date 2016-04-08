$.fn.addClassDelay = function (cls, delay)
{
    var t = this;
    window.setTimeout(function () { t.addClass(cls); }, delay || 100);
    return t;
};

$(function ()
{
    var socket;
    var currentMusic = null;
    var jingleVolume = 1;
    var jingles = {};

    function newSocket()
    {
        socket = new WebSocket($(document.body).data('socket-url'));
        socket.onopen = function ()
        {
            console.log('socket open');
            socket.send("ping");
        };
        socket.onclose = function ()
        {
            console.log('socket close');
            window.setTimeout(newSocket, 1000);
        };
        socket.onmessage = function (msg)
        {
            console.log('socket message: ' + msg.data);
            var data = JSON.parse(msg.data);
            if ('panic' in data && data.panic)
            {
                $('audio,video').remove();
                for (var key in musics)
                    if (musics[key].audio)
                        musics[key].audio.pause();
                $('#content').empty();
            }
            else
            {
                if ('method' in data)
                {
                    if (data.method in transitions)
                        transitions[data.method](data.params);
                    else
                        console.log('Undefined transition method: ' + data.method);
                }
                if ('jingle' in data && data.jingle)
                {
                    var newJingle = $('<audio src="' + jingleUrl(data.jingle) + '">').appendTo(document.body);
                    newJingle[0].volume = jingleVolume;
                    newJingle[0].play();
                    newJingle[0].onended = function () { newJingle.remove(); };
                }
                if ('music' in data)
                {
                    for (var key in musics)
                    {
                        if (!musics[key].audio)
                        {
                            if (key !== data.music)
                                continue;
                            musics[key].audio = new Audio(musics[key].url);
                        }
                        if (key !== data.music)
                        {
                            // fade out
                            $(musics[key].audio).animate({ volume: 0 }, musics[key].fadeOut * 1000, function (k) { return function () { musics[k].audio.pause(); }; }(key));
                        }
                        else
                        {
                            // fade in
                            if (musics[key].audio.paused)
                            {
                                musics[key].audio.volume = 0;
                                musics[key].audio.currentTime = 0;
                                musics[key].audio.play();
                            }
                            $(musics[key].audio).animate({ volume: musics[key].volume }, musics[key].fadeIn * 1000);
                        }
                    }
                }
            }
        };
    }

    newSocket();

    $(document.body).keypress(function (e)
    {
        if (e.keyCode === 99)   // 'c'
        {
            $(document.body).css('cursor', 'default');
        }
        else if (e.keyCode === 114)   // 'r'
        {
            $(document.body).css('cursor', 'none');
        }
    });
});

function findBestValue(startingValue, evaluator, threshold, preferHigh)
{
    startingValue = +startingValue;
    threshold = threshold || .1;
    preferHigh = !!preferHigh;
    var iter = 0;

    var negative = startingValue < 0;
    var low = negative ? startingValue : 0;
    var high = startingValue == 0 ? 1 : (negative ? 0 : startingValue);
    if (negative)
    {
        while (evaluator(low) > 0)
        {
            iter++;
            if (iter > 1000) { alert('iteration failed'); return; }
            high = low;
            low *= 2;
        }
    }
    else
    {
        while (evaluator(high) < 0)
        {
            iter++;
            if (iter > 1000) { alert('iteration failed'); return; }
            low = high;
            high *= 2;
        }
    }

    while (high - low > threshold)
    {
        iter++;
        if (iter > 1000) { alert('iteration failed'); return; }
        var mid = (high + low) / 2;
        var ev = evaluator(mid);
        if (ev === 0)
            return mid;
        if (ev < 0)
            low = mid;
        else
            high = mid;
    }
    var finalValue = preferHigh ? high : low;
    evaluator(finalValue);
    return finalValue;
}

function startBuffering(music)
{
    if (!musics[music] || musics[music].audio)
        return;
    musics[music].audio = new Audio(musics[music].url);
}
