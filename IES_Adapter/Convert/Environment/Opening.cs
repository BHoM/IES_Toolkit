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
            var panelNormal = hostPanel.Polyline().ControlPoints.FitPlane2().Normal;
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

            List<string> fuckingHell = new List<string>();

            fuckingHell.Add($"{verts_2d.ControlPoints.Count} 0\n");

            foreach (var hell in verts_2d.ControlPoints)
                fuckingHell.Add($"{BH.Adapter.IES.Convert.ToIES(hell, new oM.IES.Settings.SettingsIES(), 2)}");

            return fuckingHell;
        }
    }
}
