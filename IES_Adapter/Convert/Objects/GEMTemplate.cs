using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        public static List<string> ToIES(this GEMTemplate gemTemplate)
        {
            List<string> gemLines = new List<string>();

            gemLines.Add("LAYER\n");
            gemLines.Add($"{gemTemplate.Layer}\n");

            gemLines.Add("COLOUR\n");
            gemLines.Add($"{gemTemplate.Colour}\n");

            gemLines.Add("CATEGORY\n");
            gemLines.Add($"{gemTemplate.Category}\n");

            gemLines.Add("TYPE\n");
            gemLines.Add($"{gemTemplate.Type}\n");

            gemLines.Add("SUBTYPE\n");
            gemLines.Add($"{gemTemplate.SubType}\n");

            gemLines.Add("COLOURRGB\n");
            gemLines.Add($"{gemTemplate.ColourRGB}\n");

            gemLines.Add($"IES {gemTemplate.Name}\n");
            gemLines.Add($"{gemTemplate.VerticesCount.ToString()} {gemTemplate.FaceCount.ToString()}\n");

            return gemLines;
        }
    }
}
