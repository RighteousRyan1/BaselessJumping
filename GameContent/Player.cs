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

        private bool _canMoveLeft = true;
        private bool _canMoveRight = true;
        /*private bool _canMoveUp;
        private bool _canMoveDown;*/

        public Team PvPTeam { get; set; }

        public Rectangle frame;
        private Color auraColor;

        internal Player()
        {
            AllPlayers.Add(this);
        }

        // private Rectangle FutureHitbox => new(Hitbox.X + (int)velocity.X, Hitbox.Y + (int)velocity.Y, width, height);

        private bool IsMoving => ControlLeft.IsPressed || ControlRight.IsPressed;

        public void Update()
        {
            velocity.Y += 0.075f;

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
            position += velocity * collisionInfo.tValue;

            if (collisionInfo.tValue < 1)
            {
                velocity -= Vector2.Dot(velocity, collisionInfo.normal) * collisionInfo.normal;

            }

            /*var blockDown = Block.Methods.GetValidBlock((int)position.X / 16, (int)position.Y / 16 + 1);
            var blockUp = Block.Methods.GetValidBlock((int)position.X / 16, (int)position.Y / 16 - 1);
            var blockLeft = Block.Methods.GetValidBlock((int)position.X / 16 - 1, (int)position.Y / 16);
            var blockRight = Block.Methods.GetValidBlock((int)position.X / 16 + 1, (int)position.Y / 16);

            _canMoveRight = true;
            _canMoveLeft = true;

            if (blockDown.Active && blockDown.HasCollision)
            {
                if (velocity.Y > 0)
                    velocity.Y = 0;
                _collideUnder = true;
            }
            else
                _collideUnder = false;
            if (blockUp.Active && blockUp.HasCollision)
            {
                if (velocity.Y < 0)
                    velocity.Y = 0;
            }
            if (blockLeft.Active && blockLeft.HasCollision)
            {
                _canMoveLeft = false;
                if (velocity.X < 0)
                    velocity.X = 0;
            }
            if (blockRight.Active && blockRight.HasCollision)
            {
                _canMoveRight = false;
                if (velocity.X > 0)
                    velocity.X = 0;
            }*/

            if (Input.MouseMiddle)
            {
                position = Utilities.MousePosition;
                velocity = Vector2.Zero;
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
            if (ControlRight.IsPressed && _canMoveRight)
            {
                if (velocity.X < 3)
                {
                    velocity.X += 0.5f;
                }
            }
            if (ControlLeft.IsPressed && _canMoveLeft)
            {
                if (velocity.X > -3)
                {
                    velocity.X -= 0.5f;
                }
            }
            if (!IsMoving && _collideUnder)
                velocity.X *= 0.25f;
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
            // Draw_AuraPulsing();
            sb.Draw(BJGame.Textures.WhitePixel, Hitbox, Color.White);
            sb.DrawString(BJGame.Fonts.Go, ToString(), position - new Vector2(0, 10), Color.White, 0f, BJGame.Fonts.Go.MeasureString(ToString()) / 2, 0.25f, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally , 0f);
        }
        private float _pulseScale;
        private float _pulseAlpha;

        private int _pulseCooldown;
        /// <summary>
        /// This needs some concrete finishing.
        /// </summary>
        private void Draw_AuraPulsing()
        {
            var sb = BJGame.spriteBatch;
            var texture = BJGame.Textures.WhitePixel;

            if (_pulseCooldown > 0)
            {
                _pulseCooldown--;
                _pulseScale += 0.025f;
                sb.Draw(texture, position, frame, auraColor * _pulseAlpha, 0f /* Maybe changed once player rotation is implemented. */, texture.Size() / 2, _pulseScale, direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            else
            {
                _pulseScale = 0.95f;
            }
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
            if (testBox.Top > movingBox.Bottom || testBox.Bottom < movingBox.Top)
                isHorizontal = false;

            bool isVertical = true;
            if (verticalT < 0.0f)
                isVertical = false;
            if (verticalT > 1.0f)
                isVertical = false;
            if (testBox.Left > movingBox.Right || testBox.Right < movingBox.Left)
                isVertical = false;

            ChatText.NewText($"hor: {horizontalT} | vert: {verticalT} | isHor: {isHorizontal} | isVert: {isVertical}");

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
            ChatText.NewText($"normal: {info.normal}");
            return true;
        }
    }
}