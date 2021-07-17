using System;

namespace BaselessJumping.Internals.Core.Interfaces
{
    public interface ILoadable
    {
        /// <summary>
        /// This is called when everything is loaded into the engine.
        /// </summary>
        public void Load();
        /// <summary>
        /// This iscalled once everything is Unloaded from the engine, or quitting the engine.
        /// </summary>
        public void Unload();
        /// <summary>
        /// Called whenever the user calls it. Calls Unload() then Load() Manually.
        /// </summary>
        public void Reload();
    }
}