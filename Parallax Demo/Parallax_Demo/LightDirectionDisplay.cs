using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Parallax_Demo
{
    class LightDirectionDisplay
    {
        VertexPositionColor[] vertices;
        BasicEffect effect;

        float ztilt, xtilt;
        Vector3 direction;
        public Vector3 Direction { get { return -direction; } set { direction = -value; } }

        private void SetVertexPositions()
        {
            float length = 0.8f, s = 0.02f;
            Matrix xform = Matrix.CreateRotationX(ztilt) * Matrix.CreateRotationY(xtilt);
            direction = Vector3.Transform(new Vector3(0.4f, 0.6f, 1.2f), xform);
            vertices[0].Position = Vector3.Transform(new Vector3(0f, 0f, length), xform); vertices[0].Color = Color.Red;
            vertices[1].Position = Vector3.Transform(new Vector3(s, 0, length), xform); vertices[1].Color = Color.Red;
            vertices[2].Position = Vector3.Transform(new Vector3(0, 0, length), xform); vertices[2].Color = Color.Green;
            vertices[3].Position = Vector3.Transform(new Vector3(0, s, length), xform); vertices[3].Color = Color.Green;
            vertices[4].Position = Vector3.Transform(new Vector3(0, 0, length), xform); vertices[4].Color = Color.Blue;
            vertices[5].Position = new Vector3(0, 0, 0); vertices[5].Color = Color.Blue;
        }

        public LightDirectionDisplay(GraphicsDeviceManager graphics)
        {
            vertices = new VertexPositionColor[6];
            ztilt = xtilt = 0;
            SetVertexPositions();
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

        public void Update(KeyboardState kbst)
        {
            float speed = 0.01f;
            if (kbst.IsKeyDown(Keys.J)) xtilt -= speed;
            if (kbst.IsKeyDown(Keys.L)) xtilt += speed;
            if (kbst.IsKeyDown(Keys.I)) ztilt -= speed;
            if (kbst.IsKeyDown(Keys.K)) ztilt += speed;
            SetVertexPositions();
        }

        public void Draw(GraphicsDeviceManager graphics, Matrix v, Matrix p)
        {
            effect.World = Matrix.Identity;
            effect.View = v;
            effect.Projection = p;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length / 2);
            }
        }
    }
}
