using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BaselessJumping.Internals.Common
{
    public sealed class Drawing
    {
        public static Vector2 ScreenBounds => new(Base.Instance.Window.ClientBounds.X, Base.Instance.Window.ClientBounds.Y);
    }
}