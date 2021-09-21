using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaselessJumping.GameContent.Behaviour;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.GameContent
{
    public class Item
    {
        // to be completed

        public const int TOTAL_ITEMS = 10;

        #region Metadata

        public int id;
        public int netId;
        public Player owner; // null if no player owns it
        public ISource spawnSource;

        #endregion

        #region Stats

        public double damage;
        public float scale;
        public string Description { get; set; }
        public string Name { get; set; }
        public Color Rarity { get; set; }
        public Rectangle hitbox;
        public Vector2 position; // (-1, -1) if owned by a player

        #endregion

        public Item() 
        {
            spawnSource = GameSources.Create();
        }
    }
}
