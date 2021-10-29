using BaselessJumping.GameContent;
using BaselessJumping.GameContent.Props;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TextCopy;

namespace BaselessJumping.Internals
{
    public class IngameConsoleConfig
    {
        #region Cheats
        public ConsoleCommand<bool> Cheats_Enabled { get; } = new(ref GameManager.cheats, "Allows cheats on the server to be used.", false, false);
        public ConsoleCommand<float> Cheats_PlayerJumpHeight { get; } = new(ref Player.jumpHeight, "Changes the jump height of each player to {x} multiplier.", true, false);
        public ConsoleCommand<float> Cheats_PlayerMoveSpeed { get; } = new(ref Player.moveSpeed, "Changes the speed of each player to {x} multiplier.", true, false);
        public ConsoleCommand<bool> Cheats_NoClip { get; } = new(ref Player.noclip, "Enables NoClip.", true, false);

        public ConsoleCommand<bool> Cheats_PlayerImmortal { get; } = new(ref Player.immortal, "Makes the player immortal.", true, false);
        #endregion
        #region Rendering
        public ConsoleCommand<bool> Drawing_Backgrounds { get; } = new(ref GameManager.SettingsService.drawBackgrounds, "Enable the drawing of backgrounds.");
        #endregion
        #region GamePhysics
        public ConsoleCommand<float> Physics_PlayerFriction { get; } = new(ref Player.friction, "Change the friction of every player in the server.", false, false);
        public ConsoleCommand<float> Physics_BoosterPadPushScale { get; } = new(ref BoosterPad.pushScaleExtra, "Change the force applied by a booster pad by a multiplier of {x}.", false, false);
        #endregion
        #region Behavior
        public ConsoleCommand<float> Behavior_ItemGrabRange { get; } = new(ref Item.grabRange, "Modify the grab range for all items to players.", false, false);
        #endregion
    }
}