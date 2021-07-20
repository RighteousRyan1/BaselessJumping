using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using BaselessJumping.Internals.Common.Systems;
using Microsoft.Xna.Framework.Input;
using BaselessJumping.Internals.Common;
using BaselessJumping.Enums;

namespace BaselessJumping.GameContent
{
    // TODO: Get Players to work
    public sealed class Player : Entity
    {
        public static List<Player> AllPlayers { get; set; } = new();

        public Keybind ControlDown { get; set; }
        public Keybind ControlUp { get; set; }
        public Keybind ControlLeft { get; set; }
        public Keybind ControlRight { get; set; }
        public Keybind ControlJump { get; set; }

        public Vector2 Top => new(position.X + (Hitbox.X / 2), position.Y);

        public Vector2 Bottom => new(position.X + (Hitbox.X / 2), position.Y + Hitbox.Height);

        public Vector2 Left => new(position.X, position.Y + (Hitbox.Y / 2));

        public Vector2 Right => new(position.X + Hitbox.X, position.Y + (Hitbox.Y / 2));

        public int width;
        public int height;
        public int direction = 1;

        public bool IsCollidingFloor { get; private set; }
        public bool IsCollidingWallLeft { get; private set; }
        public bool IsCollidingWallRight { get; private set; }
        public bool IsCollidingCeiling { get; private set; }
        public bool IsColliding { get; private set; }

        private bool IsMoving => ControlLeft.IsPressed || ControlRight.IsPressed;

        public Team PvPTeam { get; set; }

        public Rectangle frame;
        private Color auraColor;

        internal Player()
        {
            AllPlayers.Add(this);
        }

        public void Update()
        {
            velocity.Y += 0.1f;

            UpdateBlockCollision();
            UpdateMovement();
            UpdateRecieveAttack();
            UpdateTeam();

            Hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);

            if (!Hitbox.Intersects(new(-50, -50, Utilities.WindowWidth + 100, Utilities.WindowHeight + 100)))
                velocity = Vector2.Zero;

            oldVelocity = velocity;
            oldPosition = position;
        }
        private void UpdateBlockCollision()
        {
            // Rectangle
            // Vector2
            // Vector3
            // Texture2D

            var offset = velocity;

            for (int i = 0; i < 10; i++)
            {
                BoxCastInfo collisionInfo = new();

                collisionInfo.tValue = 1f;

                foreach (var block in Block.Blocks)
                {
                    if (block != null)
                    {
                        if (block.Active)
                        {
                            if (BoxCast(Hitbox, block.CollisionBox, velocity, out var info))
                            {
                                if (info.tValue < collisionInfo.tValue)
                                    collisionInfo = info;
                            }
                        }
                    }
                }
                position += offset * collisionInfo.tValue;

                if (collisionInfo.tValue < 1)
                {
                    velocity -= Vector2.Dot(velocity, collisionInfo.normal) * collisionInfo.normal;
                    offset -= Vector2.Dot(offset, collisionInfo.normal) * collisionInfo.normal;
                    offset *= 1f - collisionInfo.tValue;
                }
                else
                    break;

                IsColliding = false;
                IsCollidingWallLeft = false;
                IsCollidingCeiling = false;
                IsCollidingFloor = false;
                IsCollidingWallRight = false;

                if (collisionInfo.normal.Y > 0)
                {
                    // ceil
                    IsCollidingCeiling = true;
                    IsColliding = true;
                }
                if (collisionInfo.normal.Y < 0)
                {
                    // floor
                    IsCollidingFloor = true;
                    IsColliding = true;
                }
                if (collisionInfo.normal.X > 0)
                {
                    // wall left
                    IsCollidingWallLeft = true;
                    IsColliding = true;
                }
                if (collisionInfo.normal.X < 0)
                {
                    // wall right
                    IsCollidingWallRight = true;
                    IsColliding = true;
                }
            }
            if (Input.MouseMiddle)
            {
                position = Utilities.MousePosition;
                velocity = Utilities.GetMouseVelocity();
            }
        }
        private void UpdateRecieveAttack()
        {

        }
        private void UpdateMovement()
        {
            if (ControlJump.JustPressed)
            {
                if (velocity.Y == 0f)
                    velocity.Y -= 4f;
            }
            if (ControlRight.IsPressed)
            {
                if (velocity.X < 3)
                {
                    velocity.X += 0.5f;
                }
            }
            if (ControlLeft.IsPressed)
            {
                if (velocity.X > -3)
                {
                    velocity.X -= 0.5f;
                }
            }
            if (IsCollidingFloor && !IsMoving && velocity.Y == 0)
                velocity.X *= 0.35f;
        }
        private void UpdateTeam()
        {
            Color getTeamColor()
            {
                if (PvPTeam == Team.Red)
                    return Color.Red;
                if (PvPTeam == Team.Blue)
                    return Color.Blue;
                if (PvPTeam == Team.Green)
                    return Color.Green;
                if (PvPTeam == Team.Yellow)
                    return Color.Yellow;
                if (PvPTeam == Team.Orange)
                    return Color.Orange;
                if (PvPTeam == Team.Purple)
                    return Color.Purple;
                if (PvPTeam == Team.Pink)
                    return Color.Pink;
                if (PvPTeam == Team.Cyan)
                    return Color.Cyan;

                return Color.Gray; // This should be when PvPTeam is None
            }
            auraColor = getTeamColor();
        }
        public void Draw()
        {
            var sb = BJGame.spriteBatch;
            DrawAura();
            sb.Draw(BJGame.Textures.WhitePixel, Hitbox, Color.White);
            sb.DrawString(BJGame.Fonts.Go, ToString(), position - new Vector2(0, 10), Color.White, 0f, BJGame.Fonts.Go.MeasureString(ToString()) / 2, 0.25f, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally , 0f);
        }
        /// <summary>
        /// This needs some concrete finishing.
        /// </summary>
        private void DrawAura()
        {
            var sb = BJGame.spriteBatch;
            var texture = BJGame.Textures.WhitePixel;
            sb.Draw(texture, position, frame, auraColor * 0.4f, 0f /* Maybe changed once player rotation is implemented. */, texture.Size() / 2, 1.25f, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        public void Initialize()
        {
            ControlJump = new("Jump", Keys.Space);
            ControlUp = new("Up", Keys.W);
            ControlDown = new("Down", Keys.S);
            ControlLeft = new("Left", Keys.A);
            ControlRight = new("Right", Keys.D);
            width = 25;
            height = 25;
        }

        public override string ToString()
        {
            return $"vel: {velocity} | oldVel {oldVelocity} | pos: {position} | oldPos: {oldPosition} | Hitbox: {Hitbox}";
        }
        private struct BoxCastInfo
        {
            public float tValue;
            public Vector2 normal;
        }
        private bool BoxCast(Rectangle movingBox, Rectangle testBox, Vector2 offset, out BoxCastInfo info)
        {
            info = new();
            float horizontalT;
            if (offset.X > 0)
                horizontalT = (float)(testBox.Left - movingBox.Right) / (float)(offset.X);
            else if (offset.X < 0)
                horizontalT = (float)(testBox.Right - movingBox.Left) / (float)(offset.X);
            else
                horizontalT = -1.0f;

            float verticalT;
            if (offset.Y > 0)
                verticalT = (float)(testBox.Top - movingBox.Bottom) / (float)(offset.Y);
            else if (offset.Y < 0)
                verticalT = (float)(testBox.Bottom - movingBox.Top) / (float)(offset.Y);
            else
                verticalT = -1.0f;

            bool isHorizontal = true;
            if (horizontalT < 0.0f)
                isHorizontal = false;
            if (horizontalT > 1.0f)
                isHorizontal = false;
            if (testBox.Top >= movingBox.Bottom || testBox.Bottom <= movingBox.Top)
                isHorizontal = false;

            bool isVertical = true;
            if (verticalT < 0.0f)
                isVertical = false;
            if (verticalT > 1.0f)
                isVertical = false;
            if (testBox.Left >= movingBox.Right || testBox.Right <= movingBox.Left)
                isVertical = false;

            // ChatText.NewText($"hor: {horizontalT} | vert: {verticalT} | isHor: {isHorizontal} | isVert: {isVertical}");

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
            // ChatText.NewText($"normal: {info.normal}");
            return true;
        }
    }
}