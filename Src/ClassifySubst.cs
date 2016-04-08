using System.Collections.Generic;
using System.Linq;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace Trophy
{
    public sealed class StackToListSubstitution<T> : IClassifySubstitute<Stack<T>, List<T>>
    {
        List<T> IClassifySubstitute<Stack<T>, List<T>>.ToSubstitute(Stack<T> instance)
        {
            if (instance == null)
                return null;
            var list = instance.ToList();
            list.Reverse();
            return list;
        }

        Stack<T> IClassifySubstitute<Stack<T>, List<T>>.FromSubstitute(List<T> instance)
        {
            return instance == null ? null : instance.ToStack();
        }
    }
}
