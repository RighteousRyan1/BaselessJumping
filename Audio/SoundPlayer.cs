
using BaselessJumping.GameContent;
using Microsoft.Xna.Framework.Audio;

namespace BaselessJumping.Audio
{
    // TODO: Flesh this out a little more
    public sealed class SoundPlayer
    {
        public static SoundEffectInstance PlaySoundInstance(SoundEffect fromSound, float volume)
        {
            var sfx = fromSound.CreateInstance();
            sfx.Volume = volume;
            sfx?.Play();

            return sfx;
        }

        public static SoundEffectInstance PlaySoundInstance(SoundEffect fromSound, SoundContext context, float volume = 1f)
        {
            switch (context)
            {
                case SoundContext.Music:
                    volume *= GameManager.MusicVolume * GameManager.MasterVolume;
                    break;
                case SoundContext.Sound:
                    volume *= GameManager.SoundVolume * GameManager.MasterVolume;
                    break;
                case SoundContext.Ambient:
                    volume *= GameManager.AmbientVolume * GameManager.MasterVolume;
                    break;
            }
            var sfx = fromSound.CreateInstance();
            sfx.Volume = volume;
            sfx?.Play();

            return sfx;
        }
    }

    public enum SoundContext : byte
    {
        Music,
        Sound,
        Ambient
    }
}