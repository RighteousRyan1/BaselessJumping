using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static Effect LightingShader { get; set; }

        internal static List<ValueTuple<Vector2, float, float, Color>> TrackableLights { get; } = new();

        public static void InitializeShader()
            => LightingShader = Resources.GetGameResource<Effect>("Lighting");

        public static void CreateLight(Vector2 position, float power, float distance, Color color)
        {
            LightingShader.Parameters["oCoordinates"].SetValue(position);
            LightingShader.Parameters["oLightPower"].SetValue(power);
            LightingShader.Parameters["oLightDistance"].SetValue(distance);
            LightingShader.Parameters["oLightColor"].SetValue(color.ToVector3());

            TrackableLights.Add((position, power, distance, color));
        }
    }
}
