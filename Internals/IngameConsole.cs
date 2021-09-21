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
    public class IngameConsole
    {
        public static List<string> MatchedStrings { get; private set; } = new();

        public static Rectangle WritingBox { get; } = new(20, 20, 100, 100);
        public static string CurrentlyWrittenText { get; set; } = "";
        public static bool Enabled { get; set; }
        // clientside too

        #region Cheats
        public static ConsoleCommand cheats_enable = new(0f, "Allows cheats on the server to be used.");
        public static ConsoleCommand cheats_playerjumpheight = new(1f, "Changes the jump height of each player to {x} multiplier.");
        #endregion
        #region Rendering
        public static ConsoleCommand draw_bg = new(1f, "Enable the drawing of backgrounds.");
        #endregion
        #region GamePhysics
        public static ConsoleCommand phys_playerfriction = new(1f, "Change the friction of every player in the server.");
        public static ConsoleCommand phys_boosterpadpushscale = new(1f, "Change the force applied by a booster pad by a multiplier of {x}.");
        #endregion

        internal static FieldInfo[] ConsoleFields { get; } = typeof(IngameConsole).GetFields().Where(fld => fld.FieldType == typeof(ConsoleCommand)).ToArray();
        private static FieldInfo[] AllFields { get; } = typeof(IngameConsole).GetFields();

        public static void SubmitCommand(string fld, object arg)
        {
            // get this to work soon
            foreach (var field in ConsoleFields)
            {
                if (fld == field.Name)
                {
                    if (field.Name.Contains("cheats") && field.Name != "cheats_enable" && cheats_enable != 1)
                    {
                        Console.WriteLine($"Command Submitted: '{fld}' | Cannot set value as cheats are not enabled!");
                        return;
                    }
                    // GameContent.BaselessJumping.BaseLogger.Write($"Command Submitted: '{fld}' | Value set from {field?.GetValue(null)} to {arg}", Logger.LogType.Info);
                    Console.WriteLine($"Command Submitted: '{fld}' | Value set from {(field?.GetValue(null) as ConsoleCommand).Value} to {arg}");
                    if (float.TryParse(arg.ToString(), out var newFloat))
                    {
                        ((ConsoleCommand)field.GetValue(null)).Value = newFloat;
                    }
                    else
                    {
                        // GameContent.BaselessJumping.BaseLogger.Write($"Command Submitted: '{fld}' | Value found but entered value is not the same type as the field value!", Logger.LogType.Warn);
                        Console.WriteLine($"Command Submitted: '{fld}' | Value found but entered value is not the same type as the field value!");
                        return;
                    }
                    return;

                }
            }
            Console.WriteLine("Unknown command submitted.");
        }
        public static void Close()
        {
            Enabled = false;
            TextInputEXT.StopTextInput();
        }
        public static void Open()
        {
            Enabled = true;
            TextInputEXT.StartTextInput();
            TextInputEXT.SetInputRectangle(WritingBox);
            // BJGame.Instance.GraphicsDevice.ScissorRectangle = WritingBox;
        }
        internal static void Init()
        {
            TextInputEXT.TextInput += OnTextInput;
        }
        private static void OnTextInput(char obj)
        {
            // the character
            MatchedStrings.Clear();
            if (Enabled)
            {
                foreach (var fld in AllFields)
                {
                    List<string> names = new();
                    names.Add(fld.Name);
                    foreach (var match in StringComparator.FindMatches(CurrentlyWrittenText, names.ToArray()))
                    {
                        MatchedStrings.Add(match);
                    }
                }
                switch (obj.GetHashCode())
                {
                    case 851981:
                        var vals = CurrentlyWrittenText.Split(" ");
                        if (vals.Length == 1)
                        {
                            foreach (var fld in ConsoleFields)
                            {
                                if (fld.Name == vals[0])
                                {
                                    Console.WriteLine(((ConsoleCommand)fld.GetValue(null)).Description);
                                }
                            }
                        }
                        if (vals.Length == 2)
                            SubmitCommand(vals[0], vals[1]);
                        CurrentlyWrittenText = string.Empty;
                        break;
                    case 524296:
                        if (CurrentlyWrittenText.Length > 0)
                        {
                            CurrentlyWrittenText = CurrentlyWrittenText.Remove(CurrentlyWrittenText.Length - 1);
                        }
                        break;
                    case 1441814:
                        var txt = ClipboardService.GetText();
                        CurrentlyWrittenText += txt;
                        break;
                    default:
                        CurrentlyWrittenText += obj;
                        break;
                }
            }
        }
    }
    public class ConsoleCommand
    {
        public float Value { get; set; }
        public string Description { get; }
        public ConsoleCommand(float value, string description)
        {
            Value = value;
            Description = description;
        }

        public static implicit operator float(ConsoleCommand cmd) => cmd.Value;
        public static implicit operator string(ConsoleCommand cmd) => cmd.Description;
    }
}