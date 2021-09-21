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
        public static ISource Create(string overrideSource = null)
        {
            var src = new Source_Generic();
            if (overrideSource != null)
            {
                src.Reason = overrideSource;
            }
            return src;
        }
        public static IUnsafeSource<T> Create<T>(string overrideSource = null)
        {
            var src = new Source_Custom<T>();
            if (overrideSource != null)
            {
                src.Reason = overrideSource;
            }
            return src;
        }
    }
    internal class Source_FromPlayer : ISource<Player> { public string Reason { get; set; } = "Spawned from a player"; }
    internal class Source_FromEnemy : ISource<AIEnemy> { public string Reason { get; set; } = "Spawned from an enemy (AI)"; }
    internal class Source_FromSomeEntity : ISource<Entity> { public string Reason { get; set; } = "Spawned from any sort of entity"; }
    internal class Source_Generic : ISource { public string Reason { get; set; } = "Generically spawned"; }
    internal class Source_Custom<TOwner> : IUnsafeSource<TOwner> { public string Reason { get; set; } = "Generic unsafe source"; }
}