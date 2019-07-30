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

        [Description("Gets the bottom left most point of a panel when looking from within the space to the outside")]
        [Input("panel", "The Environment Panel to get the bottom left most point of")]
        [Input("panelsAsSpace", "A collection of Environment Panels representing a single space")]
        [Output("bottomLeftPoint", "The bottom left most point of the panel")]
        public static Point BottomLeft(this Panel panel, List<Panel> panelsAsSpace)
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
            double minZ = pnts.Min(x => x.Z);
            pnts = pnts.Where(x => x.Z == minZ).ToList();

            Point leftMost = null;
            foreach(Point p in pnts)
            {
                if (IsLeft(line, p))
                    leftMost = p;
            }

            return leftMost;
        }
    }
}
