using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Parallax_Demo
{
    class DecalOverlap
    {
        List<FootDecal> footprints;
        Matrix world;
        FootDecal mainFootprint;

        public DecalOverlap(FootDecal centre)
        {
            footprints = new List<FootDecal>();
            world = Matrix.Invert(centre.World);
        }

        public void AddOverlap(FootDecal footprint)
        {
            footprints.Add(footprint);
        }


    }
}
