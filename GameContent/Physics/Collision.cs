using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Systems;
using BaselessJumping.Internals;
using BaselessJumping.GameContent.Props;
using BaselessJumping.Internals.Audio;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.UI;
using BaselessJumping.MapGeneration;

namespace BaselessJumping.GameContent.Physics
{
    public static class Collision
    {
        public struct CollisionInfo
        {
            public float tValue;
            public Vector2 normal;
        }
        public static bool IsColliding(Rectangle movingBox, Rectangle collidingBox, Vector2 offset, out CollisionInfo info)
        {
            info = new();
            float horizontalT;
            if (offset.X > 0)
                horizontalT = (float)(collidingBox.Left - movingBox.Right) / (float)(offset.X);
            else if (offset.X < 0)
                horizontalT = (float)(collidingBox.Right - movingBox.Left) / (float)(offset.X);
            else
                horizontalT = -1.0f;

            float verticalT;
            if (offset.Y > 0)
                verticalT = (float)(collidingBox.Top - movingBox.Bottom) / (float)(offset.Y);
            else if (offset.Y < 0)
                verticalT = (float)(collidingBox.Bottom - movingBox.Top) / (float)(offset.Y);
            else
                verticalT = -1.0f;

            bool isHorizontal = true;
            if (horizontalT < 0.0f)
                isHorizontal = false;
            if (horizontalT > 1.0f)
                isHorizontal = false;
            if (collidingBox.Top >= movingBox.Bottom || collidingBox.Bottom <= movingBox.Top)
                isHorizontal = false;

            bool isVertical = true;
            if (verticalT < 0.0f)
                isVertical = false;
            if (verticalT > 1.0f)
                isVertical = false;
            if (collidingBox.Left >= movingBox.Right || collidingBox.Right <= movingBox.Left)
                isVertical = false;

            if (!isHorizontal && !isVertical)
                return false;

            if (!isVertical || (horizontalT < verticalT && isHorizontal))
            {
                info.tValue = horizontalT;
                if (offset.X > 0)
                    info.normal = new(-1.0f, 0.0f);
                else
                    info.normal = new(1.0f, 0.0f);
            }
            else
            {
                info.tValue = verticalT;
                if (offset.Y > 0)
                    info.normal = new(0.0f, -1.0f);
                else
                    info.normal = new(0.0f, 1.0f);
            }
            return true;
        }


        public static void HandleCollisionSimple(ref Rectangle movingBox, Rectangle collidingBox, ref Vector2 velocity, ref Vector2 position)
        {
            CollisionInfo collisionInfo = new();

            collisionInfo.tValue = 1f;

            if (IsColliding(movingBox, collidingBox, velocity, out var info))
            {
                if (info.tValue < collisionInfo.tValue)
                    collisionInfo = info;
            }
            position += velocity * collisionInfo.tValue;

            if (collisionInfo.tValue < 1)
            {
                velocity -= Vector2.Dot(velocity, collisionInfo.normal) * collisionInfo.normal;
                velocity -= Vector2.Dot(velocity, collisionInfo.normal) * collisionInfo.normal;
                velocity *= 1f - collisionInfo.tValue;
            }
            else return;
        }
    }
}