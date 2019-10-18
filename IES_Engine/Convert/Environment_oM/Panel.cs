using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Reflection;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;

namespace BH.Engine.IES
{
    public static partial class Convert
    {
        [Description("")]
        [Input("", "")]
        [Input("", "")]
        [Output("", "")]
        public static List<string> ToIESShades(this List<Panel> panelAsShade, SettingsIES settings)
        {
            List<string> gemPanel = new List<string>();

            /*for (int x = 0; x < panelAsShade.Count; x++)
            {
                string panelName = "IES_SHD_" + (x + 1).ToString();
            }*/

            gemPanel.Add("LAYER\n");
            gemPanel.Add("64\n");
            gemPanel.Add("COLOUR\n");
            gemPanel.Add("0\n");
            gemPanel.Add("CATEGORY\n");
            gemPanel.Add("1\n");
            gemPanel.Add("TYPE\n");
            gemPanel.Add("4\n");
            gemPanel.Add("COLOURRGB\n");
            gemPanel.Add("65280\n");

         //   gemPanel.Add("IES" + panelName + "\n");
            
            List<Point> shadeVertices = new List<Point>();
            foreach (Panel p in panelAsShade)
                shadeVertices.AddRange(p.Vertices().Select(x => x.RoundedPoint()));

            shadeVertices = shadeVertices.Distinct().ToList();

            gemPanel.Add(shadeVertices.Count.ToString() + " " + panelAsShade.Count.ToString() + "\n");

            foreach (Point p in shadeVertices)
                gemPanel.Add(p.ToIES(settings));

            Point zero = new Point { X = 0, Y = 0, Z = 0 };

            return gemPanel;


        }
    }
}