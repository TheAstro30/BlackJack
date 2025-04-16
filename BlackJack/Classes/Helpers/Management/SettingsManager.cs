/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.IO;
using BlackJack.Classes.Serialization;

namespace BlackJack.Classes.Helpers.Management
{
    public static class SettingsManager
    {
        /* It's worse than that, he's DEAD Jim! */
        private const string FilePath = @"\KangaSoft\BlackJack\settings.xml";

        public static Settings.Settings Settings { get; set; }

        static SettingsManager()
        {
            Settings = new Settings.Settings();
        }

        public static void Load()
        {
            var file = Utils.MainDir(FilePath, true);
            if (!File.Exists(file))
            {
                XmlSerialize<Settings.Settings>.Save(file, Settings);
                return;
            }
            var s = new Settings.Settings();
            if (XmlSerialize<Settings.Settings>.Load(file, ref s))
            {
                Settings = s;
            }
        }

        public static void Save()
        {
            XmlSerialize<Settings.Settings>.Save(Utils.MainDir(FilePath, true), Settings);
        }
    }
}
