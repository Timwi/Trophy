using System;
using System.Linq;
using RT.Util.CommandLine;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    [CommandName("uninstall"), DocumentationRhoML("Uninstalls an existing Trophy plugin.")]
    public sealed class QuizCmdUninstall : QuizCmdLine, ICommandLineValidatable
    {
        [IsPositional, IsMandatory, DocumentationRhoML("Specifies the path and filename of the plugin to uninstall.")]
        public string PluginPath = null;

        [Ignore]
        private int PluginIndex = 0;

        public ConsoleColoredString Validate()
        {
            PluginIndex = Program.Settings.InstalledPlugins.IndexOf(p => p.EqualsNoCase(PluginPath));
            if (PluginIndex == -1)
                return "The plugin, {0/Cyan}, is not installed.".Color(null).Fmt(PluginPath);
            return null;
        }

        public override int Execute()
        {
            Program.Settings.InstalledPlugins = Program.Settings.InstalledPlugins.RemoveIndex(PluginIndex);
            Program.Settings.SaveLoud();
            return 1;
        }
    }
}