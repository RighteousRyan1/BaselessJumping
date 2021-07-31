using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BaselessJumping.Audio;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.UI;

namespace BaselessJumping.Internals.Common.GameUI
{
    /// <summary>
    /// Be sure to always call the ctor as an instance field!
    /// </summary>
    public class TextButton : UIElement
    {
        public static List<TextButton> AllTextButtons { get; internal set; } = new();

        public bool canBeClicked = true;
        public Vector2 Origin => font != null ? font.MeasureString(text) / 2 : Vector2.Zero;
        public bool IsHovered { get; private set; }
        public SoundEffect HoverSound { get; set; } = null;
        public SoundEffect ClickSound { get; set; } = null;
        public Vector2 drawPosition;
        public float scale;
        public Color color = Color.White;
        public Color borderColor;
        public string text = "N/A";
        public SpriteFont font = BJGame.Fonts.SilkPixel;
        public SpriteEffects SpriteFX { get; set; } = default;
        public Rectangle HoverBox { get; private set; }
        public bool JustClicked { get; private set; }
        public bool JustReleased { get; private set; }
        public Action<TextButton> OnClick = null;
        public bool border;
        public float maxScale = 1.2f;
        public float rotation;
        private bool _newHover;
        private bool _oldHover;
        public float scalingAmount = 0.0175f;
        /// <summary>
        /// In this ctor, soundClick and soundHovered can both be null. If null, no sound will play.
        /// </summary>
        /// <param name="text">The text of the button.</param>
        /// <param name="font">The font of the button.</param>
        /// <param name="color">The color of the button.</param>
        /// <param name="drawPos">The drawing position of the button.</param>
        /// <param name="soundClick">Defaults to null and will play no sound. The sound that plays when clicked.</param>
        /// <param name="soundHovered">Defaults to null and will play no sound. The sound that plays when hovered.</param>
        /// <param name="canBeClicked">Whether or not this button can be clicked.</param>
        /// <param name="canDraw">Whether or not this button should draw and do normal clicking actions and such.</param>
        public TextButton(string text, SpriteFont font,
            Color color, Vector2 drawPos, 
            Action<TextButton> onClick, UIParent parent, 
            float scalingAmount = 0.0175f, 
            bool giveBorder = false, 
            Color borderColor = default, 
            SoundEffect soundClick = null,
            SoundEffect soundHovered = null)
        {
            Parent = parent;
            this.text = text;
            this.font = font;
            this.color = color;
            drawPosition = drawPos;
            this.borderColor = borderColor;
            border = giveBorder;
            ClickSound = soundClick;
            HoverSound = soundHovered;
            this.scalingAmount = scalingAmount;

            OnClick = onClick;
            AllTextButtons.Add(this);
        }
        internal void UpdateButton()
        {
            Vector2 origin = font.MeasureString(text) / 2;

            HoverBox = new Rectangle((int)(drawPosition.X - origin.X * scale), 
                (int)(drawPosition.Y - origin.Y * scale),
                (int)(font.MeasureString(text).X * scale), 
                (int)(font.MeasureString(text).Y * scale));

            IsHovered = HoverBox.Contains(GameUtils.MousePosition.ToPoint());

            _newHover = IsHovered;

            if (GameUtils.WindowActive)
            {
                if (_newHover && !_oldHover && canBeClicked)
                {
                    if (HoverSound != null)
                        SoundPlayer.PlaySoundInstance(ClickSound, SoundEffect.MasterVolume);
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
        public override void Draw()
        {
            if (border)
                GameUtils.DrawTextWithBorder(font, text, drawPosition, color, borderColor, 0f, scale, 2);
            else
                BJGame.spriteBatch.DrawString(font, text, drawPosition, color, rotation, Origin, scale, SpriteFX, 0f);
        }
        public override string ToString()
        {
            return "{ IsHovered: " + IsHovered + " | Scale: " + scale + " | Name: " + text + " }";
        }
    }
}