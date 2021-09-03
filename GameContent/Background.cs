using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals;

namespace BaselessJumping.GameContent
{
    public sealed class Background
    {
        public static List<Background> Backgrounds { get; } = new();
        public static float fadeSpeed = 0.01f;

        public float Alpha { get; private set; }
        private static int currentBGId = -1;

        public Texture2D Texture { get; }
        public readonly int id;


        public Background(string texturePath)
        {
            id = Backgrounds.Count;
            Texture = BJGame.Instance.Content.GetResource<Texture2D>(texturePath);

            Backgrounds.Add(this);
        }

        public Background(Texture2D texture)
        {
            id = Backgrounds.Count;
            Texture = texture;

            Backgrounds.Add(this);
        }
        public static void UpdateBGs()
        {
            foreach (var bg in Backgrounds)
            {
                if (currentBGId == bg.id)
                {
                    bg.Alpha += fadeSpeed;
                }
                else
                {
                    bg.Alpha -= fadeSpeed;
                }
                bg.Alpha = MathHelper.Clamp(bg.Alpha, 0f, 1f);
            }
        }
        public static void DrawBGs()
        {
            if (!IngameConsole.CommandValues.DrawBackgrounds)
                return;
            foreach (var bg in Backgrounds)
            {
                if (currentBGId == bg.id)
                {
                    BJGame.spriteBatch.Draw(bg.Texture, new Rectangle(0, 0, GameUtils.WindowWidth, GameUtils.WindowHeight), Color.White * bg.Alpha);
                }
            }
        }
        /// <summary>
        /// Set the current background of the game (Set to -1 for none)
        /// </summary>
        /// <param name="id"></param>
        public static void SetBackground(int id)
        {
            if (id >= Backgrounds.Count)
                currentBGId = Backgrounds.Count - 1;
            else
                currentBGId = id;
        }
    }
}