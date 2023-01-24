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



            var fuckYou = hostPanel.Polyline().FitPlane().Normal;
            //Vector normal = 

        }
    }
}
