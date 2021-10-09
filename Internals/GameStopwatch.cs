using System;
using System.Collections.Generic;

namespace BaselessJumping.Internals
{
    /// <summary>
    /// A sub-class of a stopwatch that serves the purpose of updating every game tick.
    /// </summary>
    public class GameStopwatch
    {
        internal static List<GameStopwatch> totalTrackable = new();

        public int ElapsedGameTicks { get; internal set; }
        public bool IsRunning { get; private set; }

        public void Start()
            => IsRunning = true;
        public void Stop()
        {
            ElapsedGameTicks = 0;
            IsRunning = false;
        }
        public void Pause() =>
            IsRunning = false;

        public void Restart() =>
            ElapsedGameTicks = 0;

        /// <summary>
        /// Creates a new instance of the <see cref="GameStopwatch"/> class and begins the related <see cref="GameStopwatch"/>.
        /// </summary>
        public static GameStopwatch StartNew() 
        {
            var timer = new GameStopwatch();
            timer.Start();
            return timer;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GameStopwatch"/> class.
        /// </summary>
        public GameStopwatch()
        {
            totalTrackable.Add(this);
        }

        internal void IncreaseTimer()
            => ElapsedGameTicks++;

        // when you come back -> make countdown available for player regen cooldown
    }

    public class CountdownTimer : GameStopwatch
    {
        // ehh idk
    }
}