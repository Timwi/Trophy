﻿using System.IO;
using System.Windows.Forms;
using RT.CommandLine;
using RT.Serialization;
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
            PluginIndex = Program.Settings.InstalledPlugins.IndexOf(p => p.EqualsIgnoreCase(PluginPath));
            if (PluginIndex == -1)
                return "The plugin, {0/Cyan}, is not installed.".Color(null).Fmt(PluginPath);
            return null;
        }

        public override int Execute()
        {
            Program.Settings.InstalledPlugins = Program.Settings.InstalledPlugins.RemoveIndex(PluginIndex);
            ClassifyJson.SerializeToFile(Program.Settings, Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TrophySettings.json"));
            return 1;
        }
    }
}