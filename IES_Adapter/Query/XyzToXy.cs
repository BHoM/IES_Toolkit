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
        public static Point XyzToXy(this Point pt, Cartesian refPlane)
        {
            var diff = new Vector()
            {
                X = pt.X - refPlane.Origin.X,
                Y = pt.Y - refPlane.Origin.Y,
                Z = pt.Z - refPlane.Origin.Z,
            };

            return new Point()
            {
                X = refPlane.X.DotProduct(diff),
                Y = refPlane.Y.DotProduct(diff),
                Z = 0
            };
        }
    }
}
