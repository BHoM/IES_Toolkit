using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Query
    {
        public static Plane FitPlane2(this List<Point> verts)
        {
            verts.RemoveAt(verts.Count - 1);
            var cprods = new List<Vector>();
            var baseVert = verts[0];
            for (int x = 0; x < verts.Count - 2; x++)
            {
                cprods.Add(NormalFrom3Pts(baseVert, verts[x + 1], verts[x + 2]));
            }

            var normal = new Vector() {X = 0, Y = 0, Z = 0};
            foreach (var cprodx in cprods)
            {
                normal.X += cprodx.X;
                normal.Y += cprodx.Y;
                normal.Z += cprodx.Z;
            }
            //normalise the vector
            Vector normalVec = null;

            if ((normal.X != 0) || (normal.Y != 0) || (normal.Z != 0))
            {
                var ds = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y,2) + Math.Pow(normal.Z,2));
                normalVec = new Vector()
                {
                    X = normal.X / ds,
                    Y = normal.Y / ds,
                    Z = normal.Z / ds
                };
            }
            else
            {
                normalVec = new Vector()
                {
                    X = 0,
                    Y = 0,
                    Z = 1
                };
            }
            return new Plane()
            {
                Origin = verts[0],
                Normal = normalVec
            };
        }

        public static Vector NormalFrom3Pts (this Point pt1, Point pt2, Point pt3)
        {
            var v1 = new Vector
            {
                X = pt2.X - pt1.X,
                Y = pt2.Y - pt1.Y,
                Z = pt2.Z - pt1.Z
            };
            var v2 = new Vector
            {
                X = pt3.X - pt1.X,
                Y = pt3.Y - pt1.Y,
                Z = pt3.Z - pt1.Z
            };

            return new Vector 
            {
                X = v1.Y * v2.Z - v1.Z * v2.Y,
                Y = -v1.X * v2.Z + v1.Z * v2.X,
                Z = v1.X * v2.Y - v1.Y * v2.X,
            };
        }
    }
}
