namespace Trophy
{
    public sealed class TransitionResult
    {
        public QuizStateBase State { get; private set; }
        public string UndoLine { get; private set; }
        public string JsMethod { get; private set; }
        public object JsParameters { get; private set; }
        public string JsMusic { get; private set; }
        public string JsJingle { get; private set; }

        public TransitionResult(QuizStateBase state, string undoLine = null, string jsMethod = null, object jsParams = null, string jsMusic = null, string jsJingle = null)
        {
            State = state;
            UndoLine = undoLine;
            JsMethod = jsMethod;
            JsParameters = jsParams;
            JsMusic = jsMusic;
            JsJingle = jsJingle;
        }
    }
}
