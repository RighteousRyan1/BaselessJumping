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
using BaselessJumping.MapGeneration;
using BaselessJumping.GameContent.Shapes;
using BaselessJumping.Audio;

namespace BaselessJumping.GameContent
{
    public class GameManager
    {
        public static Quest TestQuest = new(("Joe Mama", false), ("Joe Daddy", true));


        internal static SettingsService SettingsService { get; private set; } = new();
        // TODO: fix music volume not changing
        // TODO: fix music audio not looping
        // TODO: Make music audio do stuff like it's supposed to
        public static GameTime LastCapturedGameTime { get; internal set; }
        public static Background ForestBG = new("ForestBG");
        public static Background MountainsBG = new("MountainsBG");
        public static Background JungleBG = new("JungleBG");

        public static Keybind ViewAll = new("View All", Keys.J);
        public static Keybind InputHandle = new("InputHandle", Keys.L);
        public static Keybind ShowRenderData = new("View FPS", Keys.F10);

        public static Logger BaseLogger { get; } = new($"{Base.ExePath}", "client_logger");

        public static ContentManager Content => Base.Instance.Content;

        private static bool _showRenders;

        public static UIElement lastElementClicked;

        public static float MasterVolume { get; set; } = 1f;
        public static float MusicVolume { get; set; } = 0.5f;
        public static float SoundVolume { get; set; } = 0.5f;
        public static float AmbientVolume { get; set; } = 0.5f;

        public static bool cheats;

        public static JsonHandler SettingsHandler = new(SettingsService, Path.Combine(Base.ExePath, "Config", "settings.json"));

        internal static void Update()
        {
            if (Input.KeyJustPressed(Keys.O))
            {
                foreach (var x in SettingsService.GetType().GetFields())
                {
                    Console.WriteLine($"{x.Name} : {x.GetValue(SettingsService)}");
                }
            }
            if (Input.KeyJustPressed(Keys.P))
                TestQuest.Objectives[0].Complete();
            RPCHandler.Update();
            if (Base.Instance.IsActive)
            {
                if (Input.KeyJustPressed(Keys.Escape))
                    IngameUI.parent_PauseMenu.Visible = !IngameUI.parent_PauseMenu.Visible;
                #region GameManager.Update
                foreach (var st in GameStopwatch.totalTrackable)
                    if (st is not null && st.IsRunning)
                        st?.IncreaseTimer();
                foreach (var bind in Keybind.AllKeybinds)
                    bind?.Update();
                foreach (var music in Music.AllMusic)
                    music?.Update();
                Background.UpdateBGs();
                foreach (var player in Player.AllPlayers)
                    player?.Update();
                foreach (var booster in BoosterPad.BoosterPads)
                    booster?.Update();
                if (ShowRenderData.JustPressed)
                    _showRenders = !_showRenders;
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
                foreach (var q in Quad.quads)
                    q?.UpdateVerticePositions();


                void handleClicks()
                {
                    if (Base.Instance.IsActive)
                    {
                        foreach (var parent in UIParent.TotalParents.ToList())
                        {
                            if (parent.Visible)
                            {
                                foreach (var element in parent.Elements)
                                {
                                    if (!element.MouseHovering && element.InteractionBox.ToRectangle().Contains(GameUtils.MousePosition.ToPoint()))
                                    {
                                        element?.MouseOver();
                                        element.MouseHovering = true;
                                    }
                                    else if (element.MouseHovering && !element.InteractionBox.ToRectangle().Contains(GameUtils.MousePosition.ToPoint()))
                                    {
                                        element?.MouseLeave();
                                        element.MouseHovering = false;
                                    }
                                    if (Input.CanDetectClick() && element.MouseHovering)
                                    {
                                        element?.MouseClick();
                                        lastElementClicked = element;
                                        return;
                                    }
                                    if (Input.CanDetectClick(true) && element.MouseHovering)
                                    {
                                        element?.MouseRightClick();
                                        lastElementClicked = element;
                                        return;
                                    }
                                    if (Input.MiddleClick() && element.MouseHovering)
                                    {
                                        element?.MouseMiddleClick();
                                        lastElementClicked = element;
                                        return;
                                    }
                                }
                            }
                        }
                        if (!Input.MouseLeft && !Input.MouseRight && !Input.MouseMiddle)
                        {
                            lastElementClicked = null;
                        }
                    }
                }
                handleClicks();
                if (Base.Instance.IsActive)
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

                if (IngameUI.exitCooldown.ElapsedMilliseconds > 2000)
                    Base.Instance.Exit();

                Update_TestingStuff_REMOVE_LATER_PLEASE();
            }
        }
        internal static void Draw()
        {
            Background.DrawBGs();

            #region GameContent.Draw
            foreach (var booster in BoosterPad.BoosterPads)
                booster?.Draw();
            if (_showRenders)
            {
                // $"{Math.Round(1 / LastCapturedGameTime.ElapsedGameTime.TotalSeconds)}";
                var str1 = $"Render FPS: {Base.RenderFPS}";
                var str2 = $"Logic FPS: {Base.LogicFPS}";
                var str3 = $"Render Time: {Base.RenderTime}";
                var str4 = $"Logic Time: {Base.LogicTime}";

                if (Base.RenderFPS == double.PositiveInfinity)
                    str1 = $"Render FPS: Undefined";

                if (Base.LogicFPS == double.PositiveInfinity)
                    str2 = $"Logic FPS: Undefined";

                var pos1 = new Vector2(0, Base.Fonts.Komika.MeasureString(str1).Y / 2);
                var pos2 = new Vector2(0, Base.Fonts.Komika.MeasureString(str2).Y / 2);
                var pos3 = new Vector2(0, Base.Fonts.Komika.MeasureString(str3).Y / 2);
                var pos4 = new Vector2(0, Base.Fonts.Komika.MeasureString(str4).Y / 2);

                var strOffset = 16;



                GameUtils.DrawStringQuick(str1, new(0, GameUtils.WindowHeight - strOffset * 2), 1f, pos1);
                GameUtils.DrawStringQuick(str2, new(0, GameUtils.WindowHeight - strOffset * 3), 1f, pos2);
                GameUtils.DrawStringQuick(str3, new(0, GameUtils.WindowHeight - strOffset * 4), 1f, pos3);
                GameUtils.DrawStringQuick(str4, new(0, GameUtils.WindowHeight - strOffset * 5), 1f, pos4);
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
                if (parent.Visible)
                    parent?.DrawElements();
            ChatText.DrawAll();
            #endregion
            var orig = Base.Fonts.SilkPixel.MeasureString(ChatText.curTypedText);
            var orig2 = new Vector2(0, orig.Y / 2);
            GameUtils.DrawStringAtMouse(IngameConsole.CurrentlyWrittenText + $"\n\n{Input.DeltaScrollWheel + 1}", new(0, -20));

            int offY = 0;
            foreach (var match in IngameConsole.MatchedStrings)
            {
                GameUtils.DrawStringAtMouse($"{match} | similarity: {StringComparator.CompareTo_GetSimilarity(IngameConsole.CurrentlyWrittenText, match)}%", new(20, offY));
                offY += 30;
            }

            int lol = 0;
            foreach (var task in TestQuest.Objectives)
            {
                GameUtils.DrawStringQuick_IgnoreOrigin($"{task.Name}:{task.Completed}", new(10, 10 + lol));
                lol += 20;
            }
            Base.spriteBatch.DrawString(Base.Fonts.SilkPixel, ChatText.curTypedText, new(20, GameUtils.WindowHeight - 20), Color.White, 0f, orig2, 0.5f, default, 0f);
            DrawExitTimer();
        }

        public static void DrawExitTimer()
        {
            if (IngameUI.exitCooldown.ElapsedMilliseconds > 0)
                GameUtils.DrawStringQuick("Exiting game...", new(GameUtils.WindowWidth / 2, 20), 2f);
        }

        internal static void Initialize()
        {
            // TODO: at load time, set the field bound to the SettingsService property to the SettingsService property
            LoadSettings();
            #region GameContent Init
            LoadableSystem.Load();

            Init_Players();
            IngameConsole.Init();
            foreach (var player in Player.AllPlayers)
                player?.Initialize();

            LightingEngine.InitializeShader();

            for (int i = 0; i < GameUtils.WindowWidth / 16; i++)
            {
                for (int j = 0; j < GameUtils.WindowHeight / 16; j++)
                {
                    new Block(i, j, false, Color.White, true);
                }
            }
            #endregion
            Background.SetBackground(0);

            Console.WriteLine(ForestBG.id);
            Console.WriteLine(Background.currentBGId);
            IngameUI.Initialize_PauseMenu();
            IngameUI.Initialize_ConfirmExit();
            RPCHandler.Load();

            foreach (var elem in UIElement.AllUIElements)
                elem.OnMouseOver += OnMouseOver;
        }

        private static void OnMouseOver(UIElement affectedElement)
        {
            var sfx = Resources.GetGameResource<SoundEffect>("UITick");

            SoundPlayer.PlaySoundInstance(sfx, 0.1f);
        }

        internal static void Exit()
        {
            RPCHandler.Terminate();
            LoadableSystem.Unload();
            SaveSettings();
        }
        private static void Init_Players()
        {
            var player = new Player(TextureLoader.GetTexture("Particle"))
            {
                position = new Vector2(200, 200)
            };
            // TODO: When done, remove this
#pragma warning disable CS4014
            new GenPattern(150 / 16, 400 / 16, 1, 0, 0, 10, 20).Generate();
#pragma warning restore
        }
        public static void SaveSettings()
        {
            SettingsHandler.Serialize(Base.JsonOptions_IncludeFields, true);
            Console.WriteLine($"Settings safely saved.");
        }
        public static void LoadSettings()
        {
            if (!File.Exists(SettingsHandler.JsonPath))
            {
                SettingsHandler.Serialize(Base.JsonOptions_IncludeFields, true);
            }
            SettingsService = SettingsHandler.DeserializeAndSet<SettingsService>();
            Console.WriteLine($"Settings safely loaded.");
        }
        public static void Update_TestingStuff_REMOVE_LATER_PLEASE()
        {
            Stage stage = new("CustomStage");
            if (Input.KeyJustPressed(Keys.Z))
            {
                Stage.SaveStage(stage);
            }
            if (Input.KeyJustPressed(Keys.X))
            {
                Stage.LoadStage(stage.Name);
                Stage.SetStage(stage);
            }

            if (Input.KeyJustPressed(Keys.OemTilde))
                IngameConsole.Enabled = !IngameConsole.Enabled;

            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.I))
            {
                Player.AllPlayers[0].Damage(1);
                //LightingEngine.CreateLight(GameUtils.MousePosition / GameUtils.WindowHeight, 1f, 50f, Color.White);
            }
            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.P))
            {
                Player.AllPlayers[0].Heal(1);
            }
            /*if (Input.FirstPressedKey.IsNum(out int num))
            {
                Background.SetBackground(num);
            }*/
        }

        public static class IngameUI
        {
            public static Stopwatch exitCooldown = new();

            #region Region_PauseMenu
            internal static UIParent parent_PauseMenu = new();

            internal static UITextButton elem_PauseResume;
            internal static UITextButton elem_PauseOptions;
            internal static UITextButton elem_PauseExit;
            #endregion
            #region Region_ConfirmExit
            internal static UIParent parent_confirmExit = new();

            internal static UIText elem_confirmExitText;
            internal static UITextButton elem_confirmExit;
            internal static UITextButton elem_declineExit;
            #endregion
            #region Region_OptionsMenu
            internal static UIParent parent_options = new();
            #endregion
            internal static bool InOptions { get; set; }
            public static void Initialize_PauseMenu()
            {
                elem_PauseResume = new("Resume", Base.Fonts.Lato, Color.White, Color.Gray, 1f);
                elem_PauseResume.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2, 250, 200f, 100f);

                elem_PauseOptions = new("Options", Base.Fonts.Lato, Color.White, Color.Gray, 1f);
                elem_PauseOptions.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2, 500, 200f, 100f);

                elem_PauseExit = new("Exit", Base.Fonts.Lato, Color.White, Color.Gray, 1f);
                elem_PauseExit.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2, 750, 200f, 100f);

                elem_PauseExit.OnMouseClick += Elem_PauseExit_OnMouseClick;
                elem_PauseOptions.OnMouseClick += OpenOptionsMenu;

                parent_PauseMenu.AppendElement(elem_PauseResume);
                parent_PauseMenu.AppendElement(elem_PauseOptions);
                parent_PauseMenu.AppendElement(elem_PauseExit);
            }

            private static void OpenOptionsMenu(UIElement affectedElement)
            {
                Console.WriteLine("Normally, this would open the options menu. But the options menu isn't finished yet! Stay tuned.");
            }

            public static void Initialize_ConfirmExit()
            {
                elem_confirmExitText = new("Are you sure you want to leave?", Base.Fonts.Lato, Color.White, 1f);
                elem_confirmExitText.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2, GameUtils.WindowHeight / 2 - 100, elem_confirmExitText.Font.MeasureString(elem_confirmExitText.Text).X, elem_confirmExitText.Font.MeasureString(elem_confirmExitText.Text).Y);

                elem_confirmExit = new("Yes", Base.Fonts.Lato, Color.White, Color.Green);
                elem_confirmExit.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2 - 75, GameUtils.WindowHeight / 2, 100, 50);
                elem_confirmExit.OnMouseClick += ExitGame_ThroughButton;

                elem_declineExit = new("No", Base.Fonts.Lato, Color.White, Color.Red);
                elem_declineExit.InteractionBox = RectangleF.AtCenter(GameUtils.WindowWidth / 2 + 75, GameUtils.WindowHeight / 2, 100, 50);
                elem_declineExit.OnMouseClick += DeclineGameExit;

                parent_confirmExit.AppendElements(elem_confirmExitText, elem_confirmExit, elem_declineExit);
            }

            private static void DeclineGameExit(UIElement affectedElement) { parent_confirmExit.Visible = false; parent_PauseMenu.Visible = true; }

            private static void ExitGame_ThroughButton(UIElement affectedElement) => exitCooldown.Start();

            private static void Elem_PauseExit_OnMouseClick(UIElement affectedElement) { parent_confirmExit.Visible = true; parent_PauseMenu.Visible = false; }
        }
    }
}