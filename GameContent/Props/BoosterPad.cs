using BaselessJumping.GameContent;
using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals;

namespace BaselessJumping.GameContent.Props
{
    public sealed class BoosterPad : Entity
    {
        public static List<BoosterPad> BoosterPads { get; private set; } = new();
        private BoosterPad() => BoosterPads.Add(this);

        public enum BoosterPadDirection
        {
            North,
            East,
            South,
            West,
            NorthEast,
            SouthEast,
            SouthWest,
            NorthWest
        }

        public BoosterPadDirection BoosterDirection { get; set; }
        public float pushScale = 0f;

        private Texture2D bubbleTexture = Resources.GetGameResource<Texture2D>("Bubble");
        private Texture2D arrowTexture = Resources.GetGameResource<Texture2D>("Arrow");

        public Color BubbleColor { get; set; }
        public Color ArrowColor { get; set; }

        public float radians;

        public static BoosterPad Create(BoosterPadDirection direction, Vector2 position, float pushScale, Color bubbleColor, Color arrowColor)
        {
            BoosterPad pad = new();
            pad.BoosterDirection = direction;
            pad.pushScale = pushScale;
            pad.BubbleColor = bubbleColor;
            pad.ArrowColor = arrowColor;
            pad.position = position;

            return pad;
        }

        internal void Update()
        {
            int width = bubbleTexture.Width;
            int height = bubbleTexture.Height;
            Hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);
            Update_ApplyVelocityToPlayers();

            oldPosition = position;
        }

        private void Update_ApplyVelocityToPlayers()
        {
            radians = 0f;
            switch (BoosterDirection)
            {
                case BoosterPadDirection.North:
                    radians = 0f;
                    break;
                case BoosterPadDirection.East:
                    radians = MathHelper.PiOver2;
                    break;
                case BoosterPadDirection.South:
                    radians = MathHelper.Pi;
                    break;
                case BoosterPadDirection.West:
                    radians = MathHelper.Pi * 1.5f;
                    break;
                case BoosterPadDirection.NorthEast:
                    radians = MathHelper.PiOver4;
                    break;
                case BoosterPadDirection.SouthEast:
                    radians = MathHelper.Pi - MathHelper.PiOver4;
                    break;
                case BoosterPadDirection.SouthWest:
                    radians = MathHelper.Pi + MathHelper.PiOver4;
                    break;
                case BoosterPadDirection.NorthWest:
                    radians = MathHelper.TwoPi - MathHelper.PiOver4;
                    break;
            }
            foreach (var player in Player.AllPlayers)
            {
                if (player.Hitbox.Intersects(Hitbox))
                {
                    player.velocity += new Vector2(0, -pushScale * IngameConsole.phys_boosterpadpushscale).RotatedByRadians(radians);
                }
            }
        }
        internal void Draw()
        {
            var sb = BJGame.spriteBatch;


            sb.Draw(bubbleTexture, position, null, BubbleColor, 0f, bubbleTexture.Size() / 2, 1f, default, 0f);
            sb.Draw(arrowTexture, position, null, ArrowColor, radians, arrowTexture.Size() / 2, 1f, default, 0f);
        }
    }
}