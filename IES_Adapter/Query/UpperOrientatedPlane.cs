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
            var plane = panel.Polyline().FitPlane();

            if (plane.Normal.Z == 1 || plane.Normal.Z == -1)
                return new Cartesian(plane.Origin, Vector.XAxis, Vector.YAxis, Vector.ZAxis);
            else
            {
                var projY = Vector.ZAxis.Project(plane.Normal);
                var projX = projY.Rotate(Math.PI / -2, plane.Normal);
                return new Cartesian(plane.Origin, projX, Vector.YAxis, Vector.ZAxis);
            }
        }
    }
}
