using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System;

namespace BaselessJumping.GameContent
{
    public abstract class Entity
    {
        // I know entities should only store IDs, but too bad

        public static List<Entity> AllEntities { get; } = new();

        public Vector2 velocity;

        public Vector2 position;

        public bool InWorld { get; set; }
    }
}