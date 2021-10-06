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
    public class GameManager
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

        internal static void Update()
        {
            #region GameManager.Update
            foreach (var st in GameStopwatch.totalTrackable)
                if(st is not null && st.IsRunning)
                    st?.IncreaseTimer();
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
            foreach (var i in Item.items)
                i?.UpdateWorld();

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
                if (Input.KeyJustPressed(Keys.NumPad0))
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
            #endregion

            Update_TestingStuff_REMOVE_LATER_PLEASE();
        }

        internal static void Draw()
        {
            Background.DrawBGs();

            #region GameContent.Draw
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
            foreach (var player in Player.AllPlayers)
                player?.Draw();
            foreach (var i in Item.items)
                i?.Draw();
            foreach (var p in Particle.particles)
                p?.Draw();
            foreach (var parent in UIParent.TotalParents)
                parent?.DrawElements();
            ChatText.DrawAllButtons();
            #endregion
            var orig = BJGame.Fonts.SilkPixel.MeasureString(ChatText.curTypedText);
            var orig2 = new Vector2(0, orig.Y / 2);
            GameUtils.DrawStringAtMouse(IngameConsole.CurrentlyWrittenText + $"\n\n{Input.DeltaScrollWheel + 1}", new(0, -20));

            int offY = 0;
            foreach (var match in IngameConsole.MatchedStrings)
            {
                GameUtils.DrawStringAtMouse($"{match} | similarity: {StringComparator.CompareTo_GetSimilarity(IngameConsole.CurrentlyWrittenText, match)}%", new(20, offY));
                offY += 30;
            }
            BJGame.spriteBatch.DrawString(BJGame.Fonts.SilkPixel, ChatText.curTypedText, new(20, GameUtils.WindowHeight - 20), Color.White, 0f, orig2, 0.5f, default, 0f);
        }
        internal static void Init()
        {
            #region GameContent Init
            LoadableSystem.Load();

            Init_Players();
            IngameConsole.Init();
            foreach (var player in Player.AllPlayers)
                player?.Initialize();

            LightingEngine.InitializeShader();

            LightingEngine.CreateLight(PlayerOne.position, 1f, 2f, Color.White);
            for (int i = 0; i < GameUtils.WindowWidth / 16; i++)
            {
                for (int j = 0; j < GameUtils.WindowHeight / 16; j++)
                {
                    new Block(i, j, false, Color.White, true);
                }
            }
            #endregion
            InitializeTextures();
            Background.SetBackground(1);
        }

        private static void InitializeTextures()
        {
        }
        internal static void Exit()
        {
            LoadableSystem.Unload();
        }
        private static void Init_Players()
        {
            PlayerOne = new(TextureLoader.GetTexture("Particle"));
            PlayerOne.position = new Vector2(200, 200);
            // TODO: When done, remove this
            new GenPattern(150 / 16, 400 / 16, 1, 0, 0, 10, 20).Generate();
        }
        public static void Update_TestingStuff_REMOVE_LATER_PLEASE()
        {
            Stage stage = new("CustomStage");
            if (Input.KeyJustPressed(Keys.Z))
            {
                BaseLogger.Write("Saving stage with name '" + stage.Name + "'...", Logger.LogType.Info);
                Stage.SaveStage(stage);
            }
            if (Input.KeyJustPressed(Keys.X))
            {
                BaseLogger.Write("Attempting to load stage '" + stage.Name + "'...", Logger.LogType.Info);
                Stage.LoadStage(stage.Name);
                Stage.SetStage(stage);
            }

            if (Input.KeyJustPressed(Keys.OemTilde))
                IngameConsole.Enabled = !IngameConsole.Enabled;

            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.I))
            {
                PlayerOne.Damage(1);
                //LightingEngine.CreateLight(GameUtils.MousePosition / GameUtils.WindowHeight, 1f, 50f, Color.White);
            }
            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.P))
            {
                PlayerOne.Heal(1);
            }
            /*if (Input.FirstPressedKey.IsNum(out int num))
            {
                Background.SetBackground(num);
            }*/
        }
    }
}