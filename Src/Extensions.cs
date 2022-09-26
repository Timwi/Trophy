using System;
using System.Collections.Generic;
using System.Linq;
using RT.Serialization;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    public static class Extensions
    {
        public static T[] RemoveIndex<T>(this T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index >= array.Length)
                throw new ArgumentException("Index out of bounds.", "index");
            var newArray = new T[array.Length - 1];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            if (index < array.Length - 1)
                Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
            return newArray;
        }

        public static T[] RemoveIndexes<T>(this T[] array, int[] indexes)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (indexes.Any(ix => ix < 0 || ix >= array.Length))
                throw new ArgumentOutOfRangeException("indexes", "Index out of bounds.");
            var newArray = new T[array.Length - indexes.Length];
            var curOld = 0;
            var curNew = 0;
            Array.Sort(indexes);
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] > curOld)
                    Array.Copy(array, curOld, newArray, curNew, indexes[i] - curOld);
                curNew += indexes[i] - curOld;
                curOld = indexes[i] + 1;
            }
            if (curOld < array.Length)
                Array.Copy(array, curOld, newArray, curNew, array.Length - curOld);
            return newArray;
        }

        public static T[] ReplaceIndex<T>(this T[] array, int index, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException("index", "Index out of bounds.");
            var newArray = new T[array.Length];
            Array.Copy(array, 0, newArray, 0, array.Length);
            newArray[index] = element;
            return newArray;
        }

        public static T[] ReplaceIndex<T>(this T[] array, int index, Func<T, T> elementSelector)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (elementSelector == null)
                throw new ArgumentNullException("elementSelector");
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException("index", "Index out of bounds.");
            return array.ReplaceIndex(index, elementSelector(array[index]));
        }

        public static T[] InsertAtIndex<T>(this T[] array, int index, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index > array.Length)
                throw new ArgumentException("Index out of bounds.", "index");
            var newArray = new T[array.Length + 1];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = element;
            if (index < array.Length)
                Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }

        public static TransitionResult With(this QuizStateBase state, string jsMethod = null, object jsParams = null, string jsMusic = null, string jsJingle = null)
        {
            return new TransitionResult(state, null, jsMethod ?? state.JsMethod, jsParams ?? state.JsParameters, jsMusic ?? state.JsMusic, jsJingle ?? state.JsJingle);
        }

        public static TransitionResult NoTransition(this QuizStateBase state)
        {
            return new TransitionResult(state);
        }

        public static IEnumerable<T> Repeat<T>(this T obj, int times)
        {
            return Enumerable.Repeat(obj, times);
        }

        public static T ApplyToClone<T>(this T obj, Action<T> action) where T : ICloneable
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (action == null)
                throw new ArgumentNullException("action");
            var newObj = (T) obj.Clone();
            action(newObj);
            return newObj;
        }

        public static ConsoleColoredString ToUsefulString(this object obj)
        {
            if (obj == null)
                return "<null>".Color(ConsoleColor.DarkGray);
            if (obj is IToConsoleColoredString ccs)
                return ccs.ToConsoleColoredString();
            if (obj is string str)
                return str.Color(ConsoleColor.Yellow);
            if (ExactConvert.IsTrueIntegerType(obj.GetType()))
                return obj.ToString().Color(ConsoleColor.Red);
            if (obj is bool)
                return obj.ToString().Color(ConsoleColor.Green);

            var t = obj.GetType();
            if (t.IsArray)
                return "{0/White} × {1/DarkCyan}".Color(null).Fmt(((dynamic) obj).Length, t.GetElementType().Name);

            if (t.TryGetGenericParameters(typeof(IDictionary<,>), out var arguments))
                return "{0/White} × {1/DarkMagenta} → {2/DarkCyan}".Color(null).Fmt(((dynamic) obj).Count, arguments[0].Name, arguments[1].Name);
            if (t.TryGetGenericParameters(typeof(ICollection<>), out arguments))
                return "{0/White} × {1/DarkCyan}".Color(null).Fmt(((dynamic) obj).Count, arguments[0].Name);

            return obj.ToConsoleColoredString(ConsoleColor.Magenta);
        }
    }
}
