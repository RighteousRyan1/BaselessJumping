using Microsoft.Xna.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaselessJumping.GameContent
{
    public class Stage
    {
        public Stage(string name) => Name = name;
        public string Name { get; set; }

        public List<Block> BlockArray { get; private set; } = new();

        private static Stage curLoadedStage;

        public static void SaveStage(Stage stage)
        {
            string root = Path.Combine(BJGame.ExePath, "Stages");
            string path = Path.Combine(root, $"{stage.Name}.stg");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            stage.BlockArray.Clear();

            foreach (var block in Block.Blocks)
            {
                if (block != null)
                {
                    stage.BlockArray.Add(block);
                }
            }

            using var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite));

            foreach (var bl in stage.BlockArray)
            {
                writer.Write(bl.Active);
                writer.Write(bl.X);
                writer.Write(bl.Y);
            }
        }
        public static void LoadStage(Stage stage)
        {
            string root = Path.Combine(BJGame.ExePath, "Stages");
            string path = Path.Combine(root, $"{stage.Name}.stg");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            using (var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                foreach (var bl in stage.BlockArray)
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    var b = Block.Blocks[x, y];
                    b.Active = reader.ReadBoolean();
                }
            }
            curLoadedStage = stage;
        }
    }
}