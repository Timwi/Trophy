using System;
using RT.CommandLine;
using RT.Serialization;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    [CommandName("start"), DocumentationRhoML("Starts a new quiz and saves the quiz state to a file.")]
    public abstract class QuizCmdStart : QuizCmdLine
    {
        [IsMandatory, IsPositional, DocumentationRhoML("Path and filename where to save the new quiz.")]
        public string OutputFile = null;

        public abstract QuizBase StartState { get; }

        public override int Execute()
        {
            ClassifyJson.SerializeToFile(StartState, OutputFile);
            ConsoleUtil.WriteLine("File saved.".Color(ConsoleColor.Green));
            return 0;
        }
    }
}