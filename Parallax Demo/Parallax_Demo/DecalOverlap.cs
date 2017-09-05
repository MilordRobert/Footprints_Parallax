using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parallax_Demo
{
    class DecalOverlap : FootDecal
    {
        FootDecal footprint;
        RenderTarget2D collisionMap;

        public DecalOverlap(FootDecal overlap, Vector3 position, float size, Ground ground, float rotation, Texture2D normalMap, Footprint_Game game, bool running, bool right, float weight)
            : base (position, size * 2, ground, rotation, normalMap, game, running, right, weight)
        {
            collisionMap = new RenderTarget2D(game.GraphicsDevice, 1024, 1024, true, SurfaceFormat.Color, DepthFormat.None);
            footprint = overlap;
        }

        public override Texture2D prepareTexture(Footprint_Game game)
        {
            //return base.prepareTexture(game);

            Texture2D tex = footprint.prepareTexture(game);
            game.collision.Parameters["SecondMap"].SetValue(tex);

            game.GraphicsDevice.SetRenderTarget(collisionMap);
            //GraphicsDevice.Clear(Color.Transparent);
            //foreach (FootDecal f in steps)
            //collision.Parameters["NormalMap"].SetValue(normalMap);
            //collision.Parameters["size"].SetValue(0.25f);
            //collision.Parameters["World"].SetValue(Matrix.Identity);
            //collision.Parameters["SecondWorld"].SetValue(steps[0].World * Matrix.Invert(steps[1].World));
            //collision.Parameters["SecondWorldInverse"].SetValue(Matrix.Invert(steps[0].World * Matrix.Invert(steps[1].World)));
            //collision.Parameters["TopLeft"].SetValue(steps[1].Vertices[0].TextureCoordinate);
            //collision.Parameters["TopRight"].SetValue(steps[1].Vertices[1].TextureCoordinate);
            //collision.Parameters["BottomLeft"].SetValue(steps[1].Vertices[2].TextureCoordinate);
            //collision.Parameters["Length"].SetValue(steps[1].Length_X);
            //collision.Parameters["SecondMap"].SetValue(normalMap);
            //foreach (EffectPass pass in collision.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //         PrimitiveType.TriangleList, steps[0].Vertices, 0, steps[0].Vertices.Length, steps[0].ReverseIndices, 0, steps[0].Indices.Length / 3);
            //}
            //GraphicsDevice.SetRenderTarget(null);
            //game.runningEffect.Parameters["World"].SetValue(Matrix.Identity);

            ////runningEffect.Parameters["SecondMap"].SetValue(hole);
            //game.runningEffect.Parameters["Extract"].SetValue(game.extract);
            //if (RightFoot)
            //    game.runningEffect.Parameters["NormalMap"].SetValue(game.normalMap);
            //else
            //    game.runningEffect.Parameters["NormalMap"].SetValue(game.leftFoot);
            
            //if (footprint.RightFoot)
            //    game.runningEffect.Parameters["NormalMap"].SetValue(game.normalMap);
            //else
            //    game.runningEffect.Parameters["NormalMap"].SetValue(game.leftFoot);

           


            game.collision.Parameters["NormalMap"].SetValue(game.normalMap);
            game.collision.Parameters["size"].SetValue(1 / size);
            //game.collision.Parameters["Osize"].SetValue(footprint.Length_X / 2);
            game.collision.Parameters["World"].SetValue(Matrix.Identity);
            game.collision.Parameters["SecondWorld"].SetValue(World * Matrix.Invert(footprint.World));
            game.collision.Parameters["SecondWorldInverse"].SetValue(Matrix.Invert(World * Matrix.Invert(footprint.World)));
            game.collision.Parameters["TopLeft"].SetValue(footprint.Vertices[0].TextureCoordinate);
            game.collision.Parameters["TopRight"].SetValue(footprint.Vertices[1].TextureCoordinate);
            game.collision.Parameters["BottomLeft"].SetValue(footprint.Vertices[2].TextureCoordinate);
            game.collision.Parameters["Length"].SetValue(Length_X);

            foreach (EffectPass pass in game.collision.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
                     PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, ReverseIndices, 0, Indices.Length / 3);
            }
            game.GraphicsDevice.SetRenderTarget(null);
            return collisionMap;
        }


    }
}
