using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BaselessJumping.GameContent;
using BaselessJumping.GameContent.Shapes;
using BaselessJumping.Internals.Common;
using BaselessJumping.Internals.Common.GameInput;
using BaselessJumping.Internals.Loaders;
using BaselessJumping.Internals.UI;
using BaselessJumping.Localization;
using Microsoft.Xna.Framework;

namespace BaselessJumping.Internals
{
    internal class RPCHandler
    {
        private static RichPresence _rpc;

        private static DiscordRpcClient _client;
        public static void Load()
        {
            _rpc = new RichPresence() { Details = "Playing Baseless Jumping" };
            _rpc.Assets = new();
            _client = new DiscordRpcClient("896203207062216734");
            _rpc.Timestamps = new Timestamps()
            {
                Start = DateTime.UtcNow,
            };
            _client?.SetPresence(_rpc);
            _client.Initialize();
        }
        public static void Update()
        {
            _client?.Invoke();

            SetClientDetails("Playing Baseless Jumping");
            SetClientInfo(largeText: $"Players: {Player.AllPlayers.Count}", largeKey: "bruhcat");

            if (!_client.IsDisposed)
            {
                _client?.SetPresence(_rpc);
            }
        }

        public static void SetClientDetails(string details)
        {
            _rpc.Details = details;
        }
        public static void SetClientInfo(string largeText = default, string smallText = default, string largeKey = default, string smallKey = default)
        {
            if (largeText != default)
                _rpc.Assets.LargeImageText = largeText;
            if (smallText != default)
                _rpc.Assets.SmallImageText = smallText;
            if (largeKey != default)
                _rpc.Assets.LargeImageKey = largeKey;
            if (smallKey != default)
                _rpc.Assets.SmallImageKey = smallKey;
        }
        public static void Terminate()
        {
            if (_client.IsInitialized)
                _client?.UpdateEndTime(DateTime.UtcNow);
            _client?.Dispose();
        }
    }
}
