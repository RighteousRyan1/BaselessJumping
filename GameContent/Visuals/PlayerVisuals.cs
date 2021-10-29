using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.Enums;
using BaselessJumping.Internals.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Systems;
using BaselessJumping.Internals;
using BaselessJumping.GameContent.Props;
using BaselessJumping.Internals.Audio;
using BaselessJumping.Internals.Common.Utilities;
using BaselessJumping.Internals.UI;
using BaselessJumping.MapGeneration;
using BaselessJumping.GameContent.Shapes;

namespace BaselessJumping.GameContent.Visuals
{
    public sealed class PlayerVisuals
    {
        public Trail Trail { get; private set; }

        public Player AttachedPlayer { get; }

        public PlayerVisuals(Player attacher)
        {
            AttachedPlayer = attacher;
        }

        public void UpdateTrail()
        {
        }
    }
}