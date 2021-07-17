using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaselessJumping.Internals.Common
{
    public sealed class Drawing
    {
        public static Vector2 ScreenBounds => new(BJGame.Instance.Window.ClientBounds.X, BJGame.Instance.Window.ClientBounds.Y);
    }
}