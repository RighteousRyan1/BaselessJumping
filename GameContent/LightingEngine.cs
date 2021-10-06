using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaselessJumping.GameContent
{
    /// <summary>
    /// This is incomplete in every way.
    /// </summary>
    public class LightingEngine
    {
        private static Effect[] shaders = new Effect[200];

        public static void InitializeShader()
        {
            // GameUtils.PopulateArray(ref shaders, Resources.GetGameResource<Effect>("Lighting"));
        }

        public static void CreateLight(Vector2 position, float power, float distance, Color color)
        {
            int index = 0;
            foreach (var i in shaders)
                if (i != null)
                    index++;

            Console.WriteLine($"Lighting shader initialized at {position} with a power of {power}.\nLighting index {index}");

            if (index >= shaders.Length)
                return;

            shaders[index] = Resources.GetGameResource<Effect>("Lighting");

            shaders[index].Parameters["oCoordinates"].SetValue(position);
            shaders[index].Parameters["oLightPower"].SetValue(power);
            shaders[index].Parameters["oLightDistance"].SetValue(distance);
            shaders[index].Parameters["oLightColor"].SetValue(color.ToVector3());

        }
    }
}
