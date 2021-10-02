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
using BaselessJumping.Internals.Common.GameUI;
using BaselessJumping.MapGeneration;

namespace BaselessJumping.GameContent.Visuals
{
    public sealed class PlayerVisuals
    {
        public Triangle Trail { get; private set; }

        public Player AttachedPlayer { get; }

        public PlayerVisuals(Player attacher)
        {
            AttachedPlayer = attacher;
        }

        public void UpdateTrail()
        {
            Trail.verticePositions[0] = AttachedPlayer.position;//PlayerOne.oldPositions[10].RotatedByRadians(PlayerOne.velocity.ToRotation());
            Trail.verticePositions[1] = AttachedPlayer.position; // + diff.RotatedByRadians(-MathHelper.Pi / 2);
            Trail.verticePositions[2] = AttachedPlayer.position; // + diff.RotatedByRadians(MathHelper.Pi / 2) * 10;
        }
    }
}