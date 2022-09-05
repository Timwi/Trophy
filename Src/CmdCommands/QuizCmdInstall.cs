using System;
using System.IO;
using System.Linq;
using System.Reflection;
using RT.CommandLine;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy
{
    [CommandName("install"), DocumentationRhoML("Installs a new Trophy plugin.")]
    public sealed class QuizCmdInstall : QuizCmdLine, ICommandLineValidatable
    {
        [IsPositional, IsMandatory, DocumentationRhoML("Specifies the path and filename of the plugin to install.")]
        public string PluginPath = null;

        public ConsoleColoredString Validate()
        {
            if (Program.Settings.InstalledPlugins.Any(p => p.EqualsIgnoreCase(PluginPath)))
                return "The plugin, {0/Cyan}, is already installed.".Color(null).Fmt(PluginPath);

            if (!File.Exists(PluginPath))
                return "The file, {0/Cyan}, does not exist.".Color(null).Fmt(PluginPath);

            try
            {
                Assembly.LoadFile(PluginPath);
            }
            catch (Exception e)
            {
                return "Error loading plugin {0/Cyan}: {1/Magenta} ({2})".Color(null).Fmt(PluginPath, e.Message, e.GetType().FullName);
            }

            return null;
        }

        public override int Execute()
        {
            Program.Settings.InstalledPlugins = Program.Settings.InstalledPlugins.Concat(PluginPath).ToArray();
            Program.Settings.SaveLoud();
            return 1;
        }
    }
}