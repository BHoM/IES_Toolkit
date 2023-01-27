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
        public static Polyline PolygonInFace(this Opening opening, Panel hostPanel, Point origin = null, bool flip = false)
        {
            BH.oM.Geometry.CoordinateSystem.Cartesian coordinateSystem = hostPanel.UpperOrientatedPlane();
            //Plane plane = hostPanel.Polyline().ControlPoints.FitPlane2();

            /*if (flip)
                coordinateSystem = coordinateSystem.FlipPlane();

            if(origin != null)
                coordinateSystem.Origin = origin;

            TransformMatrix transformation = Engine.Geometry.Create.OrientationMatrix(coordinateSystem, new Cartesian());
            return Engine.Geometry.Create.Polyline(opening.Polyline().ControlPoints.Select(x => x.Transform(transformation)));*/


            /*if (origin == null && flip)
                refPlane = refPlane.Flip();// = coordinateSystem.FlipPlane();
            else
            {*/
                /*if (coordinateSystem.Z.IsParallel(Vector.ZAxis) != 0) //Parallel to Z
                {
                    Vector localX = flip ? -Vector.XAxis : Vector.XAxis;
                    Vector localY = coordinateSystem.Z.CrossProduct(localX);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(origin, localX, localY);
                }
                else
                {
                    var projY = Vector.ZAxis.Project(plane);
                    var projX = plane.Normal.CrossProduct(projY);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(plane.Origin, projX, projY);
            }
            //}*/

            var vertices = opening.Polyline().ControlPoints;
            var pts2D = vertices.Select(x => x.XyzToXy(coordinateSystem)).ToList();

            return Engine.Geometry.Create.Polyline(pts2D);
        }
    }
}
