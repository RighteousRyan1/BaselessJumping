using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaselessJumping.GameContent
{
    public sealed class Particle : Entity
    {
        public const int MAX_PARTICLES = 20000;
        public static Particle[] particles = new Particle[MAX_PARTICLES];

        public float scale;
        public float rotation;

        internal static Texture2D defTexture;
        public int id;
        public Color color;

        public bool active;

        public float decay = 0.025f;

        private Particle()
        {
            var indice = particles.FirstOrDefault(x => x is null);

            var index = Array.IndexOf(particles, indice);

            if (index == -1)
            {
                Console.WriteLine("Max particles reached... Cannot instantiate new particle.");
                return;
            }

            id = index;

            defTexture = Base.Instance.Content.GetResource<Texture2D>("Particle");

            active = true;
            particles[id] = this;
        }

        public void Update()
        {
            position += velocity;

            scale -= decay;

            if (scale < 0)
            {
                active = false;
                particles[id] = null;
            }
        }
        public void Draw()
        {
            Base.spriteBatch.Draw(defTexture, position, null, color, rotation, defTexture.Size() / 2, scale, default, 0f);
            // GameUtils.DrawStringQuick(id, position + new Vector2(0, -30));
        }

        public static Particle SpawnParticle(Vector2 position, Vector2 velocity, Color color, float scale, float rotation)
        {
            Particle particle = new();
            particle.rotation = rotation;
            particle.position = position;
            particle.velocity = velocity;
            particle.color = color;
            particle.scale = scale;

            return particles[particle.id];
        }
    }
}