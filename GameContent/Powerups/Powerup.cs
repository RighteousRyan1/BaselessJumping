using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaselessJumping.GameContent.Powerups
{
    public class Powerup
    {
        internal List<Powerup> powerups = new();

        public int tickDuration;

        public bool Active { get; set; }

        public bool Bad { get; }

        public Action<Powerup> ActiveActionOnPlayer { get; set; }

        public int powerupId;

        public Powerup(int duration, bool bad, Action<Powerup> doWhileActive)
        {
            tickDuration = duration;
            Bad = bad;
            ActiveActionOnPlayer = doWhileActive;

            powerupId = powerups.Count;

            powerups.Add(this);
        }

        public void Apply(Player player, int duration)
        {
            var asList = player.Powerups.ToList();
            var i = asList.FirstOrDefault(i => i == null);

            var index = asList.IndexOf(i);

            if (index == -1)
            {
                var x = asList.FirstOrDefault(i => i.powerupId == powerupId);

                var ind = asList.IndexOf(x);

                player.Powerups[ind].tickDuration = duration;

                player.Powerups[ind].Active = true;
            }
            player.Powerups[index].Active = true;
        }

        internal void Update()
        {
            if (tickDuration > 0)
                tickDuration--;
            else
                Active = false;

            if (Active)
                ActiveActionOnPlayer?.Invoke(this);
        }
    }
}
