using BaselessJumping.Audio;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaselessJumping.GameContent
{
    public class Block
    {
        public static Block[,] Blocks = new Block[1000, 1000];

        public int id;

        private int _oldId;

        public const int MAX_BLOCKS = 16000;
        internal readonly int amount_current_blocks = 0;

        public bool Active { get; internal set; }
        public Color Color { get; set; }

        public bool HasCollision { get; set; }

        public int X { get; }
        public int Y { get; }

        public TileFraming FramingStyle { get; private set; }

        public int xWorld;
        public int yWorld;

        private bool JustCreated
        {
            get
            {
                int curId = id;

                var getChange = curId > 0 && _oldId == 0;

                _oldId = curId;

                return getChange;
            }
        }

        public Texture2D texture = Resources.GetResourceBJ<Texture2D>("GrassBlock");

        public Vector2 Center => new(xWorld + CollisionBox.Width / 2, CollisionBox.Y + CollisionBox.Height / 2);
        public Vector2 Top => new(xWorld + (CollisionBox.Width / 2), yWorld);
        public Vector2 Bottom => new(xWorld + (CollisionBox.Width / 2), yWorld + CollisionBox.Height);
        public Vector2 Left => new(xWorld, yWorld + (CollisionBox.Height / 2));
        public Vector2 Right => new(xWorld + CollisionBox.Width, yWorld + (CollisionBox.Height / 2));

        public Rectangle CollisionBox => new(xWorld, yWorld, 16, 16);

        public class Methods
        {
            public static void PlaceBlock(int i, int j, int type, Color color = default)
            {
                if (color == default)
                    color = Color.White;
                var block = Blocks[i, j];
                if (block.Active)
                    return;
                block.id = type;
                block.Active = true;
                block.Color = color;
            }
            public static void BreakBlock(int i, int j)
            {
                var block = Blocks[i, j];

                if (!block.Active)
                    return;

                block.Active = false;

                SoundPlayer.PlaySoundInstance(Resources.GetResourceBJ<SoundEffect>("BlockBreak"), 0.1f);
            }
            public static Block GetValidBlock(int i, int j)
            {
                try
                {
                    if (Blocks[i, j] == null)
                    {
                        return new(0, 0, false, default, false);
                    }
                }
                catch
                {
                    return new(0, 0, false, default, false);
                }
                return Blocks[i, j];
            }
            public static Block GetActiveBlock(int i, int j)
            {
                var block = GetValidBlock(i, j);
                if (!block.Active)
                {
                    return new(0, 0, false, default, false);
                }
                return block;
            }
        }

        internal Block(int x, int y, bool active, Color color, bool collidable, int type = 0)
        {
            id = type;
            X = x;
            Y = y;
            Color = color;
            Active = active;
            HasCollision = collidable;
            amount_current_blocks++;
            if (amount_current_blocks > MAX_BLOCKS)
                throw new Exception("Blocks amount was larger than " + nameof(MAX_BLOCKS) + $" ({MAX_BLOCKS})");
            Blocks[X, Y] = this;
        }

        internal void Update()
        {
            Update_PerType();
            xWorld = X * 16;
            yWorld = Y * 16;

            FramingStyle = UpdateBlock_GetAutoFraming();
        }
        private void Update_PerType()
        {
            if (!Active)
                id = 0;
            if (JustCreated)
            {
                switch (id)
                {
                    case 1:
                        texture = Resources.GetResourceBJ<Texture2D>("GrassBlock");
                        break;
                }
            }
        }
        private TileFraming UpdateBlock_GetAutoFraming()
        {
            // who wants some spaghetti?
            // check framing of surrounding blocks to determine corner-tiles
            if (!Active)
                return TileFraming.Middle;

            var left = Methods.GetValidBlock(X - 1, Y);
            var right = Methods.GetValidBlock(X + 1, Y);
            var up = Methods.GetValidBlock(X, Y - 1);
            var down = Methods.GetValidBlock(X, Y + 1);

            #region Sub-Cardinal Framing

            bool getTopLeftCornerFrame()
            {
                bool checkRight = right.Active; //right.FramingStyle == TileFraming.Middle || right.FramingStyle == TileFraming.Right || right.FramingStyle == TileFraming.TopRightCorner;
                bool checkLeft = left.FramingStyle == TileFraming.Top || left.FramingStyle == TileFraming.TopLeft;
                bool checkUp = up.FramingStyle == TileFraming.TopLeft || up.FramingStyle == TileFraming.Left;
                bool checkDown = down.Active;
                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getTopRightCornerFrame()
            {
                bool checkRight = right.FramingStyle == TileFraming.Top || right.FramingStyle == TileFraming.TopRight;
                bool checkLeft = left.Active; //left.FramingStyle == TileFraming.Middle || left.FramingStyle == TileFraming.Left || left.FramingStyle == TileFraming.TopLeftCorner;
                bool checkUp = up.FramingStyle == TileFraming.TopRight || up.FramingStyle == TileFraming.Right;
                bool checkDown = down.Active;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getBottomLeftCornerFrame()
            {
                bool checkRight = right.Active; // right.FramingStyle == TileFraming.Middle || right.FramingStyle == TileFraming.Right || right.FramingStyle == TileFraming.BottomRightCorner;
                bool checkLeft = left.FramingStyle == TileFraming.Bottom || left.FramingStyle == TileFraming.BottomLeft;
                bool checkUp = up.Active;
                bool checkDown = down.FramingStyle == TileFraming.BottomLeft || down.FramingStyle == TileFraming.Left;
                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getBottomRightCornerFrame()
            {
                bool checkRight = right.FramingStyle == TileFraming.Bottom || right.FramingStyle == TileFraming.BottomRight;
                bool checkLeft = left.Active;//left.FramingStyle == TileFraming.Middle || left.FramingStyle == TileFraming.Left || left.FramingStyle == TileFraming.BottomLeftCorner;
                bool checkUp = up.Active;
                bool checkDown = down.FramingStyle == TileFraming.BottomRight || down.FramingStyle == TileFraming.Right;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getRightUpDownFrame()
            {
                bool checkRight = right.FramingStyle == TileFraming.FacingRight;
                bool checkLeft = left.Active;
                bool checkUp = up.Active; // up.FramingStyle == TileFraming.TopRight;
                bool checkDown = down.Active; // down.FramingStyle == TileFraming.BottomRight;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getLeftUpDownFrame()
            {
                bool checkLeft = left.FramingStyle == TileFraming.FacingLeft;
                bool checkRight = right.Active;
                bool checkUp = up.Active; //up.FramingStyle == TileFraming.TopLeft;
                bool checkDown = down.Active; //down.FramingStyle == TileFraming.BottomLeft;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getUpLeftRightFrame()
            {
                bool checkLeft = left.Active;//left.FramingStyle == TileFraming.TopLeft;
                bool checkDown = down.Active;
                bool checkUp = up.FramingStyle == TileFraming.Up;
                bool checkRight = right.Active; //right.FramingStyle == TileFraming.TopRight;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            bool getDownLeftRightFrame()
            {
                bool checkLeft = left.Active;//left.FramingStyle == TileFraming.BottomLeft;
                bool checkDown = down.FramingStyle == TileFraming.Down;
                bool checkUp = up.Active;
                bool checkRight = right.Active;//right.FramingStyle == TileFraming.BottomRight;

                return checkDown && checkUp && checkRight && checkLeft;
            }
            #endregion
            #region Cardinal Framing
            bool shouldBeTopLeftFramed()
            {
                bool actualCond = right.Active && down.Active;

                bool otherNeighborsInactive =
                    !left.Active && !up.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeTopFramed()
            {
                bool actualCond = down.Active && left.Active && right.Active;

                bool otherNeighborsInactive =
                    !up.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeTopRightFramed()
            {
                bool actualCond = left.Active && down.Active;

                bool otherNeighborsInactive =
                    !right.Active && !up.Active;


                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeLeftFramed()
            {
                bool actualCond = right.Active && up.Active && down.Active;

                bool otherNeighborsInactive =
                    !left.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeMiddleFramed()
            {
                bool actualCond = right.Active && left.Active && down.Active && up.Active;

                return actualCond;
            }
            bool shouldBeRightFramed()
            {
                bool actualCond = left.Active && up.Active && down.Active;

                bool otherNeighborsInactive =
                    !right.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomLeftFramed()
            {
                bool actualCond = right.Active && up.Active;

                bool otherNeighborsInactive =
                    !left.Active && !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomFramed()
            {
                bool actualCond = up.Active && left.Active && right.Active;

                bool otherNeighborsInactive =
                    !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeBottomRightFramed()
            {
                bool actualCond = left.Active && up.Active;

                bool otherNeighborsInactive =
                    !right.Active && !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeLeftRightFramed()
            {
                bool actualCond = left.Active && right.Active;

                bool otherNeighborsInactive =
                    !up.Active && !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeUpFramed()
            {
                bool actualCond = down.Active;

                bool otherNeighborsInactive =
                    !up.Active && !right.Active && !left.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeFacingRightFramed()
            {
                bool actualCond = left.Active;

                bool otherNeighborsInactive =
                    !up.Active && !right.Active && !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeUpDownFramed()
            {
                bool actualCond = up.Active && down.Active;

                bool otherNeighborsInactive =
                    !left.Active && !right.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeFacingLeftFramed()
            {
                bool actualCond = right.Active;

                bool otherNeighborsInactive =
                    !up.Active && !left.Active && !down.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeDownFramed()
            {
                bool actualCond = up.Active;

                bool otherNeighborsInactive =
                    !down.Active && !left.Active && !right.Active;
                return actualCond && otherNeighborsInactive;
            }
            bool shouldBeNeutralFramed()
            {
                bool actualCond =
                    !up.Active && !down.Active && !left.Active && !right.Active;
                return actualCond;
            }
            #endregion

            if (getTopLeftCornerFrame())
                return TileFraming.TopLeftCorner;
            if (getTopRightCornerFrame())
                return TileFraming.TopRightCorner;
            if (getBottomLeftCornerFrame())
                return TileFraming.BottomLeftCorner;
            if (getBottomRightCornerFrame())
                return TileFraming.BottomRightCorner;
            if (getRightUpDownFrame())
                return TileFraming.RightUpDown;
            if (getLeftUpDownFrame())
                return TileFraming.LeftUpDown;
            if (getUpLeftRightFrame())
                return TileFraming.UpLeftRight;
            if (getDownLeftRightFrame())
                return TileFraming.DownLeftRight;

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
                    #region Other...
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

                    #endregion
                    case TileFraming.TopLeftCorner:
                        frame = new(54, 36, 16, 16);
                        break;
                    case TileFraming.TopRightCorner:
                        frame = new(54, 54, 16, 16);
                        break;
                    case TileFraming.BottomLeftCorner:
                        frame = new(54, 0, 16, 16);
                        break;
                    case TileFraming.BottomRightCorner:
                        frame = new(54, 18, 16, 16);
                        break;

                    case TileFraming.RightUpDown:
                        frame = new(72, 0, 16, 16);
                        break;
                    case TileFraming.LeftUpDown:
                        frame = new(72, 36, 16, 16);
                        break;
                    case TileFraming.UpLeftRight:
                        frame = new(72, 54, 16, 16);
                        break;
                    case TileFraming.DownLeftRight:
                        frame = new(72, 18, 16, 16);
                        break;
                }
            }
            BJGame.spriteBatch.Draw(texture, new Rectangle(xWorld, yWorld, 16, 16), frame, Color, 0f, Vector2.Zero, default, 0f);
                // BJGame.spriteBatch.DrawString(BJGame.Fonts.Amatic, "X", Right, Color.White, 0f, Vector2.Zero, 0.5f, default, default);
        }
        public override string ToString()
        {
            return $"X: {X} | Y: {Y} | xW: {xWorld} | yW: {yWorld} | HasCollision: {HasCollision} | color: {Color}";
        }
    }
}
