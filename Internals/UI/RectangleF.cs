using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Internals.UI
{
    /// <summary>
    /// A rectangle with float based math
    /// </summary>
    public struct RectangleF
    {
        public float X;

        public float Y;

        public float Width;

        public float Height;

        public Vector2 Center => new(X + Width / 2, Y + Height / 2);

        public Vector2 CenterOrigin => new(Width / 2, Height / 2);

        public Vector2 Position => new(X, Y);

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public static RectangleF AtCenter(float x, float y, float width, float height)
        {
            return new(x - width / 2, y - height / 2, width, height);
        }
    }
}
