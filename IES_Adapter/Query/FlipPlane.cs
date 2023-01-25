using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Query
    {
        public static Cartesian FlipPlane(this Cartesian refPlane)
        {
            return new Cartesian(
                refPlane.Origin,
                refPlane.X,
                -refPlane.Y,
                -refPlane.Z
            );
        }

    }
}
