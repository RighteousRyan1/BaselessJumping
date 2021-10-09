using Microsoft.Xna.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace BaselessJumping.GameContent
{
    public class Stage
    {
        public string Name { get; private set; } = string.Empty;

        public List<Block> BlockMap { get; private set; } = new();

        public static Stage currentLoadedStage;

        private static byte[] fileHeader = new byte[5] { 82, 89, 65, 78, 82 };

        public static readonly IList<string> ProhibitedNames = new ReadOnlyCollection<string>(new List<string> { "CouldNotLocateError", "InvalidHeaderError", "OldSaveSystemError", "LoadError", "CouldNotLocateError.stg", "InvalidHeaderError.stg", "OldSaveSystemError.stg", "LoadError.stg" });

        private const int SavingSystemVersion = 1; //INCREMENT THIS AFTER EVERY BREAKING CHANGE TO THE FILE SAVING SYSTEM

        public Stage(string name)
        {
            Name = name;
        }

        public static void SaveStage(Stage stage)
        {
            string root = Path.Combine(Base.ExePath, "Stages");
            string path = Path.Combine(root, $"{stage.Name}.stg");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            stage.BlockMap.Clear();

            foreach (var block in Block.Blocks)
            {
                if (block != null)
                {
                    stage.BlockMap.Add(block);
                }
            }

            using var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite));

            //File format
            // 1. File header (5 bytes) - spells out "RYANR" in ASCII
            // 2. Stage saving system version (int) - dictates the stage saving system's current version
            // 3. Stage count (int) - defines the length of the stage's tile map
            // 4. Tile X (int) - tile X position
            // 5. Tile Y (int) - tile Y position
            // 6. Tile active (bool) determines whether or not the tile is in the world and visible.
            // 7. Tile id (int) - tile's id value
            // 8, 9, 10. Tile color (byte, byte, byte) the tile's color.
            // 11. Tile collision (bool) the tile's ability for the player to collide into it

            writer.Write(fileHeader);
            writer.Write(SavingSystemVersion);
            writer.Write(stage.BlockMap.Count);
            foreach (var tl in stage.BlockMap)
            {
                writer.Write(tl.X);
                writer.Write(tl.Y);
                writer.Write(tl.Active);
                writer.Write(tl.id);

                writer.Write(tl.Color.R);
                writer.Write(tl.Color.G);
                writer.Write(tl.Color.B);

                writer.Write(tl.HasCollision);
            }
            if (File.Exists(path))
            {
                GameManager.BaseLogger.Write($"Overwrote \"{stage.Name}.stg\" in stages folder.", Internals.Logger.LogType.Info);
                return;
            }
            GameManager.BaseLogger.Write($"Saved stage file \"{stage.Name}.stg\" to stages folder.", Internals.Logger.LogType.Info);
        }

        public static Stage LoadStage(string fileName)
        {
            string root = Path.Combine(Base.ExePath, "Stages");
            string path = Path.Combine(root, $"{fileName}.stg");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            Stage returnStage = new(fileName);

            if (!File.Exists(path))
            {
                GameManager.BaseLogger.Write($"Could not find stage file \"{fileName}.stg\" in the stages folder, with the path {path}.", Internals.Logger.LogType.Error);
                return new Stage("CouldNotLocateError");
            }

            using (var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                try
                {
                    var header = reader.ReadBytes(5);
                    if (!header.SequenceEqual(fileHeader))
                    {
                        GameManager.BaseLogger.Write($"File \"{fileName}.stg\" is not a valid stage file (header invalid)", Internals.Logger.LogType.Error);
                        return new Stage("InvalidHeaderError");
                    }
                    int stageVersion = reader.ReadInt32();
                    if (stageVersion != SavingSystemVersion)
                    {
                        GameManager.BaseLogger.Write($"File \"{fileName}.stg\" was saved using an older version of the file saving system. Unable to load.", Internals.Logger.LogType.Error); //eventually implement conversion of old files to current version
                        return new Stage("OldSaveSystemError");
                    }
                    int tileAmount = reader.ReadInt32();
                    for (int i = 0; i < tileAmount; i++)
                    {
                        var x = reader.ReadInt32();
                        var y = reader.ReadInt32();
                        var active = reader.ReadBoolean();
                        var tileType = reader.ReadInt32();

                        var r = reader.ReadByte();
                        var g = reader.ReadByte();
                        var b = reader.ReadByte();

                        var col = reader.ReadBoolean();

                        returnStage.BlockMap.Add(new Block(x, y, active, new(r, g, b), col, tileType));
                    }
                }
                catch (Exception e)
                {
                    GameManager.BaseLogger.Write($"An error occurred while loading the stage file \"{fileName}.stg\":\n{e}", Internals.Logger.LogType.Error);
                    return new Stage("LoadError");
                }
            }
            GameManager.BaseLogger.Write($"Loaded stage file \"{fileName}.stg\".", Internals.Logger.LogType.Info);
            return returnStage;
        }

        public static void SetStage(Stage stage)
        {
            currentLoadedStage = stage;
            foreach (var bl in stage.BlockMap)
                Block.Blocks[bl.X, bl.Y] = bl;
            GameManager.BaseLogger.Write($"Set stage \"{stage.Name}\" as current.", Internals.Logger.LogType.Info);
        }
    }
}