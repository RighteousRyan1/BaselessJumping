using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaselessJumping.GameContent;
using BaselessJumping.GameContent.Behaviour;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping
{
    public static class GameAssets
    {
        public static Texture2D[] ItemTexture = new Texture2D[Item.TOTAL_ITEMS];
        public static Texture2D[] ProjectileTexture = new Texture2D[0]; // replace with Projectile.TOTAL_PROJECTILES when Projectile.cs is made ;)
    }
}