using RT.CommandLine;

namespace Trophy
{
    [CommandName("load"), DocumentationRhoML("Loads a quiz state from a file.")]
    public sealed class QuizCmdLoad : QuizCmdLine
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename to the quiz to load.")]
        public string QuizFile = null;

        [IsMandatory, IsPositional, DocumentationRhoML("Specifies the path containing the resource files.")]
        public string ResourcePath = null;

        [Option("-l"), DocumentationRhoML("Specifies a path and filename to which to write logging messages. Without this option, no logging is performed.")]
        public string LogFile = null;

        [Option("-p"), DocumentationRhoML("Specifies the port number on which to run the webserver. Default is 24567.")]
        public int Port = 24567;

        public override int Execute()
        {
            return Program.MainQuiz(Port, QuizFile, ResourcePath, LogFile);
        }
    }
}
