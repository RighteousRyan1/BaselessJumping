using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BaselessJumping.Internals.Common
{
    public static class Utilities
    {
        public static float GetRotationVectorOf(Vector2 initial, Vector2 target) => (target - initial).ToRotation();
        public static float ToRotation(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
        public static Vector2 RotatedByRadians(this Vector2 spinPoint, double radians, Vector2 center = default)
        {
            float cosRotation = (float)Math.Cos(radians);
            float sinRotation = (float)Math.Sin(radians);
            Vector2 newPoint = spinPoint - center;
            Vector2 result = center;
            result.X += newPoint.X * cosRotation - newPoint.Y * sinRotation;
            result.Y += newPoint.X * sinRotation + newPoint.Y * cosRotation;
            return result;
        }
        public static Vector3 ToVector3(this Vector2 twoD) => new(twoD.X, twoD.Y, 0f);
        public static Vector2 DistanceFrom(this Vector2 start, Vector2 target) => target - start;
        public static Vector2 MousePosition => new(Input.CurrentMouseSnapshot.X, Input.CurrentMouseSnapshot.Y);
        public static Vector2 MousePosition_ToBlockCoordinates => MousePosition / 16;
        public static int MouseX => (int)MousePosition.X;
        public static int MouseY => (int)MousePosition.Y;
        public static int MouseX_TBC => (int)MousePosition.X / 16;
        public static int MouseY_TBC => (int)MousePosition.Y / 16;
        public static int WindowWidth => BJGame.Instance.Window.ClientBounds.Width;
        public static int WindowHeight => BJGame.Instance.Window.ClientBounds.Height;
        public static Vector2 WindowBounds => new(WindowWidth, WindowHeight);
        public static Vector2 WindowCenter => WindowBounds / 2;
        public static Vector2 WindowBottom => new(WindowWidth / 2, WindowHeight);
        public static Vector2 WindowTop => new(WindowWidth / 2, 0);
        public static Vector2 WindowTopRight => new(WindowWidth, 0);
        public static Vector2 WindowBottomRight => new(WindowWidth, WindowHeight);
        public static Vector2 WindowTopLeft => new(0, 0);
        public static Vector2 WindowBottomLeft => new(0, WindowHeight);
        public static Vector2 WindowLeft => new(0, WindowHeight / 2);
        public static Vector2 WindowRight => new(WindowWidth, WindowHeight / 2);
        public static Vector2 Size(this Texture2D tex) => new(tex.Width, tex.Height);
        public static bool MouseOnScreen => MousePosition.X >= 0 && MousePosition.X <= WindowWidth && MousePosition.Y >= 0 && MousePosition.Y < WindowHeight;
        public static bool MouseOnScreenProtected => MousePosition.X > 16 && MousePosition.X < WindowWidth - 16 && MousePosition.Y > 16 && MousePosition.Y < WindowHeight - 16;
        public static void DrawTextWithBorder(SpriteFont font, string text, Vector2 pos, Color color, Color borderColor, float rot, float scale, int borderSize)
        {
            var origin = font.MeasureString(text) / 2;
            int yOffset = 0;
            int xOffset = 0;
            for (int i = 0; i < borderSize + 3; i++)
            {
                if (i == 0)
                    xOffset = -borderSize;
                if (i == 1)
                    xOffset = borderSize;
                if (i == 2)
                    yOffset = -borderSize;
                if (i == 3)
                    yOffset = borderSize;


                BJGame.spriteBatch.DrawString(font, text, pos + new Vector2(xOffset, yOffset), borderColor, rot, origin, scale, default, 0f);
            }
            BJGame.spriteBatch.DrawString(font, text, pos, color, rot, origin, scale, default, 0f);
        }
        public static void DrawTextureWithBorder(Texture2D texture, Vector2 pos, Color color, Color borderColor, float rot, float scale, int borderSize)
        {
            var origin = texture.Size() / 2;
            int yOffset = 0;
            int xOffset = 0;
            for (int i = 0; i < borderSize + 3; i++)
            {
                if (i == 0)
                    xOffset = -borderSize;
                if (i == 1)
                    xOffset = borderSize;
                if (i == 2)
                    yOffset = -borderSize;
                if (i == 3)
                    yOffset = borderSize;


                BJGame.spriteBatch.Draw(texture, pos + new Vector2(xOffset, yOffset), null, borderColor, rot, origin, scale, default, 0f);
            }
            BJGame.spriteBatch.Draw(texture, pos, null, color, rot, origin, scale, default, 0f);
        }
        public static T[,] Resize2D<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
        public static Point ToPoint(this Vector2 vector2) => new((int)vector2.X, (int)vector2.Y);
        public static bool WindowActive => BJGame.Instance.IsActive;
        private static List<int> chosenTs = new();
        public static T PickRandom<T>(T[] input)
        {
            int rand = new Random().Next(0, input.Length);

            return input[rand];
        }
        public static List<T> PickRandom<T>(T[] input, int amount)
        {
            List<T> values = new();
            for (int i = 0; i < amount; i++)
            {
            ReRoll:
                int rand = new Random().Next(0, input.Length);

                if (!chosenTs.Contains(rand))
                {
                    chosenTs.Add(rand);
                    values.Add(input[rand]);
                }
                else
                    goto ReRoll;
            }
            chosenTs.Clear();
            return values;
        }
        public static void DrawStringAtMouse(object text) => BJGame.spriteBatch.DrawString(BJGame.Fonts.Komika, text.ToString(), MousePosition + new Vector2(25), Color.White, 0f, Vector2.Zero, 0.25f, default, 0f);
    }
}