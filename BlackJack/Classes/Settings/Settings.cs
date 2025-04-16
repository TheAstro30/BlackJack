/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Drawing;
using System.Xml.Serialization;
using BlackJack.Classes.Serialization;
using BlackJack.Classes.Settings.SettingsData;

namespace BlackJack.Classes.Settings
{
    [Serializable, XmlRoot("settings")]
    public sealed class Settings
    {
        [XmlAttribute("location")]
        public string LocationString
        {
            get => XmlFormatting.WritePointFormat(Location);
            set => Location = XmlFormatting.ParsePointFormat(value);
        }

        [XmlAttribute("size")]
        public string SizeString
        {
            get => XmlFormatting.WriteSizeFormat(Size);
            set => Size = XmlFormatting.ParseSizeFormat(value);
        }

        [XmlAttribute("max")] public bool Maximized { get; set; }

        [XmlIgnore] public Point Location { get; set; }

        [XmlIgnore] public Size Size { get; set; }

        /* Game options */
        [XmlElement("options")] public GameOptionData Options = new GameOptionData();

        /* Constructor */
        public Settings()
        {
            /* Set default settings */
            Size = new Size(900, 600);
            Options.Sound.EnableVoice = true;
            Options.Sound.VoiceVolume = 100;
            Options.Sound.EnableEffects = true;
            Options.Sound.EffectsVolume = 100;
            Options.Sound.EnableMusic = true;
            Options.Sound.MusicVolume = 60;
            Options.Confirm.OnExit = true;
        }
    }
}
    
