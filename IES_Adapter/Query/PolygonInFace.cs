using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Query
    {
        public static Polyline PolygonInFace ( this Opening opening, Panel hostPanel, Point origin, bool flip )
        {


            Cartesian coordinateSystem = hostPanel.UpperOrientatedPlane();

            if (origin is null)
            {
                if (flip)
                {
                    coordinateSystem = coordinateSystem.FlipPlane();
                }
            }
            else
            {
                if (coordinateSystem.Z.IsParallel(Vector.ZAxis) != 0) //Parallel to Z
                {
                    Vector localX = flip ? -Vector.XAxis : Vector.XAxis;
                    Vector localY = coordinateSystem.Z.CrossProduct(localX);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(origin, localX, localY);

                }
                else
                {
                    Vector projY = Vector.ZAxis.Project(coordinateSystem.Z);
                    Vector projX = projY.Rotate(-Math.PI / 2, coordinateSystem.Z);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(origin, projX, projY);
                
                }
            }
            var vertices = opening.Polyline().ControlPoints;
            var pts2D = new List<Point>();

            foreach (Point pt in vertices)
                {
                   var pt2D = pt.XyzToXy(coordinateSystem);
                    pts2D.Add(pt2D);
                }
            return Engine.Geometry.Create.Polyline(pts2D);
        }
/*
        public static Cartesian CoordinateSystem(this Panel hostPanel) 
        {
            Polyline boundary = hostPanel.Polyline();
            Plane plane = boundary.FitPlane();

            Vector localX = (boundary.ControlPoints[1] - boundary.ControlPoints[0]).Normalise();

            Vector localY = plane.Normal.CrossProduct(localX);

            return new Cartesian(plane.Origin, localX, localY, plane.Normal);
        }
*/
    }
}
