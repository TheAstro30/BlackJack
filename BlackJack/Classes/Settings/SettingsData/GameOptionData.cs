/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Xml.Serialization;

namespace BlackJack.Classes.Settings.SettingsData
{
    [Serializable]
    public sealed class ConfirmOptionData
    {
        [XmlAttribute("exit")]
        public bool OnExit { get; set; }
    }

    [Serializable]
    public sealed class GameOptionData
    {
        [XmlElement("sound")]
        public SoundData Sound = new SoundData();

        [XmlElement("confirm")]
        public ConfirmOptionData Confirm = new ConfirmOptionData();
    }
}
