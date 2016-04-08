using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Text;

namespace Trophy
{
    public sealed class Transition
    {
        public ConsoleKey Key { get; private set; }
        public string Name { get; private set; }
        private Func<TransitionResult> _executor;

        private Transition(ConsoleKey key, string name, Func<TransitionResult> executor)
        {
            Key = key;
            Name = name;
            _executor = executor;
        }

        private static TransitionResult resultFromState(QuizStateBase state, string name)
        {
            if (state == null)
                return null;
            return new TransitionResult(state, name, state.JsMethod, state.JsParameters, state.JsMusic, state.JsJingle);
        }

        public static Transition Simple(ConsoleKey key, string name, Func<TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                var ret = executor();
                if (ret != null && ret.UndoLine == null)
                    ret = new TransitionResult(ret.State, name, ret.JsMethod, ret.JsParameters, ret.JsMusic, ret.JsJingle);
                return ret;
            });
        }

        public static Transition Simple(ConsoleKey key, string name, Func<QuizStateBase> executor)
        {
            return new Transition(key, name, () => resultFromState(executor(), name));
        }

        public static Transition Simple(ConsoleKey key, string name, Action action)
        {
            return new Transition(key, name, () => { action(); return null; });
        }

        public static Transition Simple(ConsoleKey key, string name, string jsMethod, object jsParameters = null, string jsMusic = null, string jsJingle = null)
        {
            return new Transition(key, name, () => new TransitionResult(null, null, jsMethod, jsParameters, jsMusic, jsJingle));
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Func<string, TransitionResult> executor, bool allowEmpty = false)
        {
            return new Transition(key, name, () =>
            {
                var str = InputBox.GetLine(prompt);
                if (str == null || (!allowEmpty && string.IsNullOrEmpty(str)))
                    return null;
                return executor(str);
            });
        }

        public static Transition String(ConsoleKey key, string name, string prompt, Action<string> executor, bool allowEmpty = false)
        {
            return String(key, name, prompt, s => { executor(s); return null; }, allowEmpty);
        }

        public static Transition String(ConsoleKey key, string name, string prompt1, string prompt2, Func<string, string, TransitionResult> executor, bool allowEmpty = false)
        {
            return new Transition(key, name, () =>
            {
                var str1 = InputBox.GetLine(prompt1);
                if (str1 == null || (!allowEmpty && string.IsNullOrEmpty(str1)))
                    return null;

                var str2 = InputBox.GetLine(prompt2);
                if (str2 == null || (!allowEmpty && string.IsNullOrEmpty(str2)))
                    return null;

                return executor(str1, str2);
            });
        }

        public static Transition String(ConsoleKey key, string name, string prompt1, string prompt2, Action<string, string> executor, bool allowEmpty = false)
        {
            return String(key, name, prompt1, prompt2, (s1, s2) => { executor(s1, s2); return null; }, allowEmpty);
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Func<T, TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                var selected = Program.ConsoleSelect(selection, describe);
                if (selected == null)
                    return null;
                var transition = executor((T) selected);
                if (transition != null && transition.UndoLine == null)
                    transition = new TransitionResult(transition.State, name, transition.JsMethod, transition.JsParameters, transition.JsMusic, transition.JsJingle);
                return transition;
            });
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Func<T, QuizStateBase> executor)
        {
            return Select(key, name, selection, describe, obj => resultFromState(executor(obj), name));
        }

        public static Transition Select<T>(ConsoleKey key, string name, IEnumerable<T> selection, Func<T, ConsoleColoredString> describe, Action<T> executor)
        {
            return Select(key, name, selection, describe, obj => { executor(obj); return (TransitionResult) null; });
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Func<int, TransitionResult> executor) where T : IToConsoleColoredString
        {
            return new Transition(key, name, () =>
            {
                for (int i = 0; i < selection.Length; i++)
                    ConsoleUtil.WriteLine("{0}: {1}".Color(ConsoleColor.White).Fmt(i + 1, selection[i].ToConsoleColoredString()));

                ConsoleUtil.Write("Enter index: ".Color(ConsoleColor.Yellow));
                var str = "";
                while (true)
                {
                    var k = Console.ReadKey(true);
                    if (k.Key >= ConsoleKey.D0 && k.Key <= ConsoleKey.D9)
                    {
                        str += (char) (k.Key - ConsoleKey.D0 + '0');
                        ConsoleUtil.Write(k.KeyChar.ToString());
                    }
                    else if (k.Key == ConsoleKey.Backspace && str.Length > 0)
                    {
                        str = str.Substring(0, str.Length - 1);
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                    }
                    else if (k.Key == ConsoleKey.Escape)
                        return null;
                    else if (k.Key == ConsoleKey.Enter)
                        break;
                }
                int index;
                if (!int.TryParse(str, out index) || index > selection.Length || index == 0)
                    return null;

                var transition = executor(index - 1);
                if (transition != null && transition.UndoLine == null)
                    transition = new TransitionResult(transition.State, name, transition.JsMethod, transition.JsParameters, transition.JsMusic, transition.JsJingle);
                return transition;
            });
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Func<int, QuizStateBase> executor) where T : IToConsoleColoredString
        {
            return SelectIndex(key, name, selection, index => resultFromState(executor(index), name));
        }

        public static Transition SelectIndex<T>(ConsoleKey key, string name, T[] selection, Action<int> executor) where T : IToConsoleColoredString
        {
            return SelectIndex(key, name, selection, index => { executor(index); return (TransitionResult) null; });
        }

        public static Transition Find<T>(ConsoleKey key, string name, IEnumerable<T> options, Func<T, ConsoleColoredString> describe, Func<T, TransitionResult> executor)
        {
            return new Transition(key, name, () =>
            {
                var words = new List<string> { "" };
                var input = options.Select(obj => new { Obj = obj, Describe = describe(obj) }).ToArray();

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine();

                    var dic = new Dictionary<string, T>();
                    if (words.Count > 0 && !string.IsNullOrWhiteSpace(words[0]))
                    {
                        var results = input.Where(inf => words.All(w => inf.Describe.ToString().ToLowerInvariant().Contains(w.ToLowerInvariant())))
                            .OrderByDescending(inf => words.Where(w => !string.IsNullOrWhiteSpace(w)).Sum(w => Regex.Matches(inf.Describe.ToString(), @"\b{0}|{0}\b".Fmt(Regex.Escape(w)), RegexOptions.IgnoreCase).Count))
                            .Select(inf => inf.Obj)
                            .Take(22)
                            .ToArray();
                        var tt = new TextTable { ColumnSpacing = 2 };
                        for (int i = 0; i < results.Length.ClipMax(21); i++)
                        {
                            var keyname = Program.KeyName(
                                i == 0 ? ConsoleKey.Enter : (ConsoleKey) (ConsoleKey.D0 + ((i - 1) % 10)),
                                i < 11 ? 0 : ConsoleModifiers.Shift);
                            dic[keyname.ToString()] = results[i];
                            tt.SetCell(0, i, keyname, alignment: HorizontalTextAlignment.Right);
                            tt.SetCell(1, i, describe(results[i]));
                        }
                        if (results.Length > 21)
                            tt.SetCell(1, 21, "(there’s more)".Color(ConsoleColor.Red));
                        tt.WriteToConsole();
                    }

                    Console.CursorTop = 0;
                    ConsoleUtil.Write("Find words: ".Color(ConsoleColor.Cyan) + words.JoinString(" ").Color(ConsoleColor.Yellow));
                    var keyInfo = Console.ReadKey(true);
                    var keyName = Program.KeyName(keyInfo.Key, keyInfo.Modifiers).ToString();
                    Console.Clear();

                    if (keyName == "Escape")
                        return null;
                    if (dic.ContainsKey(keyName))
                        return executor(dic[keyName]);

                    if (keyInfo.KeyChar == ' ')
                        words.Add("");
                    else if (keyName == "Backspace")
                    {
                        if (!string.IsNullOrWhiteSpace(words[words.Count - 1]))
                            words[words.Count - 1] = words[words.Count - 1].Remove(words[words.Count - 1].Length - 1);
                        else if (words.Count > 1)
                            words.RemoveAt(words.Count - 1);
                    }
                    else if (keyInfo.KeyChar >= 'a' && keyInfo.KeyChar <= 'z')
                        words[words.Count - 1] += keyInfo.KeyChar;
                }
            });
        }

        public static Transition Find<T>(ConsoleKey key, string name, IEnumerable<T> options, Func<T, ConsoleColoredString> describe, Func<T, QuizStateBase> executor)
        {
            return Find(key, name, options, describe, obj => resultFromState(executor(obj), name));
        }

        public static Transition Find<T>(ConsoleKey key, string name, IEnumerable<T> options, Func<T, ConsoleColoredString> describe, Action<T> executor)
        {
            return Find(key, name, options, describe, obj => { executor(obj); return (TransitionResult) null; });
        }

        public TransitionResult Execute()
        {
            return _executor();
        }
    }
}
