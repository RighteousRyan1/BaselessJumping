using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.Internals.Loaders
{
	public sealed class TextureLoader
	{
		public static string[] LoadAllTexturesFrom(string path)
        {
			List<string> files = new();
			foreach (string txFile in Directory.GetFiles(LoaderBase.Root + path).Where(x => x.EndsWith(".png"))) // NAKA CODE
            {
				var stream = TitleContainer.OpenStream(txFile);

				var tex = Texture2D.FromStream(BJGame.Instance.GDManager.GraphicsDevice, stream);

				files.Add(txFile);
				Debug.WriteLine($"Loaded Texture Asset at '{txFile}'");
			}

			return files.ToArray();
        }
		public static Texture2D GetTexture(string path)
        {
			return BJGame.Instance.Content.Load<Texture2D>(path);
        }
	}
}
