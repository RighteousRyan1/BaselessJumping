using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;

namespace BaselessJumping.GameContent
{
    public abstract class Entity
    {
        // I know entities should only store IDs, but too bad

        public Vector2 velocity;
        public Vector2 oldVelocity;

        public Vector2 position;
        public Vector2 oldPosition;

        public readonly int whoAmI;

        public bool active;

        public Rectangle Hitbox { get; set; }
    }
}