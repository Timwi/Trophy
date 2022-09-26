using System;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    public abstract class QuizStateBase : ICloneable
    {
        public abstract IEnumerable<Transition> Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }
        public abstract string JsMethod { get; }
        public abstract object JsParameters { get; }
        public virtual string JsMusic => null;
        public virtual string JsJingle => null;

        public QuizStateBase() { }

        public object Clone() => MemberwiseClone();

        public static ConsoleKeyInfo ReadKey()
        {
            ConsoleUtil.Write("<press a key>".Color(ConsoleColor.DarkYellow));
            var ret = Console.ReadKey(true);
            Console.WriteLine();
            return ret;
        }
    }
}
