/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackJack.Classes.DirectSound;

namespace BlackJack.Classes.Helpers.Management
{
    /* Use the Force... */
    public enum SoundVoiceType
    {
        PlaceBets = 0,
        NoMoreBets = 1,
        PlayerBlackJack = 2,
        DealerBlackJack = 3,
        PlayerBust = 4,
        DealerBust = 5,
        PlayerWins = 6,
        PlayerLoses = 7,
        Push = 8
    }

    public enum SoundEffectType
    {
        Shuffle = 0,
        Deal = 1,
        ChipDrop = 2
    }

    internal class AudioData
    {
        public SoundVoiceType VoiceType { get; set; }

        public SoundEffectType EffectType { get; set; }

        public Sound Player { get; set; }
    }

    public static class AudioManager
    {
        /* Easier way to manage and play external sounds */
        private static readonly Dictionary<int, Sound> VoiceNumeric = new Dictionary<int, Sound>();

        private static readonly List<AudioData> Voice = new List<AudioData>();

        private static readonly List<AudioData> Sounds = new List<AudioData>();
 
        private static readonly List<string> Music = new List<string>();
        private static int _musicIndex ;

        private static Sound _music;

        private static int _voiceVolume;
        private static int _effectsVolume;
        private static int _musicVolume;

        public static void Init()
        {
            var voiceNumericSearch = new FolderSearch();
            voiceNumericSearch.OnFileFound += VoiceNumericSearchFileFound;

            var d = new DirectoryInfo(Utils.MainDir(@"data\voice\numeric", false));
            voiceNumericSearch.BeginSearch(d, "*.wav", "*", false);

            /* Build voice announcment list */
            Voice.AddRange(
                new []
                {
                    LoadVoice(Utils.MainDir(@"\data\voice\bet.wav"), SoundVoiceType.PlaceBets, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\no-more-bets.wav"), SoundVoiceType.NoMoreBets, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\player-blackjack.wav"), SoundVoiceType.PlayerBlackJack, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\dealer-blackjack.wav"), SoundVoiceType.DealerBlackJack, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\player-bust.wav"), SoundVoiceType.PlayerBust, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\dealer-bust.wav"), SoundVoiceType.DealerBust, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\player-wins.wav"), SoundVoiceType.PlayerWins, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\player-loses.wav"), SoundVoiceType.PlayerLoses, _voiceVolume),
                    LoadVoice(Utils.MainDir(@"\data\voice\push.wav"), SoundVoiceType.Push, _voiceVolume)
                });

            /* Build sound effects list */
            Sounds.AddRange(
                new[]
                {
                    LoadEffect(Utils.MainDir(@"\data\fx\card-shuffle.wav"), SoundEffectType.Shuffle, _effectsVolume),
                    LoadEffect(Utils.MainDir(@"\data\fx\card-deal.wav"), SoundEffectType.Deal, _effectsVolume),
                    LoadEffect(Utils.MainDir(@"\data\fx\chip-drop.wav"), SoundEffectType.ChipDrop, _effectsVolume)
                });

            SetVoiceVolume(SettingsManager.Settings.Options.Sound.VoiceVolume);
            SetEffectsVolume(SettingsManager.Settings.Options.Sound.EffectsVolume);
            SetMusicVolume(SettingsManager.Settings.Options.Sound.MusicVolume);

            var musicSearch = new FolderSearch();
            musicSearch.OnFileSearchCompleted += MusicSearchCompleted;
            musicSearch.OnFileFound += MusicSearchFileFound;

            d = new DirectoryInfo(Utils.MainDir(@"data\music\", false));
            musicSearch.BeginSearch(d, "*.mp3", "*", false);
        }

        public static void SetVoiceVolume(int volume)
        {
            _voiceVolume = volume;
            if (VoiceNumeric.Count > 0)
            {
                foreach (var s in VoiceNumeric.Where(s => s.Value != null))
                {
                    s.Value.Volume = volume;
                }
            }
            if (Voice.Count == 0)
            {
                return;
            }
            foreach (var s in Voice.Where(s => s.Player != null))
            {
                s.Player.Volume = volume;
            }
        }

        public static void SetEffectsVolume(int volume)
        {
            _effectsVolume = volume;
            if (Sounds.Count == 0)
            {
                return;
            }
            foreach (var s in Sounds.Where(s => s.Player != null))
            {
                s.Player.Volume = volume;
            }
        }

        public static void SetMusicVolume(int volume)
        {
            _musicVolume = volume;
            if (Sounds.Count == 0 || _music == null)
            {
                return;
            }
            _music.Volume = volume;
        }

        /* Play functions */
        public static void PlayVoice(SoundVoiceType type, bool sync = false)
        {
            if (!SettingsManager.Settings.Options.Sound.EnableEffects)
            {
                return;
            }
            foreach (var s in Voice.Where(s => s.VoiceType == type))
            {
                if (sync)
                {
                    s.Player?.Play(true);
                    return;
                }
                s.Player?.PlayAsync(true);
                return;
            }
        }

        public static void PlayVoiceNumeric(int num)
        {
            if (!SettingsManager.Settings.Options.Sound.EnableVoice || !VoiceNumeric.ContainsKey(num))
            {
                return;
            }
            VoiceNumeric[num].Play(true);
        }

        public static void Play(SoundEffectType type, bool sync = false)
        {
            if (!SettingsManager.Settings.Options.Sound.EnableEffects)
            {
                return;
            }
            foreach (var s in Sounds.Where(s => s.EffectType == type))
            {
                if (sync)
                {
                    s.Player?.Play(true);
                    return;
                }
                s.Player?.PlayAsync(true);
                return;
            }
        }

        public static void PlayMusic(bool next = false)
        {
            if (!SettingsManager.Settings.Options.Sound.EnableMusic || Music.Count == 0)
            {
                return;
            }
            if (next)
            {
                _musicIndex++;
                if (_musicIndex > Music.Count - 1)
                {
                    _musicIndex = 0;
                    Music.Shuffle();
                }
            }
            _music = new Sound(Music[_musicIndex]) { Volume = _musicVolume };
            _music.OnMediaEnded += OnMusicEnd;
            _music.PlayAsync();
        }

        public static void PauseMusic()
        {
            _music?.Pause();            
        }

        public static void ResumeMusic()
        {
            _music?.Resume(); 
        }

        public static void StopMusic()
        {
            _music?.Stop();
        }

        /* FolderSearch callback */
        private static void VoiceNumericSearchFileFound(string file)
        {
            var index = VoiceNumeric.Count + 4;
            var data = new Sound(file);
            VoiceNumeric.Add(index, data);
        }

        private static void MusicSearchFileFound(string file)
        {
            Music.Add(file);
        }

        private static void MusicSearchCompleted(FolderSearch search)
        {
            Music.Shuffle();
            /* Begin music playback */
            PlayMusic();
        }

        /* Music playback callback */
        private static void OnMusicEnd(Sound sound)
        {
            _music.OnMediaEnded -= OnMusicEnd;
            /* Get next track */
            _musicIndex++;
            if (_musicIndex > Music.Count - 1)
            {
                _musicIndex = 0;
                Music.Shuffle();
            }
            _music = new Sound(Music[_musicIndex]) { Volume = _musicVolume };
            _music.OnMediaEnded += OnMusicEnd;
            _music.PlayAsync();
        }

        /* Private load methods */
        private static AudioData LoadVoice(string file, SoundVoiceType type, int volume)
        {
            var data = new AudioData { VoiceType = type };
            LoadSound(data, file, volume);
            return data;
        }
        private static AudioData LoadEffect(string file, SoundEffectType type, int volume)
        {
            var data = new AudioData {EffectType = type};
            LoadSound(data, file, volume);
            return data;
        }

        private static void LoadSound(AudioData data, string file, int volume)
        {
            if (File.Exists(file))
            {
                data.Player = new Sound(file) { Volume = volume };
            }
        }
    }
}
