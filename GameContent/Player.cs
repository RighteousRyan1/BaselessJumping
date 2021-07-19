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
        public int direction;

        private bool _canMoveLeft;
        private bool _canMoveRight;
        /*private bool _canMoveUp;
        private bool _canMoveDown;*/

        public Team PvPTeam { get; set; }

        public Rectangle frame;
        private Color auraColor;

        public Texture2D Texture { get; private set; }

        internal Player(Texture2D texture)
        {
            Texture = texture;
            AllPlayers.Add(this);
        }

        // private Rectangle FutureHitbox => new(Hitbox.X + (int)velocity.X, Hitbox.Y + (int)velocity.Y, width, height);

        private bool IsMoving => ControlLeft.IsPressed || ControlRight.IsPressed;
        private bool _collideUnder;

        public void Update()
        {
            UpdateVelocity();
            UpdateTileCollision();
            UpdateMovement();
            UpdateRecieveAttack();
            UpdateTeam();

            Hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);

            if (!Hitbox.Intersects(new(-50, -50, Utilities.WindowWidth + 100, Utilities.WindowHeight + 100)))
                velocity = Vector2.Zero;

            oldVelocity = velocity;
            oldPosition = position;
        }
        private void UpdateVelocity()
        {
            position += velocity;

            velocity.Y += 0.075f;
        }
        private void UpdateTileCollision()
        {
            // Rectangle
            // Vector2
            // Vector3
            // Texture2D

            var blockDown = Block.Methods.GetValidBlock((int)position.X / 16, (int)position.Y / 16 + 1);
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
            }

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
                // if (blockCollide)
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
            Draw_AuraPulsing();
            sb.Draw(Texture, Hitbox, Color.White);
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
            var texture = Texture;

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
    }
}