namespace BaselessJumping.Internals.Core.Interfaces
{
    public interface ILoadable
    {
        /// <summary>
        /// This is called when everything is loaded into the game.
        /// </summary>
        public void Load();
        /// <summary>
        /// This iscalled once everything is Unloaded from the game, or quitting the game.
        /// </summary>
        public void Unload();
    }
}