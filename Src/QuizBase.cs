﻿using System;
using System.Collections.Generic;
using RT.Serialization;

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
        protected Stack<Tuple<QuizStateBase, string>> _undo = new();
        [ClassifyNotNull, ClassifySubstitute(typeof(StackToListSubstitution<Tuple<QuizStateBase, string>>))]
        protected Stack<Tuple<QuizStateBase, string>> _redo = new();

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

        public string RedoLine => _redo.Count == 0 ? null : _redo.Peek().Item2;

        public abstract string CssJsFilename { get; }
        public virtual string MoreCss => null;

        /// <summary>This data is added to a <c>data-data</c> attribute on the <c>body</c> tag in the HTML.</summary>
        public virtual object ExtraData => null;
    }
}
