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
        [Description("Convert a BHoM Geometry Point into an IES string representation for GEM files")]
        [Input("pt", "BHoM Geometry Point to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesPt", "The IES string representation of the point")]
        public static string ToIES(this Point pt, SettingsIES settings)
        {
            return " " + Math.Round(pt.X, 6).ToString() + " " + Math.Round(pt.Y, 6).ToString() + " " + Math.Round(pt.Z, 6).ToString() + "\n";
        }

        [Description("Convert an IES point representation to a BHoM point")]
        [Input("iesPt", "The IES string representation of a point to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("point", "A BHoM Geometry Point")]
        public static Point ToBHoMPoint(this string iesPt, SettingsIES settings)
        {
            try
            {
                iesPt = iesPt.Trim();
                string[] split = iesPt.Split(' ');
                return new Point
                {
                    X = System.Convert.ToDouble(split[0]),
                    Y = System.Convert.ToDouble(split[1]),
                    Z = (split.Length > 2 ? System.Convert.ToDouble(split[2]) : 0),
                };
            }
            catch(Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError("An error occurred in parsing that IES string to a BHoM point. Error was: " + e.ToString());
                return null;
            }
        }
    }
}