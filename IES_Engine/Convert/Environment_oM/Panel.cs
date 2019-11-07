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
        [Description("Convert a collection of BHoM Environment Panels that represent shading elements into the IES string representation for GEM format")]
        [Input("panelsAsSpace", "The collection of BHoM Environment Panels that represent shading elements")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesSpace", "The IES string representation of shade for GEM")]
        public static List<string> ToIESShades(this List<Panel> panelsAsShade, SettingsIES settings)
        {
            List<string> gemPanel = new List<string>();

            for (int x = 0; x < panelsAsShade.Count; x++)
            {
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
                gemPanel.Add("IES IES_SHD_" + (x + 1).ToString() + "\n");
                List<Point> points = panelsAsShade[x].Vertices().Select(y => y.RoundedPoint()).ToList();
                points = points.Distinct().ToList();
                gemPanel.Add(points.Count.ToString() + " 1");

                string s = points.Count.ToString();

                foreach(Point p in points)
                {
                    gemPanel.Add(p.ToIES(settings));
                    s += " " + points.IndexOf(p).ToString();
                }
                s += "\n";
                gemPanel.Add(s);
                gemPanel.Add("0");
            }

            return gemPanel;
        }
    }
}