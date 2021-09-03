using BaselessJumping.Internals.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TextCopy;

namespace BaselessJumping.Internals
{
    public class IngameConsole
    {
        // public static TextInputEXT inputter;
        public static string curWritten = "";
        public static bool Enabled { get; set; }
        // clientside too

        public static SettableKeyValuePair<string, int> server_cheats = new("server_cheats", 0);
        public static SettableKeyValuePair<string, int> draw_backgrounds = new("draw_backgrounds", 1);
        public static SettableKeyValuePair<string, float> phys_playerfriction = new("phys_playerfriction", 1f);
        public static class CommandValues
        {
            public static bool DrawBackgrounds { get; internal set; }
        }
        public static void SubmitCommand(string command, object arg)
        {
            // get this to work soon
            /*foreach (var cmdList in all)
            {
                for (int i = 0; i < cmdList.Length; i++)
                {
                    var cmd = cmdList[i];
                    if (command == cmd.Key)
                    {
                        Console.WriteLine($"Command Submitted: '{command}' | Value set from {cmd.Value} to {arg}");
                        cmd.Value = arg;
                        return;
                    }
                }
            }
            Console.WriteLine("Unknown command submitted.");*/
        }
        public static void Close()
        {
            Enabled = false;
            TextInputEXT.StopTextInput();
        }
        public static void Open()
        {
            Rectangle textRectangle = new(20, 20, 100, 100);
            Enabled = true;
            TextInputEXT.StartTextInput();
            TextInputEXT.SetInputRectangle(new(20, 120, 100, 100));
            BJGame.Instance.GraphicsDevice.ScissorRectangle = textRectangle;
        }
        internal static void Init() =>
            TextInputEXT.TextInput += OnTextInput;
        private static void OnTextInput(char obj)
        {
            // the character
            if (Enabled)
            {
                Console.WriteLine($"{obj}: {obj.GetHashCode()}");
                switch (obj.GetHashCode())
                {
                    case 851981:
                        var vals = curWritten.Split(" ");
                        if (vals.Length < 2 || vals[1] == string.Empty)
                        {
                            Console.WriteLine($"Command incomplete. Only {vals.Length} args written.");
                        }
                        else
                            SubmitCommand(vals[0], vals[1]);
                        curWritten = string.Empty;
                        return;
                    case 524296:
                        if (curWritten.Length > 0)
                        {
                            curWritten = curWritten.Remove(curWritten.Length - 1);
                        }
                        return;
                    case 1441814:
                        var txt = ClipboardService.GetText();
                        curWritten += txt;
                        return;
                    default:
                        curWritten += obj;
                        return;
                }
            }
        }
    }
}