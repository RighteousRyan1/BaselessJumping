
using BaselessJumping.GameContent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaselessJumping.MapGeneration
{
    public class GenPattern
    {
        /// <summary>The amount of blocks that this GenPattern will move in the X axis per-iteration.</summary>
        public int speedX;
        /// <summary>The amount of blocks that this GenPattern will move in the Y axis per-iteration.</summary>
        public int speedY;
        /// <summary></summary>
        public int width;
        /// <summary></summary>
        public int height;
        /// <summary></summary>
        public int x;
        /// <summary></summary>
        public int y;
        /// <summary>The amount of times this GenPattern recursively calls.</summary>
        public int steps;

        /// <summary>Generate a new GenPattern.</summary>
        public GenPattern(int x, int y, int steps, int speedX, int speedY, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.steps = steps;
            this.speedX = speedX;
            this.speedY = speedY;
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// Generates this GenPattern.
        /// </summary>
        /// <param name="patternManipulator">Call an action each time this method is iterated. <para> </para>For instance, for each step this GenPattern has, call this parameter's contents.</param>
        public async Task Generate(int type = 1, Action<GenPattern> patternManipulator = null, Action<Block> blockManipulator = null)
        {
            // this could be insanely optimised
            int attempts = 0;
            do
            {
                await Task.Delay(50);
                attempts++;
                int addStepX = speedX * attempts;
                int addStepY = speedY * attempts;
                for (int i = x - width / 2 + addStepX; i < x + width / 2 + addStepX; i++)
                {
                    for (int j = y - height / 2 + addStepY; j < y + height / 2 + addStepY; j++)
                    {
                        if (j < 0)
                            break;
                        if (i < 0)
                            break;
                        var block = Block.Methods.GetValidBlock(i, j, out var valid);
                        if (valid)
                        {
                            block.Active = true;
                            block.id = type;
                            patternManipulator?.Invoke(this);
                            blockManipulator?.Invoke(block);
                        }
                    }
                }
            }
            while (attempts < steps);
        }
        public override string ToString()
        {
            return $"Coords: ({x}, {y}) | Speed: ({speedX}, {speedY}) | Steps: {steps} | Dims: ({width}, {height})";
        }
    }
}