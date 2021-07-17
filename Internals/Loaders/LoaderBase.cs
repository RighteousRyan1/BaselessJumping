using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace BaselessJumping.Internals.Loaders
{
	public sealed class LoaderBase
	{
		public static string Root { get; set; }

		public static string DirectoryRoot { get; }

		static LoaderBase()
        {
			DirectoryRoot = Assembly.GetExecutingAssembly().Location;
        }

		internal static void UpdateRoot()
        {
			Root = $"{DirectoryRoot}/" + Root + "/";
        }
	}
}
