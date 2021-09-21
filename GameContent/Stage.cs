using Microsoft.Xna.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaselessJumping.GameContent
{
    public class Stage
    {
        // TODO: Finish IO
        public Stage(string name) => Name = name;
        public string Name { get; set; }

        public Block[,] BlockArray { get; private set; }

        private static Stage curLoadedStage;

        public static void SaveStage(Stage stage)
        {
            string root = Path.Combine(BJGame.ExePath, "Stages");
            string path = Path.Combine(root, $"{stage.Name}.stg");
            Directory.CreateDirectory(root);
            stage.BlockArray = Block.Blocks;

            using var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite));

            writer.Write(stage.BlockArray.Length);
            foreach (var bl in stage.BlockArray)
            {
                if (bl == null)
                    break;
                writer.Write(bl.Active);
                writer.Write(bl.X);
                writer.Write(bl.Y);
            }
        }
        public static void LoadStage(Stage stage)
        {
            string root = Path.Combine(BJGame.ExePath, "Stages");
            string path = Path.Combine(root, $"{stage.Name}.stg");
            Directory.CreateDirectory(root);
            using var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
            int count = reader.ReadInt32();

            stage.BlockArray = new Block[count, count];
            for (int i = 0; i < count; i++)
            {
                var block = Block.Blocks[reader.ReadInt32(), reader.ReadInt32()];

                block.Active = reader.ReadBoolean();
            }
            curLoadedStage = stage;
        }
    }
}