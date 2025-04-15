/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BlackJack.Classes.Audio
{
    public enum VoiceType
    {
        //hmm
    }

    [Serializable]
    public class VoiceData
    {
        
    }

    [Serializable]
    public class Voice
    {
        [XmlElement("data")]
        public List<VoiceData> Data = new List<VoiceData>();
    }
}
