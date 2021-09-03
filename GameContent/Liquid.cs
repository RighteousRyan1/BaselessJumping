using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using BaselessJumping.Internals.Common.Utilities;
using System.Collections.Generic;

namespace BaselessJumping.GameContent
{
    public class Liquid
    {
        public int cols;
        public int rows;

        public float flexibility = 1f;

        static VertexBuffer vertexBuffer;

        static BasicEffect basicEffect;
        static Matrix world = Matrix.CreateTranslation(0, 0, 0);
        static Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        static Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
        static VertexPositionColor[] vertices = new VertexPositionColor[3];
        public static void init()
        {
            basicEffect = new BasicEffect(BJGame.Instance.GraphicsDevice);
        }

        public static void TestDraw()
        {
            float sin = (float)Math.Sin(BaselessJumping.LastCapturedGameTime.TotalGameTime.TotalSeconds);
            vertices[0] = new VertexPositionColor(new Vector3(0, 1 + sin / 5, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(0.5f + sin / 5, 0, 0), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(-0.5f + sin / 5, 0, 0), Color.Blue);

            vertexBuffer = new VertexBuffer(BJGame.Instance.GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            BJGame.Instance.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new();
            rasterizerState.CullMode = CullMode.None;
            BJGame.Instance.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                BJGame.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }
    }
}