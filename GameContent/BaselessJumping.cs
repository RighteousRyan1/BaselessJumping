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
using System.Linq;
using BaselessJumping.Internals.Common.Systems;
using Microsoft.Xna.Framework.Content;

namespace BaselessJumping.GameContent
{
    public class BaselessJumping
    {
        public static bool ClickMiddle;
        public static bool ClickLeft;
        public static bool ClickRight;
        public static ChatText mostRecentText;
        public static GameTime LastCapturedGameTime { get; internal set; }
        public static event Action OnInitializeButtons;
        public static Background ForestBG = new("ForestBG");
        public static Background MountainsBG = new("MountainsBG");
        public static Background JungleBG = new("JungleBG");

        public static Keybind ViewAll = new("View All", Keys.J);
        public static Keybind InputHandle = new("InputHandle", Keys.L);
        public static Keybind ShowFPS = new("View FPS", Keys.F10);

        public static ContentManager Content => BJGame.Instance.Content;

        public static Player PlayerOne { get; private set; }
        private static bool _showFPS;
        internal static void Update()
        {
            Background.UpdateBGs();
            foreach (var player in Player.AllPlayers)
                player.Update();
            if (ShowFPS.JustPressed)
                _showFPS = !_showFPS;
            if (ViewAll.JustPressed)
                ChatText.displayAllChatTexts = !ChatText.displayAllChatTexts;
            if (InputHandle.JustPressed)
            {
                if (!TextInput.trackingInput)
                {
                    ChatText.NewText("Input started...");
                    TextInput.BeginInput();
                }
                else
                {
                    ChatText.NewText("Input ended...");
                    TextInput.EndInput(() =>
                    {
                        ChatText.NewText(TextInput.InputtedText);
                    });
                }
            }
            foreach (var p in Particle.particles)
                p?.Update();
            foreach (var music in Music.AllMusic)
                music?.Update();
            foreach (var cText in ChatText.TotalTexts)
                cText?.Update();

            if (BJGame.Instance.IsActive)
            {
                if (Input.MouseLeft && Utilities.MouseOnScreenProtected)
                {
                    Block.Methods.PlaceBlock(Utilities.MouseX_TBC, Utilities.MouseY_TBC);
                }
                if (Input.MouseRight && Utilities.MouseOnScreenProtected)
                {
                    Block.Methods.BreakBlock(Utilities.MouseX_TBC, Utilities.MouseY_TBC);
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

            TestingStuff_REMOVE_LATER_PLEASE();
        }

        internal static void Draw()
        {
            Background.DrawBGs();
            Utilities.DrawStringAtMouse(Input.DeltaScrollWheel);
            Background.SetBackground(Input.DeltaScrollWheel);
            foreach (var player in Player.AllPlayers)
                player?.Draw();
            if (_showFPS)
            {
                BJGame.spriteBatch.DrawString(BJGame.Fonts.Lato,
                            $"{Math.Round(1 / LastCapturedGameTime.ElapsedGameTime.TotalSeconds)}",
                            new Vector2(0, Utilities.WindowHeight - 16), Color.White, 0f, Vector2.Zero, 0.35f, default, default);
            }
            ChatText.DrawAllButtons();
            foreach (var b in Block.Blocks)
                b?.Draw();

            foreach (var p in Particle.particles)
                p?.Draw();

            var orig = BJGame.Fonts.SilkPixel.MeasureString(ChatText.curTypedText);
            var orig2 = new Vector2(0, orig.Y / 2);
            BJGame.spriteBatch.DrawString(BJGame.Fonts.SilkPixel, ChatText.curTypedText, new Vector2(20, Utilities.WindowHeight - 20), Color.White, 0f, orig2, 0.5f, default, 0f);
        }

        internal static void Init()
        {
            Background.SetBackground(0);

            Particle.defTexture = Content.Load<Texture2D>("Particle");
            foreach (var player in Player.AllPlayers)
                player.Initialize();

            Internal_InitButtons();

            for (int i = 0; i < Utilities.WindowWidth / 16; i++)
            {
                for (int j = 0; j < Utilities.WindowHeight / 16; j++)
                {
                    new Block(i, j, false, Color.White, true);
                }
            }
            // Init_Players();
        }

        private static void Init_Players()
        {
            PlayerOne = new();
            PlayerOne.width = 25;
            PlayerOne.height = 25;
            PlayerOne.position = new Vector2(500, 500);
        }

        internal static void Internal_InitButtons()
        {
            OnInitializeButtons?.Invoke();
        }

        public static void TestingStuff_REMOVE_LATER_PLEASE()
        {
            if (Input.KeyJustPressed(Keys.G))
            {
                for (int i = 0; i < 360; i++)
                {
                    Particle.SpawnParticle(Utilities.MousePosition, new Vector2(0, 10).RotatedByRadians(i), Color.Red, 0.5f, 0f);
                }
            }
        }
    }
}