using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        public static List<string> ToIES (this Opening opening, Panel hostPanel)
        {
            


            var panelNormal = hostPanel.Polyline().FitPlane().Normal;
            var origin = new Point();
            var flip = false;

            if (panelNormal.Z is (-1|1))
            {
                origin = hostPanel.UpperRightCorner();
                flip = true;
            }
            /*else if (panelNormal.Angle())
            {

            }*/
            else
            {
                origin = hostPanel.LowerLeftCorner();
                flip = false;
            }

            var verts_2d = opening.PolygonInFace(hostPanel,origin, flip);

        }
    }
}
