using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using BaselessJumping.Audio;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.UI;

namespace BaselessJumping.Internals.Common.GameUI
{
    /// <summary>
    /// Be sure to always call the ctor as an instance field!
    /// </summary>
    public class SpriteButton : UIElement
    {
        public static List<SpriteButton> AllSpriteButtons { get; internal set; } = new();
        public bool canBeClicked = true;
        public Vector2 Origin => texture != null ? texture.Size() / 2 : Vector2.Zero;
        public bool IsHovered { get; private set; }
        public SoundEffect HoverSound { get; set; } = null;
        public SoundEffect ClickSound { get; set; } = null;
        public Color color = Color.White;
        public Vector2 drawPosition;
        public float scale;
        public Texture2D texture = Resources.GetResourceBJ<Texture2D>("WhitePixel");
        public SpriteEffects SpriteFX { get; set; } = default;
        public Rectangle HoverBox { get; private set; }
        public bool JustClicked { get; private set; }
        public bool JustReleased { get; private set; }
        public Action<SpriteButton> OnClick = null;
        public float maxScale = 1.2f;
        public float rotation;
        private bool _newHover;
        private bool _oldHover;
        public float scalingAmount = 0.0175f;
        public SpriteButton(Texture2D texture,
            Color color, Vector2 drawPos, Action<SpriteButton> onClick, UIParent parent, 
            float scalingAmount = 0.0175f, SoundEffect soundClick = null, SoundEffect soundHovered = null)
        {
            Parent = parent;
            this.texture = texture;
            this.color = color;
            drawPosition = drawPos;
            ClickSound = soundClick;
            HoverSound = soundHovered;
            this.scalingAmount = scalingAmount;

            OnClick = onClick;
            AllSpriteButtons.Add(this);
        }
        public override void Update()
        {
            Vector2 origin = Origin;

            HoverBox = new Rectangle(
                (int)(drawPosition.X - origin.X * scale),
                (int)(drawPosition.Y - origin.Y * scale),
                (int)(texture.Size().X * scale),
                (int)(texture.Size().Y * scale));

            IsHovered = HoverBox.Contains(GameUtils.MousePosition.ToPoint());

            _newHover = IsHovered;

            if (GameUtils.WindowActive)
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
        public override void Draw()
        {
            BJGame.spriteBatch.Draw(texture, drawPosition, null, color, rotation, Origin, scale, SpriteFX, 0f);
        }
        public override string ToString()
        {
            return "{ IsHovered: " + IsHovered + " | Scale: " + scale + " }";
        }
    }
}