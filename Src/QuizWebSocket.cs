using System.Net;
using RT.Json;
using RT.Serialization;
using RT.Servers;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    sealed class QuizWebSocket : WebSocket
    {
        private IPEndPoint _endpoint;

        public QuizWebSocket(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
        }

        protected override void onBeginConnection()
        {
            base.onBeginConnection();
            lock (Program.Sockets)
                Program.Sockets.Add(this);
            Program.LogMessage("WebSocket connection from {0}".Fmt(_endpoint));
        }

        protected override void onEndConnection()
        {
            lock (Program.Sockets)
                Program.Sockets.Remove(this);
            base.onEndConnection();
            Program.LogMessage("CLOSED WebSocket connection from {0}".Fmt(_endpoint));
        }

        protected override void onTextMessageReceived(string msg)
        {
            if (msg == "ping")
            {
                // Because of concurrency, make sure we access Program.Quiz.CurrentState only once.
                var state = Program.Quiz.CurrentState;
                if (state.JsMethod != null)
                {
                    var prms = state.JsParameters.NullOr(p =>
                    {
                        var ret = ClassifyJson.Serialize(state.JsParameters);
                        if (ret.ContainsKey(":fulltype"))
                            ret.Remove(":fulltype");
                        return ret;
                    });
                    SendLoggedMessage(new JsonDict { { "method", state.JsMethod }, { "params", prms }, { "music", state.JsMusic }, { "jingle", state.JsJingle } });
                }
            }
            base.onTextMessageReceived(msg);
        }

        public void SendLoggedMessage(JsonValue json)
        {
            Program.LogMessage("{0}: {1}".Fmt(_endpoint, JsonValue.ToString(json)));
            SendMessage(json);
        }
    }
}
