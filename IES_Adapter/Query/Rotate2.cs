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
        public static Vector Rotate2(this Vector vec, double angle, Vector axis)
        {
            var x = vec.X; var y = vec.Y; var z = vec.Z;
            var u = axis.X; var v = axis.Y; var w = axis.Z;
            //Extracted common factors for simplicity and efficiency
            var r2 = Math.Pow(u,2) + Math.Pow(v,2) + Math.Pow(w, 2);
            var r = Math.Sqrt(r2);
            var ct = Math.Cos(angle);
            var st = Math.Sin(angle) / r;
            var dt = (u * x + v * y + w * z) * (1 - ct) / r2;

            return new Vector ()
            {
                X = u * dt + x * ct + (-w * y + v * z) * st,
                Y = v * dt + y * ct + (w * x - u * z) * st,
                Z = w * dt + z * ct + (-v * x + u * y) * st,
            };
        }
    }
}
