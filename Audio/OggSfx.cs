using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Audio
{
    public class OggSfx
    {
        private VorbisReader reader;

        private string path;

        public SoundEffect Sound { get; }

        public OggSfx(string soundPath)
        {
            path = soundPath;
            const int bufferSize = 4096;
            float[] buffer = new float[bufferSize];
            reader = new(Path.Combine(Base.Instance.Content.RootDirectory, path + ".ogg"));

            int samples = reader.ReadSamples(buffer, 0, buffer.Length);
            reader.ReadSamples(buffer, samples, buffer.Length - samples);

            List<byte> samps = new();
            int count;
            while ((count = reader.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    int temp = (int)(short.MaxValue * buffer[i]);
                    if (temp > short.MaxValue)
                        temp = short.MaxValue;
                    else if (temp < short.MinValue)
                        temp = short.MinValue;

                    var sampleWrite = BitConverter.GetBytes((short)temp);

                    samps.Add(sampleWrite[0]);
                    samps.Add(sampleWrite[1]);
                }
            }

            Sound = new(samps.ToArray(), reader.SampleRate, (AudioChannels)reader.Channels);
        }

        public void Play(float volume = 1f)
        {
            var sfx = Sound.CreateInstance();
            sfx.Volume = volume;
            sfx?.Play();
        }
    }
}
