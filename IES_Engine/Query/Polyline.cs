using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Bounding Box             ****/
        /***************************************************/

        [Description("Get a 2d polyline representation of a BHoM BoundingBox")]
        [Input("box", "The BHoM bounding box")]
        [Output("polyline", "The 2d polyline representation of the box")]
        public static Polyline ToPolyline(this BoundingBox box)
        {
            Point min = box.Min.RoundedPoint();
            Point max = box.Max.RoundedPoint();

            double minDist = Math.Min(max.X - min.X, max.Y - min.Y);
            minDist = Math.Min(minDist, max.Z - min.Z);

            List<Point> pnts = new List<Point>();
            pnts.Add(min);

            if((max.X - min.X) == minDist || (max.Z - min.Z) == minDist)
            {
                //YZ plane
                /*pnts.Add(new Point { X = min.X, Y = max.Y, Z = min.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = min.Y, Z = max.Z });*/

                double x1 = min.Z;
                double x2 = max.Z;
                double y1 = min.Y;
                double y2 = max.Y;

                double xc = (x1 + x2) / 2;
                double xd = (x1 - x2) / 2;

                double yc = (y1 + y2) / 2;
                double yd = (y1 - y2) / 2;

                double x3 = xc - yd;
                double x4 = xc + yd;
                double y3 = yc + xd;
                double y4 = yc - xd;

                pnts.Add(new Point { X = min.X, Y = y3, Z = x3 });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = y4, Z = x4 });
            }
            else if ((max.Y - min.Y) == minDist)
            {
                //XZ
                /*pnts.Add(new Point { X = min.X, Y = min.Y, Z = max.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = max.Y, Z = min.Z });*/

                double x1 = min.X;
                double x2 = max.X;
                double y1 = min.Z;
                double y2 = max.Z;

                double xc = (x1 + x2) / 2;
                double xd = (x1 - x2) / 2;

                double yc = (y1 + y2) / 2;
                double yd = (y1 - y2) / 2;

                double x3 = xc - yd;
                double x4 = xc + yd;
                double y3 = yc + xd;
                double y4 = yc - xd;

                pnts.Add(new Point { X = x3, Y = min.Y, Z = y3 });
                pnts.Add(max);
                pnts.Add(new Point { X = x4, Y = max.Y, Z = y4 });
            }
            else
            {
                double x1 = min.X;
                double x2 = max.X;
                double y1 = min.Y;
                double y2 = max.Y;

                double xc = (x1 + x2) / 2;
                double xd = (x1 - x2) / 2;

                double yc = (y1 + y2) / 2;
                double yd = (y1 - y2) / 2;

                double x3 = xc - yd;
                double x4 = xc + yd;
                double y3 = yc + xd;
                double y4 = yc - xd;

                pnts.Add(new Point { X = x3, Y = y3, Z = min.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = x4, Y = y4, Z = max.Z });
            }

            /*pnts.Add(new Point { X = min.X, Y = min.Y, Z = max.Z });
            pnts.Add(new Point { X = min.X, Y = max.Y, Z = max.Z });
            pnts.Add(new Point { X = min.X, Y = max.Y, Z = min.Z });*/

            pnts.Add(min);

            return new Polyline { ControlPoints = pnts };
        }
    }
}
