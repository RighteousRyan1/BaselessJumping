using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.Internals.Common.Utilities
{
    public static class DrawUtils
    {
        public static Vector2 ToNormalizedCoordinates(this Vector2 orig)
        {
            var display = GameUtils.GetNormalDisplay();
            
            var x = GameUtils.InverseLerp(0, display.X, orig.X, false);
            var y = GameUtils.InverseLerp(0, display.Y, orig.Y, false);
            return new(x, y);
        }

        public static void DrawDebugBox(Rectangle box)
        {
            Base.spriteBatch.Draw(Resources.GetGameResource<Texture2D>("WhitePixel"), box, Color.White * 0.5f);
        }
    }
}
