using Microsoft.Xna.Framework;
using System;

namespace BaselessJumping.GameContent.Behaviour
{
    public class GameSources
    {
        public static ISource<Player> GetPlayerSource()
        {
            return new Source_FromPlayer();
        }
        public static ISource<AIEnemy> GetEnemySource()
        {
            return new Source_FromEnemy();
        }
        public static ISource<Entity> GetAnyEntitySource(string overrideSource = null)
        {
            var src = new Source_FromSomeEntity();
            if (overrideSource != null)
            {
                src.Reason = overrideSource;
            }
            return src;
        }
    }
    internal class Source_FromPlayer : ISource<Player> { public string Reason { get; set; } = "Shot from a player"; }
    internal class Source_FromEnemy : ISource<AIEnemy> { public string Reason { get; set; } = "Shot from an enemy (AI)"; }
    internal class Source_FromSomeEntity : ISource<Entity> { public string Reason { get; set; } = "Shot from any sort of entity"; }
}