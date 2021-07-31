using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BaselessJumping.Internals.UI
{
    public abstract class UIElement
    {
        public static List<UIElement> AllUIElements { get; internal set; } = new();
        public UIParent Parent { get; set; }

        // I'm not entirely sure this is necessary, given that only if Visible is true in UIParent, Draw() is called.
        internal bool ShouldDraw => Parent.Visible;

        internal UIElement() { AllUIElements.Add(this); }

        public virtual void Draw() { }

        public virtual void Update() { }
    }
}