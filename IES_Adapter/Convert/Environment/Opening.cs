using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        public static List<string> ToIES(this Opening opening, Panel hostPanel, SettingsIES settingsIES)
        {
            List<string> rtn = new List<string>();

            var coordSystem = hostPanel.Polyline().CoordinateSystem();
            var localToGlobal = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(coordSystem);

            var polyline = opening.Polyline().Transform(localToGlobal);

            rtn.Add($"{polyline.ControlPoints.Count.ToString()} {opening.Type.ToIES(settingsIES)}\n");

            foreach (var cPoint in polyline.ControlPoints)
                rtn.Add($" {cPoint.ToIES(settingsIES, false)}");

            return rtn;
        }
    }
}
