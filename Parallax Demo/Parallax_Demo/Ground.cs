using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parallax_Demo
{
    class Ground
    {

        VertexPositionNormalTexture[] beach;
        int[] indices;
        int UVMultiplier = 32;

        public Ground()
        {
            indices = new int[4];
            beach = new VertexPositionNormalTexture[4];

            beach[0].Position = new Vector3(50, -0.01f, 100);
            beach[1].Position = new Vector3(50, -0.01f, -100);
            beach[2].Position = new Vector3(-50, -0.01f, 100);
            beach[3].Position = new Vector3(-50, -0.01f, -100);

            beach[0].Normal = new Vector3(0, 1, 0);
            beach[1].Normal = new Vector3(0, 1, 0);
            beach[2].Normal = new Vector3(0, 1, 0);
            beach[3].Normal = new Vector3(0, 1, 0);

            beach[0].TextureCoordinate = new Vector2(0, 0);
            beach[1].TextureCoordinate = new Vector2(0, UVMultiplier);
            beach[2].TextureCoordinate = new Vector2(UVMultiplier, 0);
            beach[3].TextureCoordinate = new Vector2(UVMultiplier, UVMultiplier);


            indices = new int[] { 1, 0, 2, 1, 2, 3 };
        }

        public VertexPositionNormalTexture[] Vertices
        {
            get { return beach; }
        }

        public int[] Indices
        {
            get { return indices; }
        }

        public float Length_X
        {
            get { return (beach[2].Position.X - beach[0].Position.X) / UVMultiplier; }
        }

        public float Length_Z
        {
            get { return (beach[1].Position.Z - beach[0].Position.Z) / UVMultiplier; }
        }
    }
}
