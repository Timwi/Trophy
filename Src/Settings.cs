using RT.Util;
using RT.Util.Serialization;

namespace Trophy
{
    [Settings("Trophy", SettingsKind.MachineSpecific, SettingsSerializer.ClassifyJson)]
    sealed class Settings : SettingsBase
    {
        [ClassifyNotNull]
        public string[] InstalledPlugins = new string[0];
    }
}
