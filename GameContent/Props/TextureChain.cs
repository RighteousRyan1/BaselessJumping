using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.GameContent;
using BaselessJumping.GameContent.Shapes;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.UI;
using BaselessJumping.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.GameContent.Props
{
    /// <summary>
    /// WIP for now
    /// </summary>
    public class TextureChain
    {
        public Texture2D Texture { get; }
        public int Length { get; set; }

        public Vector2 startPoint;
        public Vector2 endPoint;

        public float scale;
        private float _distPerChain;

        public float rotationInRadians;

        public TextureChain(Texture2D texture, int length, Vector2 beginning, Vector2 destination, float scale = 1f)
        {
            Texture = texture;
            Length = length;

            startPoint = beginning;
            endPoint = destination;

            this.scale = scale;

            _distPerChain = texture.Height;
        }

        public void Draw()
        {
            var origin = new Vector2(Texture.Width / 2, 0);
            Base.spriteBatch.Begin();
            for (int i = 0; i < Length; i++)
            {
                // TODO: Draw the stuff
                Base.spriteBatch.Draw(Texture, startPoint * i, null, Color.White, rotationInRadians, origin, scale, default, 0f);

            }
            Base.spriteBatch.End();
        }


        public void OverrideChainDistance(float f)
            => _distPerChain = f;
    }
}
