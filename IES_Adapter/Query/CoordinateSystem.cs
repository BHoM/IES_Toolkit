using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Geometry;

namespace BH.Adapter.IES
{
    public static partial class Query
    {
        public static Cartesian CoordinateSystem(this Polyline pLine, double distanceTolerance = Tolerance.MacroDistance, double angleTolerance = Tolerance.Angle)
        {
            Vector locZ = pLine.Normal(distanceTolerance); Cartesian baseCartesiean; if (locZ.IsParallel(Vector.ZAxis, angleTolerance) != 0)
            {
                baseCartesiean = new Cartesian(pLine.ControlPoints[0], -Vector.XAxis, locZ.CrossProduct(-Vector.XAxis), locZ);
            }
            else
            {
                Vector locY = (Vector.ZAxis - Vector.ZAxis.DotProduct(locZ) * locZ).Normalise();
                Vector locX = locY.CrossProduct(locZ);
                baseCartesiean = new Cartesian(pLine.ControlPoints[0], locX, locY, locZ);
            }
            TransformMatrix locGlob1 = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(baseCartesiean);
            Point minPtLocal = pLine.ControlPoints.Select(x => x.Transform(locGlob1)).ToList().Bounds().Min;
            TransformMatrix globToLoc = BH.Engine.Geometry.Create.OrientationMatrixGlobalToLocal(baseCartesiean);
            baseCartesiean.Origin = minPtLocal.Transform(globToLoc); return baseCartesiean;
        }
    }
}
