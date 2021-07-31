using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaselessJumping.GameContent
{
    public class Enemy : Entity
    {
        // used for determining enemy actions
        public Action<Enemy> AI;

        public Texture2D texture;

        public Rectangle frame;
    }
}