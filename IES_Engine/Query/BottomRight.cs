using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the bottom right most point of a panel when looking from within the space to the outside")]
        [Input("panel", "The Environment Panel to get the bottom right most point of")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("bottomRightPoint", "The bottom right most point of the panel")]
        public static Point BottomRight(this Panel panel, List<Panel> panelsAsSpace)
        {
            Vector normal = panel.Polyline().Normal();
            if (!panel.NormalAwayFromSpace(panelsAsSpace))
                normal = panel.Polyline().Flip().Normal();

            Point centre = panel.Polyline().Centroid();

            Line line = new Line
            {
                Start = centre,
                End = centre.Translate(normal),
            };

            List<Point> pnts = panel.Vertices();

            bool wasFlat = false;
            TransformMatrix transform = null;

            if(pnts.Min(x => Math.Round(x.Z, 6)) == pnts.Max(x => Math.Round(x.Z, 6)))
            {
                //All the points are on the same Z level - we're looking at a floor/roof
                Polyline pLine = panel.Polyline();
                transform = BH.Engine.Geometry.Create.RotationMatrix(pLine.Centroid(), new Vector { X = 1, Y = 0, Z = 0 }, 1.5708);
                pLine = pLine.Transform(transform);
                pnts = pLine.ControlPoints;
                line.End = line.Start.Translate(pLine.Normal());
                wasFlat = true;
            }

            Point rightMost = null;
            foreach(Point p in pnts)
            {
                if (!IsLeft(line, p) && (rightMost == null || rightMost.Z > p.Z))
                    rightMost = p;
            }

            if (wasFlat && rightMost != null)
                rightMost = rightMost.Transform(transform.Invert());

            return rightMost;
        }
    }
}
