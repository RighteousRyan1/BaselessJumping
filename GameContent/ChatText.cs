using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BaselessJumping.GameContent
{
    public class ChatText
    {
        public int timeLeft;
        public Color color;
        public string text;
        public int whoAmI;
        public bool active;
        // I hate myself
        public static bool displayAllChatTexts;
        public static string curTypedText = string.Empty;
        public static List<ChatText> TotalTexts { get; } = new();
        private ChatText(string text, Color color, int timeLeft = 300)
        {
            this.text = text;
            this.color = color;
            this.timeLeft = timeLeft;
            active = true;
            whoAmI = TotalTexts.Count;
            TotalTexts.Add(this);
        }

        public static void NewText(object msg) => new ChatText(msg.ToString(), Color.White);
        public static void NewText(object msg, Color color) => new ChatText(msg.ToString(), color);
        public static void NewText(object msg, Color color, int timeLeft) => new ChatText(msg.ToString(), color, timeLeft);
        public static void NewText(object msg, int timeLeft) => new ChatText(msg.ToString(), Color.White, timeLeft);

        internal void Update()
        {
            timeLeft--;

            if (timeLeft <= 0)
                active = false;

            if (!displayAllChatTexts && whoAmI < TotalTexts.Count - 5)
                active = false;
        }
        internal static void DrawAllButtons()
        {
            int i = 0;
            foreach (var txt in TotalTexts)
            {
                if (txt.active)
                {
                    var orig = BJGame.Fonts.SilkPixel.MeasureString(txt.text);
                    var orig2 = new Vector2(0, orig.Y / 2);
                    BJGame.spriteBatch.DrawString(BJGame.Fonts.SilkPixel, txt.text, new Vector2(20, GameUtils.WindowHeight - 40 - (20 * i)), txt.color, 0f, orig2, 0.5f, default, 0f);
                    i++;
                }
            }
        }
        public override string ToString()
        {
            return $"Text: {text} | Color: {color} | whoAmI: {whoAmI}";
        }
    }
}