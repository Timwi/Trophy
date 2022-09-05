using RT.CommandLine;
using RT.PostBuild;

namespace Trophy
{
    [CommandLine, DocumentationRhoML("Runs the Trophy quiz engine.")]
    public abstract class QuizCmdLine
    {
        private static void PostBuildCheck(IPostBuildReporter rep)
        {
            CommandLineParser.PostBuildStep<QuizCmdLine>(rep, null);
        }

        public abstract int Execute();
    }
}
