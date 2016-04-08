using System;
using RT.Util.CommandLine;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    [CommandName("list"), DocumentationRhoML("Lists all the installed Trophy plugins.")]
    public sealed class QuizCmdList : QuizCmdLine
    {
        public override int Execute()
        {
            if (Program.Settings.InstalledPlugins.Length == 0)
                ConsoleUtil.WriteLine("There are currently no plugins installed.".Color(ConsoleColor.White));
            else
            {
                ConsoleUtil.WriteLine("The following plugins are currently installed:".Color(ConsoleColor.White));
                foreach (var pp in Program.Settings.InstalledPlugins)
                    Console.WriteLine(pp);
            }
            return 0;
        }
    }
}
