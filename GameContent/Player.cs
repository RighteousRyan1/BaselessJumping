using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using BaselessJumping.Internals.Common.GameInput;
using Microsoft.Xna.Framework.Input;
using BaselessJumping.Internals.Common;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals;
using BaselessJumping.GameContent.Visuals;
using BaselessJumping.GameContent.Physics;
using System.Linq;
using BaselessJumping.GameContent.Powerups;
using BaselessJumping.GameContent.Mechanics;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Audio;

namespace BaselessJumping.GameContent
{
    // TODO: Get Players to work
    public sealed class Player : Entity
    {
        public static List<Player> AllPlayers { get; } = new();

        public HealthBar healthBar;

        public Item[] inventory = new Item[MAX_ITEMS_INVENTORY];

        public const int MAX_POWERUPS = 5;
        public const int MAX_ITEMS_INVENTORY = 3;
        public const int PICKUP_RESET_SATISFACTION = 60;

        public Powerup[] Powerups { get; } = new Powerup[MAX_POWERUPS];

        public Item HeldItem => inventory[_heldId];

        private int _heldId;
        public Keybind ControlDown { get; set; }
        public Keybind ControlUp { get; set; }
        public Keybind ControlLeft { get; set; }
        public Keybind ControlRight { get; set; }
        public Keybind ControlJump { get; set; }
        public Keybind ControlThrowItem { get; set; }

        public Rectangle hitbox;

        public Vector2 Top => new(position.X + (hitbox.X / 2), position.Y);
        public Vector2 Bottom => new(position.X + (hitbox.X / 2), position.Y + hitbox.Height);
        public Vector2 Left => new(position.X, position.Y + (hitbox.Y / 2));
        public Vector2 Right => new(position.X + hitbox.X, position.Y + (hitbox.Y / 2));

        public readonly int width;
        public readonly int height;
        public int direction = 1;
        public int respawnTime = 180;

        private Texture2D texture;

        public static float gravity = 1f;

        /// <summary>The amount of life restored after every given tick.</summary>
        public double lifeRegen = 0.25f;
        // could potentially be made static since data sent will be life, not life regen

        public bool IsCollidingFloor { get; private set; }
        public bool IsCollidingWallLeft { get; private set; }
        public bool IsCollidingWallRight { get; private set; }
        public bool IsCollidingCeiling { get; private set; }
        public bool IsColliding { get; private set; }

        public bool alive = true;
        public static bool noclip = false;
        public static bool immortal = false;

        public GameStopwatch[] pickupCooldowns = new GameStopwatch[Item.TOTAL_ITEMS];
        public GameStopwatch respawnTimer = new();
        public GameStopwatch timeSinceLastDamageTaken = new();

        private bool IsMoving => ControlLeft.IsPressed || ControlRight.IsPressed;

        public Team PvPTeam { get; set; }

        public Rectangle frame;

        public int OnBlockType { get; private set; }

        public List<Vector2> oldPositions = new();

        public PlayerVisuals DetailManager { get; }

        public static float jumpHeight = 1f;
        public static float friction = 1f;
        public static float moveSpeed = 1f;

        // eventually add an ID to the player
        internal Player(Texture2D texture)
        {
            DetailManager = new(this);
            width = texture.Width;
            height = texture.Height;
            this.texture = texture;

            healthBar = new(100, this);

            AllPlayers.Add(this);
        }

        public void Update()
        {
            _heldId = (int)MathHelper.Clamp(_heldId, 0, 2);
            // TODO: finish prim trail
            if (alive)
            {
                hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
                UpdateLife();
                if (!IngameConsole.Enabled)
                    UpdateInput();
                if (ControlThrowItem.JustPressed)
                    ThrowItem(_heldId, out var s);
                UpdateMovement();
                GetTeam();

                if (!hitbox.Intersects(new(-50, -50, GameUtils.WindowWidth + 100, GameUtils.WindowHeight + 100))) // this is a box that extends slightly around the window.
                    Kill();
                if (!noclip)
                {
                    velocity.Y += 0.15f * gravity;

                    UpdateBlockCollision();
                    UpdateRecieveAttack();
                }
            }
            else
                UpdateDead();
        }

        private void UpdateBlockCollision()
        {
            var offset = velocity;
            OnBlockType = 0;
            for (int i = 0; i < 10; i++)
            {
                Collision.CollisionInfo collisionInfo = new();

                collisionInfo.tValue = 1f;
                foreach (var block in Block.Blocks)
                {
                    if (block != null)
                    {
                        if (block.Active)
                        {
                            if (Collision.IsColliding(hitbox, block.CollisionBox, velocity, out var info))
                            {
                                if (info.tValue < collisionInfo.tValue)
                                    collisionInfo = info;

                                if (info.normal.Y < 0)
                                    OnBlockType = block.id;
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
                if (velocity.Y == 0f)
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
        }

        private void UpdateRecieveAttack()
        {

        }

        private void UpdateMovement()
        {
            if (IsCollidingFloor && !IsMoving && velocity.Y == 0)
            {
                switch (OnBlockType)
                {
                    case Block.ID.Grass:
                        velocity.X *= 0.92f / friction;
                        break;
                    case Block.ID.Stone:
                        velocity.X *= 0.81f / friction;
                        break;
                }
            }
        }

        private void UpdateInput()
        {
            if (ControlJump.JustPressed)
            {
                if (velocity.Y == 0f)
                    velocity.Y -= 5f * jumpHeight;
            }
            if (ControlRight.IsPressed)
            {
                if (velocity.X < 3 * moveSpeed)
                {
                    velocity.X += 0.5f * moveSpeed;
                }
            }
            if (ControlLeft.IsPressed)
            {
                if (velocity.X > -3 * moveSpeed)
                {
                    velocity.X -= 0.5f * moveSpeed;
                }
            }

            if (Input.FirstPressedKey.IsNum(out int num))
            {
                if (num < 4)
                    _heldId = num;
            }

            if (Input.MouseMiddle)
            {
                position = GameUtils.MousePosition;
                velocity = GameUtils.GetMouseVelocity() / 8;
            }
        }

        private Color GetTeam()
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
            return getTeamColor();
        }

        private void UpdateLife()
        {
            if (!immortal)
            {
                if (healthBar.currentLife <= 0)
                    Kill();
            }
            #region LifeRegen

            if (timeSinceLastDamageTaken.ElapsedGameTicks > 120) // maybe un-hardcode this soon.
            {
                healthBar.HealLife(lifeRegen);
            }

            healthBar.currentLife = MathHelper.Clamp((float)healthBar.currentLife, 0, (float)healthBar.maxLife);

            #endregion
        }

        private void UpdateDead()
        {
            velocity = Vector2.Zero;
            healthBar.currentLife = 0;

            if (!respawnTimer.IsRunning)
                respawnTimer.Start();

            if (respawnTimer.ElapsedGameTicks > respawnTime)
            {
                Spawn(new(100, 100));
                respawnTimer.Restart();
                respawnTimer.Stop();
            }
            timeSinceLastDamageTaken.Restart();
        }

        public bool Spawn(Vector2 position)
        {
            if (alive)
                return false;

            this.position = position;
            healthBar.currentLife = healthBar.maxLife / 2;
            alive = true;

            var respawn = Resources.GetGameResource<SoundEffect>("PlayerRespawn");

            SoundPlayer.PlaySoundInstance(respawn, SoundContext.Sound);

            for (int i = 0; i < 50; i++)
            {
                var randCircle = position + new Vector2(0, 5).RotatedByRadians(new Random().NextDouble() * new Random().Next(0, 10));
                Particle.SpawnParticle(randCircle, (position / 4) - (randCircle / 4), GetTeam(), 0.6f, 0f);
            }

            return true;
        }

        public void Kill()
        {
            for (int i = 0; i < 50; i++)
                Particle.SpawnParticle(position, new Vector2(0, 5).RotatedByRadians(new Random().NextDouble() * new Random().Next(0, 10)), GetTeam(), 0.6f, 0f);

            var deathSound = Resources.GetGameResource<SoundEffect>("PlayerDeath");

            SoundPlayer.PlaySoundInstance(deathSound, SoundContext.Sound);

            alive = false;
        }

        public void Damage(double damage)
        {
            healthBar.DeductLife(damage);
            timeSinceLastDamageTaken.Restart();
        }

        public void Heal(double life)
        {
            healthBar.HealLife(life);
        }

        public void GrabItem(Item item, out bool successful)
        {
            var asList = inventory.ToList();
            var i = asList.FirstOrDefault(i => i == null);

            var index = asList.IndexOf(i);

            if (index == -1)
            {
                successful = false;
                return;
            }

            i = item;
            inventory[index] = i;
            Item.CopyDataTo(ref i, ref inventory[index]);
            successful = true;
        }

        public void ThrowItem(int inventoryId, out bool successful)
        {
            successful = false;
            if (inventory[inventoryId] != null)
            {

                var iId = inventory[inventoryId].id;
                var item = Item.CreateNew(iId, position);

                Item.CopyDataTo(ref inventory[inventoryId], ref item);

                item.velocity = direction == 1 ? new Vector2(5, -5) : new Vector2(-5, -5);
                inventory[inventoryId] = null;

                pickupCooldowns[iId].Restart();

                successful = true;
            }
        }

        public void Draw()
        {
            if (alive)
            {
                var sb = Base.spriteBatch;
                sb.Draw(texture, hitbox, Color.White);
                GameUtils.DrawHealthBar(healthBar, position + new Vector2(0, 15), 1f, 5f);
                if (hitbox.Contains(GameUtils.MousePosition.ToPoint()))
                    GameUtils.DrawStringQuick($"{healthBar.currentLife} / {healthBar.maxLife}", position + new Vector2(0, 30));
                int x = 0;
                foreach (var i in inventory)
                {
                    if (i != null)
                        sb.DrawString(Base.Fonts.Go, $"{i} : {x}", position - new Vector2(0, 20 * x), Color.White, 0f, Base.Fonts.Go.MeasureString(ToString()) / 2, 0.25f, default, 0f);
                    x++;
                }
                sb.DrawString(Base.Fonts.Go, $"{OnBlockType} | held: {_heldId}", position - new Vector2(0, 20), Color.White, 0f, Base.Fonts.Go.MeasureString($"{OnBlockType}") / 2, 0.25f, default, 0f);
            }
        }

        internal void Initialize()
        {
            GameUtils.PopulateArray(ref pickupCooldowns);

            ControlJump = new("Jump", Keys.Space);
            ControlUp = new("Up", Keys.W);
            ControlDown = new("Down", Keys.S);
            ControlLeft = new("Left", Keys.A);
            ControlRight = new("Right", Keys.D);
            ControlThrowItem = new("ThrowItem", Keys.T);

            foreach (var cd in pickupCooldowns)
                cd?.Start();
            timeSinceLastDamageTaken.Start();

            for (int i = 0; i < 101; i++)
                oldPositions.Add(position);
        }

        public override string ToString()
        {
            return $"vel: {velocity} | pos: {position} | Hitbox: {hitbox}";
        }
    }
}