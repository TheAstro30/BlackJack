/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Xml.Serialization;

namespace BlackJack.Classes.Settings.SettingsData
{
    [Serializable]
    public sealed class SoundData
    {
        [XmlAttribute("voice")]
        public bool EnableVoice { get; set; }

        [XmlAttribute("voiceVolume")]
        public int VoiceVolume { get; set; }

        [XmlAttribute("effects")]
        public bool EnableEffects { get; set; }

        [XmlAttribute("effectsVolume")]
        public int EffectsVolume { get; set; }

        [XmlAttribute("music")]
        public bool EnableMusic { get; set; }

        [XmlAttribute("musicVolume")]
        public int MusicVolume { get; set; }
    }
}
