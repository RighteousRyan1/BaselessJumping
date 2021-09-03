using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Systems;
using BaselessJumping.Internals;
using BaselessJumping.GameContent.Props;
using BaselessJumping.Internals.Audio;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.UI;
using BaselessJumping.Internals.Common.GameUI;
using BaselessJumping.MapGeneration;

namespace BaselessJumping.GameContent
{
    public class BaselessJumping
    {
        // TODO: fix music volume not changing
        // TODO: fix music audio not looping
        // TODO: Make music audio do stuff like it's supposed to
        public static GameTime LastCapturedGameTime { get; internal set; }
        public static Background ForestBG = new("ForestBG");
        public static Background MountainsBG = new("MountainsBG");
        public static Background JungleBG = new("JungleBG");

        public static Keybind ViewAll = new("View All", Keys.J);
        public static Keybind InputHandle = new("InputHandle", Keys.L);
        public static Keybind ShowFPS = new("View FPS", Keys.F10);

        public static Logger BaseLogger { get; } = new($"{BJGame.ExePath}", "client_logger");

        public static ContentManager Content => BJGame.Instance.Content;

        public static Player PlayerOne { get; private set; }
        private static bool _showFPS;

        public static GenPattern testPattern = new(100, 400, 5, 5, 5, 8, 8);
        internal static void Update()
        {
            if (Input.KeyJustPressed(Keys.G))
                testPattern.Generate();

            foreach (var bind in Keybind.AllKeybinds)
                bind?.Update();
            foreach (var parent in UIParent.TotalParents)
                parent?.UpdateElements();
            foreach (var music in Music.AllMusic)
                music?.Update();
            Background.UpdateBGs();
            foreach (var player in Player.AllPlayers)
                player?.Update();
            foreach (var booster in BoosterPad.BoosterPads)
                booster?.Update();
            if (ShowFPS.JustPressed)
                _showFPS = !_showFPS;
            if (ViewAll.JustPressed)
                ChatText.displayAllChatTexts = !ChatText.displayAllChatTexts;
            foreach (var b in Block.Blocks)
                b?.Update();
            foreach (var p in Particle.particles)
                p?.Update();
            foreach (var cText in ChatText.TotalTexts)
                cText?.Update();

            if (BJGame.Instance.IsActive)
            {
                int type = Input.DeltaScrollWheel + 1;
                if (Input.MouseLeft && GameUtils.MouseOnScreenProtected)
                {
                    Block.Methods.PlaceBlock(GameUtils.MouseX_TBC, GameUtils.MouseY_TBC, type);
                }
                if (Input.MouseRight && GameUtils.MouseOnScreenProtected)
                {
                    Block.Methods.BreakBlock(GameUtils.MouseX_TBC, GameUtils.MouseY_TBC);
                }
                if (Input.KeyJustPressed(Keys.C))
                {
                    foreach (var block in Block.Blocks)
                    {
                        if (block != null)
                        {
                            if (block.Active)
                            {
                                block.Active = false;
                            }
                        }
                    }
                }
            }

            Update_TestingStuff_REMOVE_LATER_PLEASE();
        }

        internal static void Draw()
        {
            Background.DrawBGs();
            foreach (var player in Player.AllPlayers)
                player?.Draw();
            foreach (var booster in BoosterPad.BoosterPads)
                booster?.Draw();
            if (_showFPS)
            {
                BJGame.spriteBatch.DrawString(BJGame.Fonts.Lato,
                            $"{Math.Round(1 / LastCapturedGameTime.ElapsedGameTime.TotalSeconds)}",
                            new(0, GameUtils.WindowHeight - 16), Color.White, 0f, Vector2.Zero, 0.35f, default, default);
            }
            foreach (var b in Block.Blocks)
                b?.Draw();

            foreach (var p in Particle.particles)
                p?.Draw();
            foreach (var parent in UIParent.TotalParents)
                parent?.DrawElements();
            ChatText.DrawAllButtons();
            var orig = BJGame.Fonts.SilkPixel.MeasureString(ChatText.curTypedText);
            var orig2 = new Vector2(0, orig.Y / 2);
            GameUtils.DrawStringAtMouse(IngameConsole.curWritten + $"\n\n{Input.DeltaScrollWheel + 1}");
            BJGame.spriteBatch.DrawString(BJGame.Fonts.SilkPixel, ChatText.curTypedText, new(20, GameUtils.WindowHeight - 20), Color.White, 0f, orig2, 0.5f, default, 0f);
        }
        internal static void Init()
        {
            #region GameContent Init
            LoadableSystem.Load();
            Background.SetBackground(1);

            for (int i = 0; i < 8; i++)
            {
                BoosterPad.Create((BoosterPad.BoosterPadDirection)i, new((i * 100) + 500, 500), 0.25f, Color.White, Color.White);
            }
            Init_Players();
            IngameConsole.Init();
            foreach (var player in Player.AllPlayers)
                player?.Initialize();
            for (int i = 0; i < GameUtils.WindowWidth / 16; i++)
            {
                for (int j = 0; j < GameUtils.WindowHeight / 16; j++)
                {
                    new Block(i, j, false, Color.White, true);
                }
            }
            #endregion
        }
        internal static void Exit()
        {
            LoadableSystem.Unload();
        }
        private static void Init_Players()
        {
            PlayerOne = new(TextureLoader.GetTexture("Particle"));
            PlayerOne.position = new Vector2(500, 500);
        }
        public static void Update_TestingStuff_REMOVE_LATER_PLEASE()
        {
            //new UIParent().AppendElement(new TextButton("", BJGame.Fonts.Amatic, new(), new(), null));

            Stage stage = new("CustomStage");
            if (Input.KeyJustPressed(Keys.Z))
            {
                BaseLogger.Write("Saving stage with name '" + stage.Name + "'...", Logger.LogType.Info);
                Stage.SaveStage(stage);
            }
            if (Input.KeyJustPressed(Keys.X))
            {
                BaseLogger.Write("Attempting to load stage '" + stage.Name + "'...", Logger.LogType.Info);
                Stage.LoadStage(stage);
            }

            if (Input.KeyJustPressed(Keys.OemTilde))
            {
                IngameConsole.Enabled = !IngameConsole.Enabled;
            }
            if (Input.FirstPressedKey.IsNum(out int num))
            {
                Background.SetBackground(num);
            }
        }
    }
}