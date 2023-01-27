using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
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
        public static Cartesian UpperOrientatedPlane(this Panel panel)
        {
            /*
            var plane = panel.Polyline().ControlPoints.FitPlane2();

            if (plane.Normal.Z == 1 || plane.Normal.Z == -1)
                return new Cartesian(plane.Origin, Vector.XAxis, Vector.YAxis, Vector.ZAxis);
            else
            {
                var projY = Vector.ZAxis.Project(plane);
                var projX = projY.Rotate2(Math.PI / -2, plane.Normal);
                return new Cartesian(plane.Origin, projX, Vector.YAxis, Vector.ZAxis);
            }*/

            var plane = panel.Polyline().ControlPoints.FitPlane2();

            //if (plane.Normal.Z == 1 || plane.Normal.Z == -1)
            if (plane.Normal.IsParallel(Vector.ZAxis) != 0)
                return BH.Engine.Geometry.Create.CartesianCoordinateSystem(plane.Origin, Vector.XAxis, plane.Normal.CrossProduct(Vector.XAxis));// new Cartesian(plane.Origin, Vector.XAxis, Vector.YAxis, Vector.ZAxis);
            else
            {

                var projY = Vector.ZAxis.Project(plane);
                var projX = plane.Normal.CrossProduct(projY);
                return Engine.Geometry.Create.CartesianCoordinateSystem(plane.Origin, projX, projY);
            }
        }
    }
}
