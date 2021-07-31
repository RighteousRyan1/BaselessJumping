using System.Collections.Generic;

namespace BaselessJumping.Internals.UI
{
    public class UIParent
    {
        public static List<UIParent> TotalParents { get; private set; } = new();
        public List<UIElement> Elements { get; private set; } = new();

        public bool Visible { get; set; } = true;

        public void AppendElement(UIElement element)
        {
            TotalParents.Add(this);
            Elements.Add(element);
        }

        internal void UpdateElements()
        {
            if (Visible)
            {
                foreach (var elem in UIElement.AllUIElements)
                    elem.Update();
            }
        }
        internal void DrawElements()
        {
            if (Visible)
            {
                foreach (var elem in UIElement.AllUIElements)
                    elem.Draw();
            }
        }
    }
}