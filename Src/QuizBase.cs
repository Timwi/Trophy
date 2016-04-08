using System;
using System.Collections.Generic;
using RT.Util.Serialization;

namespace Trophy
{
    public abstract class QuizBase
    {
        protected QuizBase() { }    // for Classify

        [ClassifyNotNull]
        public QuizStateBase CurrentState { get; protected set; }

        [ClassifyIgnoreIfDefault]
        public string UndoLine { get; private set; }

        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<Tuple<QuizStateBase, string>>))]
        protected Stack<Tuple<QuizStateBase, string>> _undo = new Stack<Tuple<QuizStateBase, string>>();
        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<Tuple<QuizStateBase, string>>))]
        protected Stack<Tuple<QuizStateBase, string>> _redo = new Stack<Tuple<QuizStateBase, string>>();

        public void Transition(QuizStateBase newState, string undoLine)
        {
            _redo.Clear();
            _undo.Push(Tuple.Create(CurrentState, UndoLine));
            CurrentState = newState;
            UndoLine = undoLine;
        }

        public bool Undo()
        {
            if (_undo.Count == 0)
                return false;
            _redo.Push(Tuple.Create(CurrentState, UndoLine));
            var tup = _undo.Pop();
            CurrentState = tup.Item1;
            UndoLine = tup.Item2;
            return true;
        }

        public bool Redo()
        {
            if (_redo.Count == 0)
                return false;
            _undo.Push(Tuple.Create(CurrentState, UndoLine));
            var tup = _redo.Pop();
            CurrentState = tup.Item1;
            UndoLine = tup.Item2;
            return true;
        }

        public string RedoLine { get { return _redo.Count == 0 ? null : _redo.Peek().Item2; } }

        public abstract string CssJsFilename { get; }
    }
}
