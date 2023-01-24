using BH.Engine.Environment;
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
        public static Point LowerLeftCorner(this Panel panel)
        {
            var pline = panel.Polyline();
            var refPlane = panel.UpperOrientatedPlane();

            var coords = pline.ControlPoints;
            coords = coords.Select(x => x.XyzToXy(refPlane)).ToList();
            
            var minX = coords.Min(pt => pt.X);
            var minY = coords.Min(pt => pt.Y);

        }
    }
}
