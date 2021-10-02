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

namespace BaselessJumping.GameContent
{
    // TODO: Get Players to work
    public sealed class Player : Entity
    {
        public static List<Player> AllPlayers { get; } = new();

        public Item[] inventory = new Item[3];

        public Item HeldItem => inventory[heldItemId];

        private int heldItemId;
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
        private Texture2D texture;
        public float gravity = 1f;

        public bool IsCollidingFloor { get; private set; }
        public bool IsCollidingWallLeft { get; private set; }
        public bool IsCollidingWallRight { get; private set; }
        public bool IsCollidingCeiling { get; private set; }
        public bool IsColliding { get; private set; }

        public GameStopwatch pickupCooldown = new();
        public const int PICKUP_RESET_SATISFACTION = 60;

        private bool IsMoving => ControlLeft.IsPressed || ControlRight.IsPressed;

        public Team PvPTeam { get; set; }

        public Rectangle frame;

        public int OnBlockType { get; private set; }

        public List<Vector2> oldPositions = new();

        public PlayerVisuals DetailManager { get; }

        internal Player(Texture2D texture)
        {
            DetailManager = new(this);
            width = texture.Width;
            height = texture.Height;
            this.texture = texture;
            AllPlayers.Add(this);
        }

        public void Update()
        {
            heldItemId = (int)MathHelper.Clamp(heldItemId, 0, 2);
            // TODO: finish prim trail
            hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
            if (!IngameConsole.Enabled)
                UpdateInput();
            if (ControlThrowItem.JustPressed)
                ThrowItem(heldItemId, out var s);
            UpdateMovement();
            UpdateTeam();
            if (!hitbox.Intersects(new(-50, -50, GameUtils.WindowWidth + 100, GameUtils.WindowHeight + 100)))
                velocity = Vector2.Zero;
            if (!IngameConsole.cheats_noclip)
            {
                velocity.Y += 0.15f * gravity;

                UpdateBlockCollision();
                UpdateRecieveAttack();
            }
            /*oldPositions.RemoveAt(100);
                oldPositions.Add(position);

                ChatText.NewText(oldPositions[100]);*/
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
                var fric = IngameConsole.phys_playerfriction;
                switch (OnBlockType)
                {
                    case Block.ID.Grass:
                        velocity.X *= 0.92f / fric;
                        break;
                    case Block.ID.Stone:
                        velocity.X *= 0.81f / fric;
                        break;
                }
            }
        }
        private void UpdateInput()
        {
            if (ControlJump.JustPressed)
            {
                if (velocity.Y == 0f)
                    velocity.Y -= 5f * IngameConsole.cheats_playerjumpheight;
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

            if (Input.FirstPressedKey.IsNum(out int num))
            {
                if (num < 4)
                    heldItemId = num;
            }

            if (Input.MouseMiddle)
            {
                position = GameUtils.MousePosition;
                velocity = GameUtils.GetMouseVelocity();
            }
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
                pickupCooldown.Restart();

                var iId = inventory[inventoryId].id;
                var item = Item.CreateNew(iId, position);

                Item.CopyDataTo(ref inventory[inventoryId], ref item);

                item.velocity = direction == 1 ? new Vector2(5, -5) : new Vector2(-5, -5);
                inventory[inventoryId] = null;
                successful = true;
            }
        }

        public void Draw()
        {
            var sb = BJGame.spriteBatch;
            sb.Draw(texture, hitbox, Color.White);
            int x = 0;
            foreach (var i in inventory)
            {
                if (i != null)
                    sb.DrawString(BJGame.Fonts.Go, $"{i} : {x}", position - new Vector2(0, 20 * x), Color.White, 0f, BJGame.Fonts.Go.MeasureString(ToString()) / 2, 0.25f, default, 0f);
                x++;
            }
            sb.DrawString(BJGame.Fonts.Go, $"{OnBlockType} | held: {heldItemId}", position - new Vector2(0, 20), Color.White, 0f, BJGame.Fonts.Go.MeasureString($"{OnBlockType}") / 2, 0.25f, default, 0f);
        }

        public void Initialize()
        {
            ControlJump = new("Jump", Keys.Space);
            ControlUp = new("Up", Keys.W);
            ControlDown = new("Down", Keys.S);
            ControlLeft = new("Left", Keys.A);
            ControlRight = new("Right", Keys.D);
            ControlThrowItem = new("ThrowItem", Keys.T);

            pickupCooldown.Start();

            for (int i = 0; i < 101; i++)
                oldPositions.Add(position);
        }

        public override string ToString()
        {
            return $"vel: {velocity} | pos: {position} | Hitbox: {hitbox}";
        }
    }
}