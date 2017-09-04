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
        protected float size;
        bool running = false;
        bool right = true;
        float weight;
        FootDecal overlap = null;

        public FootDecal(Vector3 position, float size, Ground ground, float rotation, Texture2D normalMap, Footprint_Game game, bool running, bool right, float weight)
        {
            this.weight = weight;
            this.right = right;
            this.size = size;
            this.running = running;
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
            vertices[00].Position = new Vector3(-size, 0, size);
            vertices[01].Position = new Vector3(size, 0, size);
            vertices[02].Position = new Vector3(-size, 0, -size);
            vertices[03].Position = new Vector3(size, 0, -size);

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

        public float Weight
        {
            get { return weight; }
        }

        public VertexTangentSpace[] Vertices
        {
            get { return vertices; }
        }

        public float Length_X
        {
            get { return vertices[1].Position.X - vertices[0].Position.X; }
        }

        public FootDecal Overlap
        {
            set { overlap = value; }
        }

        public float Length_Z
        {
            get { return vertices[0].Position.Z - vertices[2].Position.Z; }
        }

        public System.Drawing.RectangleF Bounds
        {
            get {
                Vector3 x = Vector3.Transform(vertices[0].Position, World);
                return new System.Drawing.RectangleF(x.X, x.Z, Length_X, Length_Z); }
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

        public bool RightFoot
        {
            get { return right; }
        }

        /// <summary>
        /// The rotation value around the Y axis.
        /// The get method simply returns the float value of that rotation
        /// The set method constructs a quaternion that will rotate by the given float value
        /// </summary>
        public float Rotation
        {
            get { return rot_value; }
            set { rot_value = value; rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rot_value);
                rotation.Normalize();
            }
        }

        /// <summary>
        /// The indices in reverse order.
        /// This is mostly used when rendering a temporary image that will later be rendered to the screen using the proper indices
        /// </summary>
        public int[] ReverseIndices
        {
            get { return reverseIndices; }
        }

        /// <summary>
        /// Creates the texture what will be used to render the footprint with a parallax map.
        /// This may include a map for running, erosion and overlapping
        /// </summary>
        /// <param name="game">The base game, used to set parameters for shaders</param>
        /// <returns></returns>
        public virtual Texture2D prepareTexture(Footprint_Game game)
        {
            //if(running)
            //{
            game.GraphicsDevice.SetRenderTarget(texture);
            game.GraphicsDevice.Clear(Color.Transparent);
            game.runningEffect.Parameters["Time"].SetValue(time);
            game.runningEffect.Parameters["Running"].SetValue(running);
            game.runningEffect.Parameters["size"].SetValue(1/size);

            foreach (EffectPass pass in game.runningEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
                        PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, ReverseIndices, 0, Indices.Length / 3);
            }
            game.GraphicsDevice.SetRenderTarget(null);

            //if(overlap != null)
            //{
            //    Matrix m = World * Matrix.CreateTranslation(0, 0, -0.3f);
            //    game.collision.Parameters["World"].SetValue(Matrix.Identity);
            //    game.collision.Parameters["SecondWorld"].SetValue(World * Matrix.Invert(m));
            //    game.collision.Parameters["SecondWorldInverse"].SetValue(World * Matrix.Invert(m));
            //    //collision.Parameters["SecondWorld"].SetValue(footprint.World * Matrix.Invert(overlap.World));
            //    //collision.Parameters["SecondWorldInverse"].SetValue(Matrix.Invert(footprint.World * Matrix.Invert(overlap.World)));
            //    game.collision.Parameters["TopLeft"].SetValue(overlap.Vertices[0].TextureCoordinate);
            //    game.collision.Parameters["TopRight"].SetValue(overlap.Vertices[1].TextureCoordinate);
            //    game.collision.Parameters["BottomLeft"].SetValue(overlap.Vertices[2].TextureCoordinate);
            //    game.collision.Parameters["Length"].SetValue(overlap.Length_X);
            //    // collision.Parameters["SecondMap"].SetValue(normalMap);
            //    foreach (EffectPass pass in game.collision.CurrentTechnique.Passes)
            //    //foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        game.GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //             PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, ReverseIndices, 0, Indices.Length / 3);
            //    }
            //}
           
            return (Texture2D)texture;
            //}

            //return game.normalMap;
            
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
