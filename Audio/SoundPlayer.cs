
using Microsoft.Xna.Framework.Audio;

namespace BaselessJumping.Audio
{
    // TODO: Flesh this out a little more
    public sealed class SoundPlayer
    {
        public static void PlaySoundInstance(SoundEffect fromSound, float volume)
        {
            var sfx = fromSound.CreateInstance();
            sfx.Volume = volume;
            sfx?.Play();
        }
    }
}