using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using BaselessJumping.Internals.Common.Systems;
using Microsoft.Xna.Framework.Input;
using BaselessJumping.Internals.Common;

namespace BaselessJumping.GameContent
{
    // TODO: Get Players to work
    public sealed class Player : Entity
    {
        public static List<Player> AllPlayers { get; set; } = new();
        internal Player() { AllPlayers.Add(this); }

        public Keybind ControlDown { get; set; }
        public Keybind ControlUp { get; set; }
        public Keybind ControlLeft { get; set; }
        public Keybind ControlRight { get; set; }
        public Keybind ControlJump { get; set; }

        private Vector2 predictedPosition;

        public Vector2 Top => new(position.X + (Hitbox.X / 2), position.Y);

        public Vector2 Bottom => new(position.X + (Hitbox.X / 2), position.Y + Hitbox.Height);

        public Vector2 Left => new(position.X, position.Y + (Hitbox.Y / 2));

        public Vector2 Right => new(position.X + Hitbox.X, position.Y + (Hitbox.Y / 2));

        public int width;
        public int height;


        private bool _canMoveLeft;
        private bool _canMoveRight;
        private bool _canMoveUp;
        private bool _canMoveDown;

        private Rectangle FutureHitbox => new(Hitbox.X + (int)velocity.X, Hitbox.Y + (int)velocity.Y, width, height);

        private bool IsMoving => !ControlLeft.IsPressed && !ControlRight.IsPressed;

        public void Update()
        {
            Update_Velocity();
            Update_TileCollision();
            Update_Movement();
            Update_HandleEnemyAttack();

            Hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);

            if (!Hitbox.Intersects(new(-50, -50, Utilities.WindowWidth + 100, Utilities.WindowHeight + 100)))
                velocity = Vector2.Zero;

            oldVelocity = velocity;
            oldPosition = position;
        }
        private void Update_Velocity()
        {
            predictedPosition = position + velocity;
            position += velocity;

            velocity.Y += 0.075f;
        }
        private void Update_TileCollision()
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
            }
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
        private void Update_HandleEnemyAttack()
        {

        }
        private void Update_Movement()
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
            if (!IsMoving)
                velocity.X *= 0.25f;
        }
        public void Draw()
        {
            var sb = BJGame.spriteBatch;
            sb.Draw(BJGame.Textures.WhitePixel, Hitbox, Color.White);
            sb.DrawString(BJGame.Fonts.Go, ToString(), position - new Vector2(0, 10), Color.White, 0f, BJGame.Fonts.Go.MeasureString(ToString()) / 2, 0.25f, default, 0f);
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