using BaselessJumping.Internals.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaselessJumping.GameContent
{
    public class AIEnemy : Entity, IUpdateableSimple, IDrawableSimple
    {
        // used for determining enemy actions
        public Action<AIEnemy> AI;

        public Texture2D texture;

        public Rectangle frame;

        public int DrawOrder { get; set; } = 1;

        public void Update()
        {
        }
        public void Draw()
        {
        }

        internal static void UpdateAll()
        {

        }
    }
}