using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BaselessJumping;
using BaselessJumping.Internals.Core.Interfaces;
using System.Reflection;
using BaselessJumping.Internals.Common;

namespace BaselessJumping.Internals.Systems
{
    public class LoadableSystem
    {
        public static void Load()
        {
            foreach (var type in CodeHelper.GetSubclasses<ILoadable>())
            {
                type.Load();
            }
        }
        public static void Unload()
        {
            foreach (var type in CodeHelper.GetSubclasses<ILoadable>())
            {
                type.Unload();
            }
        }
    }
}