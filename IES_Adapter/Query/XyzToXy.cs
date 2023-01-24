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

        public static Point XyToXyz(this Point pt, Cartesian refPlane)
        {
            var u = new Vector()
            {
                X = refPlane.X.X * pt.X,
                Y = refPlane.X.Y * pt.X,
                Z = refPlane.X.Z * pt.X,
            };

            var v = new Vector()
            {
                X = refPlane.Y.X * pt.Y,
                Y = refPlane.Y.Y * pt.Y,
                Z = refPlane.Y.Z * pt.Y,
            };

            return new Point()
            {
                X = refPlane.Origin.X + u.X + v.X,
                Y = refPlane.Origin.Y + u.Y + v.Y,
                Z = refPlane.Origin.Z + u.Z + v.Z,
            };
        }
    }
}
