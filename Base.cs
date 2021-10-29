using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.GameContent;
using BaselessJumping.GameContent.Shapes;
using BaselessJumping.Internals;
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
using System.Text.Json;

namespace BaselessJumping
{
	public class Base : Game
	{

		public static JsonSerializerOptions JsonOptions_IncludeFields = new()
		{
			IncludeFields = true,
			WriteIndented = true
		};
		public static JsonSerializerOptions DefaultJsonOPtions = new()
		{
			WriteIndented = true
		};
		public static string GameVersion => "v1.0";
		public static string AssemblyVersion => "0.1";

		public static Base Instance { get; private set; }
		public static SpriteBatch spriteBatch;
		public readonly GraphicsDeviceManager GDManager;
		public static string ProjectPath => Directory.GetCurrentDirectory();
		public static string ExePath => Assembly.GetExecutingAssembly().Location.Replace(@$"\BaselessJumping.dll", string.Empty);
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

		private Stopwatch _contentLoadTimer = new();
		private Stopwatch _gameUpdateTime = new();
		private Stopwatch _renderUpdateTime = new();

		public static TimeSpan LogicTime { get; private set; } = new();
		public static TimeSpan RenderTime { get; private set; } = new();

		public static double RenderFPS { get; private set; } = 0;
		public static double LogicFPS { get; private set; } = 0;
		public Base() : base()
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
			GameManager.Exit();
			RPCHandler.Terminate();
		}

		protected override void LoadContent()
		{
			_contentLoadTimer.Start();
			LoadGameContent();
			GameManager.Initialize();
			base.LoadContent();

			_contentLoadTimer.Stop();
			Console.WriteLine($"Load time for game assets: {_contentLoadTimer.Elapsed}");
		}

		protected override void Update(GameTime gameTime)
		{
			_gameUpdateTime.Start();
			Input.HandleInput();
			GameManager.LastCapturedGameTime = gameTime;
			if (Input.KeyJustPressed(Keys.Delete))
				Console.Clear();
			if (Input.KeyJustPressed(Keys.Insert))
				_displayMiscInfo = !_displayMiscInfo;
			if (Input.KeyJustPressed(Keys.Home))
				_showBoundKeybinds = !_showBoundKeybinds;
			GameManager.Update();

			Input.OldKeySnapshot = Input.CurrentKeySnapshot;
			Input.OldMouseSnapshot = Input.CurrentMouseSnapshot;

			LogicTime = _gameUpdateTime.Elapsed;

			LogicFPS = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2);

			_gameUpdateTime.Restart();
		}

		protected override void Draw(GameTime gameTime)
		{
			_renderUpdateTime.Start();

			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			GameManager.Draw();

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
			foreach (var trail in Triangle.triangles)
				trail?.DrawImmediate();

			spriteBatch.Begin();
			Triangle.DrawVertexHierarchies();
			spriteBatch.End();

			RenderTime = _renderUpdateTime.Elapsed;
			RenderFPS = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2);

			_renderUpdateTime.Restart();
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
			// DllManager.AttachResolver(Assembly.LoadFrom(Path.Combine(Base.ProjectPath, "References", "Native", "x64", "SDL2.dll")));

			new Base().Run();
		}
	}
}