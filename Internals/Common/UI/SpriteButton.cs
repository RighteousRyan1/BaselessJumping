using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BaselessJumping.Audio;

namespace BaselessJumping.Internals.Common.UI
{
    /// <summary>
    /// Be sure to always call the ctor as an instance field!
    /// </summary>
    public class SpriteButton
    {
        public static List<SpriteButton> AllSpriteButtons { get; internal set; } = new();
        public bool canBeClicked = true;
        public Vector2 Origin => texture != null ? texture.Size() / 2 : Vector2.Zero;
        public bool IsHovered { get; private set; }
        public Vector2 drawPosition;
        public SoundEffect HoverSound { get; set; } = null;
        public SoundEffect ClickSound { get; set; } = null;
        public Color color = Color.White;
        public bool ShouldDraw { get; set; } = true;
        public Texture2D texture = BJGame.Textures.WhitePixel;
        public SpriteEffects SpriteFX { get; set; } = default;
        public Rectangle HoverBox { get; private set; }
        public bool JustClicked { get; private set; }
        public bool JustReleased { get; private set; }
        public Action<SpriteButton> OnClick = null;
        public float maxScale = 1.2f;
        public float scale = 1f;
        public float rotation;
        private bool _newHover;
        private bool _oldHover;
        public float scalingAmount = 0.0175f;
        public SpriteButton(Texture2D texture,
            Color color, Vector2 drawPos, Action<SpriteButton> onClick, float scalingAmount = 0.0175f, SoundEffect soundClick = null, SoundEffect soundHovered = null, 
            bool canDraw = true)
        {
            this.texture = texture;
            this.color = color;
            drawPosition = drawPos;
            ClickSound = soundClick;
            HoverSound = soundHovered;
            ShouldDraw = canDraw;
            this.scalingAmount = scalingAmount;

            OnClick = onClick;
            AllSpriteButtons.Add(this);
        }
        internal void UpdateButton()
        {
            Vector2 origin = Origin;

            HoverBox = new Rectangle(
                (int)(drawPosition.X - origin.X * scale),
                (int)(drawPosition.Y - origin.Y * scale),
                (int)(texture.Size().X * scale),
                (int)(texture.Size().Y * scale));

            IsHovered = HoverBox.Contains(Utilities.MousePosition.ToPoint());

            _newHover = IsHovered;

            if (Utilities.WindowActive)
            {
                if (_newHover && !_oldHover && canBeClicked)
                {
                    if (HoverSound != null)
                        SoundPlayer.PlaySoundInstance(HoverSound, SoundEffect.MasterVolume);
                }

                if (Input.CanDetectClick() && IsHovered && canBeClicked)
                {
                    if (ClickSound != null)
                        SoundPlayer.PlaySoundInstance(ClickSound, SoundEffect.MasterVolume);
                    JustClicked = true;
                    OnClick?.Invoke(this);
                }
                else
                    JustClicked = false;
            }

            if (canBeClicked)
            {
                if (IsHovered)
                {
                    if (scale < maxScale)
                        scale += scalingAmount;
                }
                else
                {
                    if (scale > 1f)
                        scale -= scalingAmount;
                }
            }

            if (Input.CanDetectClickRelease() && IsHovered)
                JustReleased = true;
            else
                JustReleased = false;

            if (!ShouldDraw)
                scale = 1f;

            _oldHover = _newHover;
        }
        public void Draw(bool beginBatch = true)
        {
            if (beginBatch) BJGame.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            BJGame.spriteBatch.Draw(texture, drawPosition, null, color, rotation, Origin, scale, SpriteFX, 0f);
            if (beginBatch) BJGame.spriteBatch.End();
        }
        public override string ToString()
        {
            return "{ IsHovered: " + IsHovered + " | Scale: " + scale + " }";
        }
    }
}