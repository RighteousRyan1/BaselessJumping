using System.Collections.Generic;

namespace BaselessJumping.Internals.UI
{
    public class UIParent
    {
        public static List<UIParent> TotalParents { get; private set; } = new();

        public List<UIElement> Elements { get; private set; } = new();

        public bool Visible { get; set; } = false;

        public UIParent() => TotalParents.Add(this);

        public void AppendElements(params UIElement[] elements)
        {
            foreach (var elem in elements)
            {
                //TotalParents.Add(this);
                Elements.Add(elem);
                elem.Parent = this;
            }
        }
        public void AppendElement(UIElement element)
        {
            //TotalParents.Add(this);
            Elements.Add(element);
            element.Parent = this;
        }

        public void RemoveElement(UIElement element)
        {
            Elements.Remove(element);
            //if (Elements.Count <= 0)
                //TotalParents.Remove(this);
            element.Parent = null;
        }

        internal void DrawElements()
        {
            if (Visible)
            {
                foreach (var elem in Elements)
                    elem.Draw();
            }
        }
    }
}