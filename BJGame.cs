using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.GameContent;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.UI;
using BaselessJumping.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaselessJumping
{
	public class BJGame : Game
	{
		public static BJGame Instance { get; private set; }
		public static SpriteBatch spriteBatch;
		public readonly GraphicsDeviceManager GDManager;
		public static string ProjectPath => Directory.GetCurrentDirectory();
		public static string ExePath => Assembly.GetExecutingAssembly().Location.Replace(@$"\{nameof(GameContent.BaselessJumping)}.dll", string.Empty);
		private static bool _displayMiscInfo;
		private static bool _showBoundKeybinds;
		public struct Fonts
        {
			public static SpriteFont SilkPixel;
			public static SpriteFont Go;
			public static SpriteFont Komika;
			public static SpriteFont Lato;
			public static SpriteFont Amatic;
		}

		/*public struct Sounds
        {
			public static SoundEffect[] Steps = new SoundEffect[4];
			public static SoundEffectInstance[] StepsI = new SoundEffectInstance[4];

			public static SoundEffect PlayerDeath;
			public static SoundEffectInstance PlayerDeathI;

			public static SoundEffect BlockPlace;
			public static SoundEffectInstance BlockPlaceI;

			public static SoundEffect BlockBreak;
			public static SoundEffectInstance BlockBreakI;
		}

		public struct Textures
        {
			public static Texture2D WhitePixel;
			public static Texture2D UIButtonMedium;
			public static Texture2D UIButtonLarge;

			public static Texture2D UIBox;
			public static Texture2D UIBoxChecked;

			public static Texture2D GrassBlock;
		}*/

		public BJGame() : base()
		{
			IsFixedTimeStep = true;
			Instance = this;
			GDManager = new(this);
			Content.RootDirectory = "Assets";
			Console.Title = "DebugConsole";
			Window.Title = $"Baseless Jumping: {Lang.GetRandomGameTitle()}";
		}

        protected override void Initialize()
		{
			GDManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			GDManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 60;
			GDManager.ApplyChanges();
			AssetsAndOtherInit();
			base.Initialize();
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
			GameContent.BaselessJumping.Exit();
        }

        protected override void LoadContent()
		{
			LoadGameContent();

			GameContent.BaselessJumping.Init();
			base.LoadContent();
        }

		protected override void Update(GameTime gameTime)
        {
			Input.HandleInput();
			GameContent.BaselessJumping.LastCapturedGameTime = gameTime;
			if (Input.KeyJustPressed(Keys.Delete))
				Console.Clear();
			if (Input.KeyJustPressed(Keys.Insert))
				_displayMiscInfo = !_displayMiscInfo;
			if (Input.KeyJustPressed(Keys.Home))
				_showBoundKeybinds = !_showBoundKeybinds;

			GameContent.BaselessJumping.Update();

			Input.OldKeySnapshot = Input.CurrentKeySnapshot;
			Input.OldMouseSnapshot = Input.CurrentMouseSnapshot;
		}

		protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			GameContent.BaselessJumping.Draw();

			var fileNames = Directory.GetFiles(ProjectPath + "/Assets");
			float len = 0f;
			if (_displayMiscInfo)
			{
				int i = 0;
				foreach (string fileName in fileNames)
				{
					var font = Fonts.Lato;
					var scale = 0.35f;
					var infos = new FileInfo(fileName);
					spriteBatch.DrawString(font, infos.Name, new Vector2(6, 15 * i), Color.White, 0f, Vector2.Zero, scale, default, default);
					len += font.MeasureString(infos.Name).Y * scale;
					i++;
				}
			}
			if (_showBoundKeybinds)
			{
				int j = 0;
				foreach (var keybind in Keybind.AllKeybinds)
				{
					spriteBatch.DrawString(Fonts.Lato, keybind.ToString(), 
						_displayMiscInfo ? new Vector2(6, len + 15 + (15 * j)) : new Vector2(6, 15 * j), 
						Color.White, 0f, Vector2.Zero, 0.35f, default, default);

					if (keybind.IsReassignPending)
						spriteBatch.DrawString(Fonts.Lato,
							$"Reassigning '{keybind.Name}' (Current Key: {keybind.AssignedKey})", 
							Drawing.ScreenBounds / 2 + new Vector2(6, 15 * j), Color.White, 0f, Vector2.Zero, 0.35f, default, default);

					if (keybind.RecentlyBound)
						spriteBatch.DrawString(Fonts.Lato,
							$"Reassigned '{keybind.Name}' to '{keybind.AssignedKey.ParseKey()}'",
							Drawing.ScreenBounds / 2 + new Vector2(6, 15 * j), Color.White, 0f, Vector2.Zero, 0.35f, default, default);
					j++;
				}
			}
            spriteBatch.End();
        }

		private void AssetsAndOtherInit()
        {
			if (Directory.Exists($"{ExePath}/Assets"))
			{
				foreach (string fileName in Directory.GetFiles($"{ExePath}/Assets"))
				{
					File.Delete(fileName);
				}
			}
			foreach (string fileName in Directory.GetFiles(ProjectPath + "/Assets"))
			{
				var info = new FileInfo(fileName);
				byte[] spriteFontBytes = File.ReadAllBytes(fileName);
				Directory.CreateDirectory($"{ExePath}/Assets");
				File.WriteAllBytes($"{ExePath}/Assets/{info.Name}", spriteFontBytes);
			}
			Console.Clear();

			spriteBatch = new SpriteBatch(GraphicsDevice);
			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}

		private void LoadGameContent()
        {
            #region Content Loading
            Fonts.SilkPixel = Content.Load<SpriteFont>("SilkPixel");
			Fonts.Lato = Content.Load<SpriteFont>("Lato");
			Fonts.Go = Content.Load<SpriteFont>("Go");
			Fonts.Komika = Content.Load<SpriteFont>("Komika");
			Fonts.Amatic = Content.Load<SpriteFont>("Amatic");

			/*for (int i = 0; i < Sounds.StepsI.Length; i++)
			{
				Sounds.Steps[i] = Content.Load<SoundEffect>($"Step{i + 1}");
				Sounds.StepsI[i] = Sounds.Steps[i].CreateInstance();
			}

			Sounds.BlockBreak = Content.Load<SoundEffect>($"BlockBreak");
			Sounds.BlockBreakI = Sounds.BlockBreak.CreateInstance();

			Textures.WhitePixel = Content.GetResource<Texture2D>("WhitePixel"); // Content.Load<Texture2D>("WhitePixel");
			Textures.UIButtonMedium = Content.GetResource<Texture2D>("UIButtonMedium");
			Textures.UIButtonLarge = Content.GetResource<Texture2D>("UIButtonLarge");
			Textures.UIBox = Content.GetResource<Texture2D>("UIBox");
			Textures.UIBoxChecked = Content.GetResource<Texture2D>("UIBoxChecked");
			
			Textures.GrassBlock = Content.Load<Texture2D>("GrassBlock");*/
			#endregion
		}
    }
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			DllManager.AttachResolver(Assembly.GetExecutingAssembly());
			DllManager.AttachResolver(Assembly.GetAssembly(typeof(Game)));

			new BJGame().Run();
		}
	}
}
