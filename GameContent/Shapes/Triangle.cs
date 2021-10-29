using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using BaselessJumping.Internals.Common.Utilities;
using System.Collections.Generic;
using BaselessJumping.Internals.Core.Exceptions;

namespace BaselessJumping.GameContent.Shapes
{
    public class Triangle : IDisposable
    {
        public static List<Triangle> triangles = new();

        public VertexBuffer VertexBuffer { get; private set; }

        public BasicEffect Effect { get; }
        public Matrix World { get; }
        public Matrix View { get; }
        public Matrix Projection { get; }
        public static readonly VertexPositionColor[] vertColors = new VertexPositionColor[3];

        public Vector2[] vertices = new Vector2[vertColors.Length];

        public Color color;

        /// <summary>
        /// Write these as Screen Coordinates!
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        public Triangle(Vector2 pos1, Vector2 pos2, Vector2 pos3, Color color)
        {
            World = Matrix.CreateTranslation(0, 0, 0);
            View = Matrix.Identity;
            Projection = Matrix.CreateOrthographicOffCenter(0, GameUtils.WindowWidth, GameUtils.WindowHeight, 0, -1, 1);
            Effect = new(Base.Instance.GraphicsDevice);
            this.color = color;
            vertices[0] = pos1;
            vertices[1] = pos2;
            vertices[2] = pos3;
            VertexBuffer = new(Base.Instance.GraphicsDevice, typeof(VertexPositionColor), vertColors.Length, BufferUsage.WriteOnly);
            triangles.Add(this);
        }

        public void DrawImmediate()
        {
            vertColors[0] = new VertexPositionColor(new Vector3(vertices[0], 0), color);
            vertColors[1] = new VertexPositionColor(new Vector3(vertices[1], 0), color);
            vertColors[2] = new VertexPositionColor(new Vector3(vertices[2], 0), color);
            VertexBuffer.SetData(vertColors);

            Effect.World = World;
            Effect.View = View;
            Effect.Projection = Projection;
            Effect.VertexColorEnabled = true;

            Base.Instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);

            RasterizerState rasterizerState = new();
            rasterizerState.CullMode = CullMode.None;
            Base.Instance.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Base.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 1);
            }
        }

        public static void DrawVertexHierarchies()
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];
                int j = 0;
                foreach (var pos in triangle.vertices)
                {
                    Base.spriteBatch.DrawString(Base.Fonts.SilkPixel, j.ToString(), pos, Color.White, 0f, Base.Fonts.SilkPixel.MeasureString(j.ToString()) / 2, 0.5f, default, 0f);
                    j++;
                }
            }
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            Effect.Dispose();

            triangles.Remove(this);
        }

        public float FindSideLength(int side)
        {
            if (!CheckValidSide(this, side))
                throw new TriangleRelatedException("The side you tried to find was invalid.", side);
            return side < 2 ? Vector2.Distance(vertices[side], vertices[side + 1]) : Vector2.Distance(vertices[2], vertices[0]);
        }

        public float SideAdditionPostulate()
        {
            /*var dist_0_1 = Vector2.Distance(verticePositions[0], verticePositions[1]);
            var dist_1_2 = Vector2.Distance(verticePositions[1], verticePositions[2]);
            var dist_2_0 = Vector2.Distance(verticePositions[2], verticePositions[0]);*/

            var dist_0_1 = FindSideLength(0);
            var dist_1_2 = FindSideLength(1);
            var dist_2_0 = FindSideLength(2);

            return dist_0_1 + dist_1_2 + dist_2_0;
        }

        public Vector2 FindMidpointOfSide(int side)
        {
            if (!CheckValidSide(this, side))
                throw new TriangleRelatedException("The side you tried to find was invalid.", side);
            return side < 2 ? (vertices[side] + vertices[side + 1]) : (vertices[2] + vertices[0]) / 2;
        }

        /// <summary>
        /// Determines whether or not this <see cref="Triangle"/> is a Right Triangle.
        /// <para>This is not finished yet.</para>
        /// </summary>
        /// <returns></returns>
        public bool IsRight()
        {
            /*float longestSide = 0;

            float side1 = 0;
            float side2 = 0;

            for (int i = 0; i < 3; i++)
            {
                if (FindSideLength(i) > longestSide)
                    longestSide = FindSideLength(i);
                else
                {
                    side1 = FindSideLength(i);

                }
            }*/
            return false;

            //return dist_0_1 + dist_1_2 == dist_2_0; // a simple check using the pathagorean theorem
        }

        public bool IsIcoceles()
        {
            var dist_0_1 = FindSideLength(0);
            var dist_1_2 = FindSideLength(1);
            var dist_2_0 = FindSideLength(2);

            return (dist_0_1 == dist_1_2) || (dist_1_2 == dist_2_0);
        }

        public bool IsScalene() 
            => !IsRight() && !IsIcoceles();

        private static bool CheckValidSide(Triangle triangle, int side)
            => side < triangle.vertices.Length && side >= 0;
    }
}