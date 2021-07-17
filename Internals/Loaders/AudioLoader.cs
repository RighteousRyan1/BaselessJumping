using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BaselessJumping.Internals.Loaders
{
	public sealed class AudioLoader
	{
		public static string[] LoadAllAudioFrom(string path)
        {
			List<string> files = new();
			foreach (string txFile in Directory.GetFiles(path).Where(x => x.EndsWith(".png"))) // NAKA CODE
			{
				var stream = TitleContainer.OpenStream(txFile);
				var tex = SoundEffect.FromStream(stream);

				files.Add(txFile);
				Debug.WriteLine($"Loaded Texture Asset at '{txFile}'");
			}

			return files.ToArray();
		}
	}
}
