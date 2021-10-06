using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.GameContent.Mechanics
{
    public struct HealthBar
    {
        public Entity Entity { get; }

        public double maxLife;

        public double currentLife;

        public HealthBar(double maxLifeStored, Entity ent)
        {
            maxLife = maxLifeStored;

            currentLife = maxLife;
            Entity = ent;
        }

        public void DeductLife(double d)
        {
            currentLife -= d;
        }

        public void HealLife(double d)
        {
            currentLife += d;
        }
    }
}