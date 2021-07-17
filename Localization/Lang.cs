using System;

namespace BaselessJumping.Localization
{
    public class Lang
    {
        public static string GetRandomGameTitle()
        {
            int random = new Random().Next(0, 22);

            return random switch
            {
                0 => "Game of the Year award 1999",
                1 => "Between a Block and a Hard Place",
                2 => "Ryan is mad... And has two eyes!",
                3 => "Platformer Gone Wrong",
                4 => "Includes a little bit of death!",
                5 => "Better than Unity's Particle System!",
                6 => "Karlsson? Never heard of him.",
                7 => "There was once a man named....",
                8 => "Openly accepting sprite art!",
                9 => "Where there's a lack of milk, there's a lack of bone",
                10 => "The long awaited.... sequel?",
                11 => "Including a Godly Codebase!",
                12 => "Not your average platformer",
                13 => "Stealing and Dealing Nuts for ONLY $2.99",
                14 => "We do a marginal amount of messing around",
                15 => "Oi bruv, what meticulous movement is the canine performing?",
                16 => "Once a man, now a child",
                17 => "Not in any way or form similar to Terraria!",
                18 => "Also try Minecraft",
                19 => "Also try Terraria!",
                20 => "Projectiles are a man's best friend",
                _ => "Colonel... I'm trying to sneak around...",
            };
        }
    }
}