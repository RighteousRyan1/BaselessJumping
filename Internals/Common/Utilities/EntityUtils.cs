using BaselessJumping.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BaselessJumping.Internals.Common.Utilities
{
    public static class EntityUtils
    {
        public static float Distance(this Entity entity, Vector2 other) => Vector2.Distance(entity.position, other);
        public static float Distance(this Entity entity1, Entity entity2) => Vector2.Distance(entity1.position, entity2.position);

        /*public static Entity FindClosest(Vector2 position)
        {
            foreach (var entity in )
        }*/
    }
}