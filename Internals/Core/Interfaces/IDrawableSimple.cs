namespace BaselessJumping.Internals.Core.Interfaces
{
    public interface IDrawableSimple
    {
        int DrawOrder { get; set; }
        /// <summary>
        /// Draw things. Do NOT begin a spritebatch.
        /// </summary>
        public void Draw();
    }
}