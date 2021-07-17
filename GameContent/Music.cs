using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace BaselessJumping.GameContent
{
    public enum MusicState
    {
        Playing,
        Paused,
        Stopped
    }
    /// <summary>
    /// Be sure to use the 
    /// </summary>
    public class Music
    {
        public static List<Music> AllMusic { get; internal set; } = new();
        private bool _justStopped;
        public bool _justBegan;
        public static int CurrentMusicTrack { get; private set; }

        public int whoAmI;
        public readonly string musicPath;
        public float volume;
        public float pan;
        public float pitch;
        public bool roughTransition;
        public MusicState State { get; private set; }

        private SoundEffectInstance _instance;
        private Music(string musicPath, bool roughTransition)
        {
            this.musicPath = musicPath;
            this.roughTransition = roughTransition;
            _instance = BJGame.Instance.Content.Load<SoundEffect>(musicPath).CreateInstance();
        }
        public static Music CreateMusicTrack(string musicPath, bool roughTransition)
        {
            return new Music(musicPath, roughTransition);
        }
        public void Play()
        {
            CurrentMusicTrack = whoAmI;
            _justBegan = true;
            if (roughTransition)
                _instance?.Play();
            else
                volume = 0f;

        }
        public void Pause()
        {
            State = MusicState.Paused;
            _instance?.Pause();
        }
        public void Stop()
        {
            _justStopped = true;
            if (!roughTransition)
                _instance?.Stop();
        }
        internal void Update()
        {
            _instance.Volume = volume;
            _instance.Pan = pan;
            _instance.Pitch = pitch;
            if (CurrentMusicTrack != whoAmI)
            {
                State = MusicState.Stopped;
                Stop();

                if (!roughTransition)
                {
                    if (volume > 0f)
                        volume -= 0.075f;
                    else
                        _instance?.Stop();
                }
            }
            if (CurrentMusicTrack == whoAmI)
            {
                State = MusicState.Playing;
                _instance?.Play();
                if (!roughTransition)
                {
                    if (volume < 1f)
                        volume += 0.075f;
                    if (volume >= 1f)
                        volume = 1f;
                }
            }

            _justStopped = false;
            _justBegan = false;
        }
    }
}