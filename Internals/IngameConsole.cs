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

        private static IngameConsoleConfig _commands = new();

        private static PropertyInfo[] ConsoleFields { get; } = _commands.GetType().GetProperties();

        public static void SubmitCommand(string cmd, object arg)
        {
            // get this to work soon
            for (int i = 0; i < ConsoleFields.Length; i++)
            {
                var command = ConsoleFields[i];
                if (cmd == command.Name)
                {
                    var cmmd = (IConsoleCommand)command.GetValue(_commands);

                    if (cmmd.RequiresCheats && !GameContent.GameManager.cheats)
                    {
                        Console.WriteLine($"Tried changing cvar {cmd} to {arg}, but Cheats_Enable is not True.");
                        return;
                    }
                    else
                    {
                        var oldVal = command.GetValue(_commands);
                        Console.WriteLine($"Set command '{cmd}' from '{oldVal}' to '{arg}'");

                        /*var args = cmmd.GetType().GetGenericArguments()[0];

                        var m = typeof(ConsoleCommand<>).MakeGenericType(args);

                        m.GetProperty("Value").SetValue(m, arg);*/
                        if (int.TryParse(arg.ToString(), out var val1))
                        {
                            var c = cmmd as IConsoleCommand<int>;
                            c.Value = val1;
                        }
                        else if (bool.TryParse(arg.ToString(), out var val2))
                        {
                            var c = cmmd as IConsoleCommand<bool>;
                            c.Value = val2;
                        }
                        else if (float.TryParse(arg.ToString(), out var val3))
                        {
                            var c = cmmd as IConsoleCommand<float>;
                            c.Value = val3;
                        }
                        else
                        {
                            Console.WriteLine($"'{arg}' does not match the return type of '{cmd}'");
                        }

                        return;
                    }
                }
                if (cmd == command.Name)
                {
                    /* #region Special Use Cases 1
                    if (((IConsoleCommand)field.GetValue(null)).RequiresCheats && _config.Cheats_Enabled != 1)
                    {
                        Console.WriteLine($"Command Submitted: '{fld}' | Cannot set value as cheats are not enabled!");
                        return;
                    }
                    #endregion
                    Console.WriteLine($"Command Submitted: '{fld}' | Value set from {(field?.GetValue(null) as ConsoleCommand).Value} to {arg}");
                    if (float.TryParse(arg.ToString(), out var newFloat))
                    {

                        if (fld == "cheats_enable" && newFloat == 0f)
                        {
                            foreach (var cmd in ConsoleFields)
                                if (((IConsoleCommand)cmd.GetValue(null)).RequiresCheats)
                                {
                                    ((IConsoleCommand)field.GetValue(null)).Value = 0f;
                                }
                        }
                        ((IConsoleCommand)field.GetValue(null)).Value = newFloat;
                    }
                    else
                    {
                        // GameContent.BaselessJumping.BaseLogger.Write($"Command Submitted: '{fld}' | Value found but entered value is not the same type as the field value!", Logger.LogType.Warn);
                        Console.WriteLine($"Command Submitted: '{fld}' | Value found but entered value is not the same type as the field value!");
                        return;
                    }*/
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
                foreach (var fld in ConsoleFields)
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
                                    var cmd = fld.GetValue(_commands) as IConsoleCommand;
                                    Console.WriteLine(cmd.Description);
                                }
                            }
                        }
                        if (vals.Length == 2)
                        {
                            Console.WriteLine(vals[0]);
                            SubmitCommand(vals[0], vals[1]);
                        }
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
    /*public class ConsoleCommand<T> : IConsoleCommand<T>
    {
        public T Value { get; set; }
        public string Description { get; }
        public bool RequiresCheats { get; }
        public bool Saveable { get; }

        public ConsoleCommand(T value, string description, bool requiresCheats = false, bool saveable = true)
        {
            Value = value;
            Description = description;
            RequiresCheats = requiresCheats;
            Saveable = saveable;
        }

        public static implicit operator string(ConsoleCommand<T> cmd) => cmd.Description;
    }
    public interface IConsoleCommand<T>
    {
        T Value { get; set; }
        string Description { get; }
        bool RequiresCheats { get; }
        bool Saveable { get; }
    }*/
    public interface IConsoleCommand
    {
        string Description { get; }
        bool RequiresCheats { get; }
        bool Saveable { get; }
    }

    public interface IConsoleCommand<T> : IConsoleCommand
    {
        T Value { get; set; }
    }

    public class ConsoleCommand : IConsoleCommand
    {
        public string Description { get; }
        public bool RequiresCheats { get; }
        public bool Saveable { get; }

        public ConsoleCommand(string description, bool requiresCheats = false, bool saveable = true)
        {
            Description = description;
            RequiresCheats = requiresCheats;
            Saveable = saveable;
        }
    }

    public class ConsoleCommand<T> : ConsoleCommand, IConsoleCommand<T>
    {
        public T Value { get; set; }

        public ConsoleCommand(ref T value, string description, bool requiresCheats = false, bool saveable = true)
            : base(description, requiresCheats, saveable)
        {
            Value = value;
        }

        public void ValueSet(T value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}