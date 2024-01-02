using RT.Serialization;

namespace Trophy
{
    sealed class Settings
    {
        [ClassifyNotNull]
        public string[] InstalledPlugins = new string[0];
    }
}
