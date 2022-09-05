using RT.Serialization;
using RT.Util;

namespace Trophy
{
    [Settings("Trophy", SettingsKind.MachineSpecific, SettingsSerializer.ClassifyJson)]
    sealed class Settings : SettingsBase
    {
        [ClassifyNotNull]
        public string[] InstalledPlugins = new string[0];
    }
}
