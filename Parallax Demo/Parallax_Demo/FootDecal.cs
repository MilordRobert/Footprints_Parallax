using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parallax_Demo
{
    class FootDecal
    {
        private static int[] reverseIndices = new int[] { 1, 2, 0, 1, 3, 2 };
        VertexTangentSpace[] vertices;
        int[] indices;
        Vector3 localPosition;
        Color[] pixels;
        Quaternion rotation;
        float rot_value;
        float time;
        RenderTarget2D texture;

        public FootDecal(Vector3 position, Ground ground, float rotation, Texture2D normalMap, Footprint_Game game)
        {

            time = 0;
            texture = new RenderTarget2D(game.GraphicsDevice, 512, 512, true, SurfaceFormat.Color, DepthFormat.None);
            pixels = new Color[normalMap.Width * normalMap.Height];
            localPosition = new Vector3(position.X, 0, position.Z);
            vertices = new VertexTangentSpace[4];
            indices = new int[] {1, 0, 2, 1, 2, 3 };

            vertices[0].Normal = new Vector3(0, +1, 0); vertices[0].Tangent = new Vector3(+1, 0, 0); vertices[0].Bitangent = new Vector3(0, 0, 1);
            vertices[1].Normal = new Vector3(0, +1, 0); vertices[1].Tangent = new Vector3(+1, 0, 0); vertices[1].Bitangent = new Vector3(0, 0, 1);
            vertices[2].Normal = new Vector3(0, +1, 0); vertices[2].Tangent = new Vector3(+1, 0, 0); vertices[2].Bitangent = new Vector3(0, 0, 1);
            vertices[3].Normal = new Vector3(0, +1, 0); vertices[3].Tangent = new Vector3(+1, 0, 0); vertices[3].Bitangent = new Vector3(0, 0, 1);

            vertices[00].TextureCoordinate = new Vector2(0, 0);
            vertices[01].TextureCoordinate = new Vector2(1, 0);
            vertices[02].TextureCoordinate = new Vector2(0, 1);
            vertices[03].TextureCoordinate = new Vector2(1, 1);

            rot_value = rotation;
            this.rotation = Quaternion.CreateFromAxisAngle(new Vector3(0,1,0), rotation);
            this.rotation.Normalize();

            vertices[00].Position = new Vector3(-2f, 0, 2f);
            vertices[01].Position = new Vector3(2f, 0, 2f);
            vertices[02].Position = new Vector3(-2f, 0, -2f);
            vertices[03].Position = new Vector3(2f, 0, -2f);


            for(int i = 0; i < vertices.Length; i++)
            {
                float x_Diff = Vector3.Transform(vertices[i].Position, World).X - ground.Vertices[0].Position.X;
                float z_Diff = Vector3.Transform(vertices[i].Position, World).Z - ground.Vertices[0].Position.Z;
                float x_Ratio = x_Diff / (ground.Vertices[2].Position.X - ground.Vertices[0].Position.X);
                float z_Ratio = z_Diff / (ground.Vertices[1].Position.Z - ground.Vertices[0].Position.Z);

                vertices[i].WorldTextureCoord = new Vector2(ground.Vertices[2].TextureCoordinate.X * x_Ratio, ground.Vertices[1].TextureCoordinate.Y * z_Ratio);
            }
        }

        public int[] Indices
        {
            get { return indices; }
        }

        public VertexTangentSpace[] Vertices
        {
            get { return vertices; }
        }

        public float Length_X
        {
            get { return vertices[1].Position.X - vertices[0].Position.X; }
        }

        public float Length_Z
        {
            get { return vertices[0].Position.Z - vertices[2].Position.Z; }
        }

        public System.Drawing.RectangleF Bounds
        {
            get { return new System.Drawing.RectangleF(vertices[0].Position.X, vertices[0].Position.Z, Length_X, Length_Z); }
        }

        public Vector3 Position
        {
            get { return localPosition; }
            set { localPosition = value; }
        }

        public float X
        {
            get { return localPosition.X; }
            set { localPosition.X = value; }
        }

        public float Z
        {
            get { return localPosition.Z; }
            set { localPosition.Z = value; }
        }


        public Vector3 LeftCorner
        {
            get { return Vertices[0].Position; }
        }

        public Vector3 World_Position
        {
            get { return Vector3.Transform(localPosition, World); }
        }

        public Color[] Pixels
        {
            get { return pixels; }
        }

        public Matrix World
        {
            get { return Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(localPosition); }
        }

        public float RotationY
        {
            get { return rotation.Y; }
        }


        public float Rotation
        {
            get { return rot_value; }
            set { rot_value = value; rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rot_value);
                rotation.Normalize();
            }
        }

        public int[] ReverseIndices
        {
            get { return reverseIndices; }
        }

        public Texture2D prepareTexture(Footprint_Game game)
        {
            
            game.GraphicsDevice.SetRenderTarget(texture);
            game.GraphicsDevice.Clear(Color.Transparent);

            game.runningEffect.Parameters["Time"].SetValue(time);

            foreach (EffectPass pass in game.runningEffect.CurrentTechnique.Passes)
            //foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
                     PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, ReverseIndices, 0, Indices.Length / 3);
            }
            game.GraphicsDevice.SetRenderTarget(null);
            return (Texture2D)texture;
        }

        public float Time
        {
            get { return time; }
        }

        public void Age()
        {
            //if (time < 0.25)
                time += 0.001f;
            
        }
    }

    
}
