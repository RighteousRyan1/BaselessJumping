using BaselessJumping.Internals.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BaselessJumping.GameContent
{
    public sealed class Particle : Entity
    {
        public static Particle[] particles = new Particle[12000];

        public float scale;
        public float rotation;

        internal static Texture2D defTexture;
        public int id;
        public Color color;

        private static int current_notNull_particles;

        public bool active;

        private Particle()
        {
            current_notNull_particles++;
            id = current_notNull_particles;

            particles[id] = this;
        }

        public void Update()
        {
            position += velocity;

            scale -= 0.025f;

            if (scale < 0)
                active = false;
            if (!active)
            {
                particles[id] = null;
                current_notNull_particles--;
            }
        }
        public void Draw()
        {
            BJGame.spriteBatch.Draw(defTexture, position, null, color, rotation, defTexture.Size() / 2, scale, default, 0f); 
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