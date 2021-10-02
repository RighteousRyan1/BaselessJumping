using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaselessJumping.GameContent.Behaviour;
using BaselessJumping.GameContent.Physics;
using BaselessJumping.Internals.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.GameContent
{
    public class Item : Entity
    {
        // to be completed

        internal static List<Item> items = new();

        public const int TOTAL_ITEMS = 50;

        #region Metadata

        public int id;
        public int netId;
        public Player owner; // null if no player owns it
        public ISource spawnSource;

        #endregion

        #region Stats

        public int width;
        public int height;
        public double damage;
        public float scale;
        public float rotation;
        public string Description { get; set; }
        public string Name { get; set; }
        public Color Rarity { get; set; }
        public Rectangle hitbox;
        // position is (-1, -1) if owned by a player

        #endregion

        private Item(Texture2D texture) 
        {
            id = items.Count;

            width = texture.Width;
            height = texture.Height;
            StaticFieldStorage.ItemTexture[id] = texture;
            spawnSource = GameSources.Create();

            items.Add(this);
        }

        public void Draw()
        {
            OnPreDraw?.Invoke(this, new());

            BJGame.spriteBatch.Draw(StaticFieldStorage.ItemTexture[id], position, null, Color.White, rotation, StaticFieldStorage.ItemTexture[id].Size() / 2, scale, default, 0f);

            DrawUtils.DrawDebugBox(hitbox);

            OnPostDraw?.Invoke(this, new());
        }

        public void Update()
        {
            velocity *= 0.9f;

            hitbox = new((int)position.X - width / 2, (int)position.Y - height / 2, width, height);

            UpdateCollision();

            position += velocity;
        }

        public void UpdateCollision()
        {
        }

        public static Item CreateNew(Texture2D texture, Vector2 position)
        {
            var item = new Item(texture);

            item.position = position;
            item.scale = 1f;
            return item;
        }

        /// <summary>
        /// <c>SpriteBatch.Begin</c> is not called.
        /// </summary>
        public event EventHandler OnPreDraw;
        /// <summary>
        /// <c>SpriteBatch.End</c> is not called yet.
        /// </summary>
        public event EventHandler OnPostDraw;
    }
}
