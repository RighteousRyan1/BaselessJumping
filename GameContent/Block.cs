using BaselessJumping.Audio;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaselessJumping.GameContent
{
    public class Block
    {
        public static Block[,] Blocks = new Block[1000, 1000];

        public const int BLOCKS_MAX = 16000;
        internal readonly int amount_current_blocks = 0;

        public bool Active { get; internal set; }
        public Color Color { get; set; }

        public bool HasCollision { get; set; }

        public int X { get; }
        public int Y { get; }

        public TileFraming FramingStyle { get; private set; }

        public int xWorld;
        public int yWorld;

        public Texture2D texture;

        public Vector2 Center => new(xWorld + CollisionBox.Width / 2, CollisionBox.Y + CollisionBox.Height / 2);
        public Vector2 Top => new(xWorld + (CollisionBox.Width / 2), yWorld);
        public Vector2 Bottom => new(xWorld + (CollisionBox.Width / 2), yWorld + CollisionBox.Height);
        public Vector2 Left => new(xWorld, yWorld + (CollisionBox.Height / 2));
        public Vector2 Right => new(xWorld + CollisionBox.Width, yWorld + (CollisionBox.Height / 2));
        public Rectangle CollisionBox => new(xWorld, yWorld, 16, 16);

        public class Methods
        {
            public static void PlaceBlock(int i, int j, Color color = default)
            {
                if (color == default)
                    color = Color.White;
                var block = Blocks[i, j];
                if (block.Active)
                    return;

                block.Active = true;
                block.Color = color;
            }
            public static void BreakBlock(int i, int j)
            {
                var block = Blocks[i, j];

                if (!block.Active)
                    return;

                block.Active = false;
                SoundPlayer.PlaySoundInstance(BJGame.Sounds.BlockBreak, 0.1f);
            }
        }

        internal Block(int x, int y, bool active, Color color, bool collidable)
        {
            X = x;
            Y = y;
            Color = color;
            Active = active;
            HasCollision = collidable;
            amount_current_blocks++;
            if (amount_current_blocks > BLOCKS_MAX)
                throw new Exception("Blocks amount was larger than " + nameof(BLOCKS_MAX) + $" ({BLOCKS_MAX})");
            Blocks[X, Y] = this;
        }
        internal void UpdateBlock()
        {
            xWorld = X * 16;
            yWorld = Y * 16;
            if (Active)
                FramingStyle = UpdateBlock_GetAutoFraming();
        }
        internal TileFraming UpdateBlock_GetAutoFraming()
        {
            bool shouldBeTopLeftFramed()
            {
                bool actualCond = Blocks[X + 1, Y].Active && Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X - 1, Y].Active && !Blocks[X, Y - 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeTopFramed()
            {
                bool actualCond = Blocks[X, Y + 1].Active && Blocks[X - 1, Y].Active && Blocks[X + 1, Y].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y - 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeTopRightFramed()
            {
                bool actualCond = Blocks[X - 1, Y].Active && Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X + 1, Y].Active && !Blocks[X, Y - 1].Active;


                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeLeftFramed()
            {
                bool actualCond = Blocks[X + 1, Y].Active && Blocks[X, Y - 1].Active && Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X - 1, Y].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeMiddleFramed()
            {
                bool actualCond = Blocks[X + 1, Y].Active && Blocks[X - 1, Y].Active && Blocks[X, Y + 1].Active && Blocks[X, Y - 1].Active;

                return actualCond;
            }
            bool shouldBeRightFramed()
            {
                bool actualCond = Blocks[X - 1, Y].Active && Blocks[X, Y - 1].Active && Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X + 1, Y].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomLeftFramed()
            {
                bool actualCond = Blocks[X + 1, Y].Active && Blocks[X, Y - 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X - 1, Y].Active && !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomFramed()
            {
                bool actualCond = Blocks[X, Y - 1].Active && Blocks[X - 1, Y].Active && Blocks[X + 1, Y].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomRightFramed()
            {
                bool actualCond = Blocks[X - 1, Y].Active && Blocks[X, Y - 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X + 1, Y].Active && !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeLeftRightFramed()
            {
                bool actualCond = Blocks[X - 1, Y].Active && Blocks[X + 1, Y].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y - 1].Active && !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeUpFramed()
            {
                bool actualCond = Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y - 1].Active && !Blocks[X + 1, Y].Active && !Blocks[X - 1, Y].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeFacingRightFramed()
            {
                bool actualCond = Blocks[X - 1, Y].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y - 1].Active && !Blocks[X + 1, Y].Active && !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeUpDownFramed()
            {
                bool actualCond = Blocks[X, Y - 1].Active && Blocks[X, Y + 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X - 1, Y].Active && !Blocks[X + 1, Y].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeFacingLeftFramed()
            {
                bool actualCond = Blocks[X + 1, Y].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y - 1].Active && !Blocks[X - 1, Y].Active && !Blocks[X, Y + 1].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeDownFramed()
            {
                bool actualCond = Blocks[X, Y - 1].Active;

                bool otherNeighborsInactive =
                    !Blocks[X, Y + 1].Active && !Blocks[X - 1, Y].Active && !Blocks[X + 1, Y].Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeNeutralFramed()
            {
                bool actualCond =
                    !Blocks[X, Y - 1].Active && !Blocks[X, Y + 1].Active && !Blocks[X - 1, Y].Active && !Blocks[X + 1, Y].Active;
                return actualCond;
            }

            if (shouldBeTopLeftFramed())
                return TileFraming.TopLeft;
            if (shouldBeTopFramed())
                return TileFraming.Top;
            if (shouldBeTopRightFramed())
                return TileFraming.TopRight;
            if (shouldBeLeftFramed())
                return TileFraming.Left;
            if (shouldBeMiddleFramed())
                return TileFraming.Middle;
            if (shouldBeRightFramed())
                return TileFraming.Right;
            if (shouldBeBottomLeftFramed())
                return TileFraming.BottomLeft;
            if (shouldBeBottomFramed())
                return TileFraming.Bottom;
            if (shouldBeBottomRightFramed())
                return TileFraming.BottomRight;
            if (shouldBeLeftRightFramed())
                return TileFraming.LeftRight;
            if (shouldBeUpFramed())
                return TileFraming.Up;
            if (shouldBeFacingRightFramed())
                return TileFraming.FacingRight;
            if (shouldBeUpDownFramed())
                return TileFraming.UpDown;
            if (shouldBeFacingLeftFramed())
                return TileFraming.FacingLeft;
            if (shouldBeDownFramed())
                return TileFraming.Down;
            if (shouldBeNeutralFramed())
                return TileFraming.Neutral;

            return TileFraming.Middle;
        }
        public void Draw()
        {
            var frame = new Rectangle();
            if (Active)
            {
                switch (FramingStyle)
                {
                    #region Square
                    case TileFraming.TopLeft:
                        frame = new Rectangle(0, 0, 16, 16);
                        break;
                    case TileFraming.Top:
                        frame = new Rectangle(18, 0, 16, 16);
                        break;
                    case TileFraming.TopRight:
                        frame = new Rectangle(36, 0, 16, 16);
                        break;

                    case TileFraming.Left:
                        frame = new Rectangle(0, 18, 16, 16);
                        break;
                    case TileFraming.Middle:
                        frame = new Rectangle(18, 18, 16, 16);
                        break;
                    case TileFraming.Right:
                        frame = new Rectangle(36, 18, 16, 16);
                        break;

                    case TileFraming.BottomLeft:
                        frame = new Rectangle(0, 36, 16, 16);
                        break;
                    case TileFraming.Bottom:
                        frame = new Rectangle(18, 36, 16, 16);
                        break;
                    case TileFraming.BottomRight:
                        frame = new Rectangle(36, 36, 16, 16);
                        break;
                    #endregion
                    case TileFraming.Neutral:
                        frame = new Rectangle(0, 54, 16, 16);
                        break;
                    case TileFraming.LeftRight:
                        frame = new Rectangle(18, 54, 16, 16);
                        break;

                    case TileFraming.Up:
                        frame = new Rectangle(18, 72, 16, 16);
                        break;

                    case TileFraming.FacingLeft:
                        frame = new Rectangle(0, 90, 16, 16);
                        break;
                    case TileFraming.UpDown:
                        frame = new Rectangle(18, 90, 16, 16);
                        break;
                    case TileFraming.FacingRight:
                        frame = new Rectangle(36, 90, 16, 16);
                        break;

                    case TileFraming.Down:
                        frame = new Rectangle(18, 108, 16, 16);
                        break;
                }
            }
            BJGame.spriteBatch.Draw(BJGame.Textures.BlockTexture, new Rectangle(xWorld, yWorld, 16, 16), frame, Color, 0f, Vector2.Zero, default, 0f);
                // BJGame.spriteBatch.DrawString(BJGame.Fonts.Amatic, "X", Right, Color.White, 0f, Vector2.Zero, 0.5f, default, default);
        }
        public override string ToString()
        {
            return $"X: {X} | Y: {Y} | xW: {xWorld} | yW: {yWorld} | HasCollision: {HasCollision} | color: {Color}";
        }
    }
}
