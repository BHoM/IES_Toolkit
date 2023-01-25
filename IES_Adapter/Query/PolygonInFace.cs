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
        public static Polyline PolygonInFace(this Opening opening, Panel hostPanel, Point origin, bool flip )
        {
            BH.oM.Geometry.CoordinateSystem.Cartesian coordinateSystem = hostPanel.UpperOrientatedPlane();

            if (flip)
                coordinateSystem = coordinateSystem.FlipPlane();

            if (origin != null)
                coordinateSystem.Origin = origin;

            TransformMatrix transformation = Engine.Geometry.Create.OrientationMatrix(coordinateSystem, new Cartesian());
            return Engine.Geometry.Create.Polyline(opening.Polyline().ControlPoints.Select(x => x.Transform(transformation)));
        }

        public static Cartesian UpperOrientatedPlane(this Panel hostPanel) 
        {
            Polyline boundary = hostPanel.Polyline();
            Plane plane = boundary.FitPlane();

            Vector localX = (boundary.ControlPoints[1] - boundary.ControlPoints[0]).Normalise();

            Vector localY = plane.Normal.CrossProduct(localX);

            return new Cartesian(plane.Origin, localX, localY, plane.Normal);
        }

    }
}
