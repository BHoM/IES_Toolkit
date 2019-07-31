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

namespace BH.Engine.IES
{
    public static partial class Convert
    {

        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format")]
        [Input("opening", "The BHoM Environment Opening to convert")]
        [Input("panelXYZ", "The bottom left corner point of the host panel to calculate the opening points from for GEM format")]
        [Output("iesOpening", "The string representation for IES GEM format")]
        public static List<string> ToIES(this Opening opening, Point panelXYZ)
        {
            List<string> gemOpening = new List<string>();

            List<Point> vertices = opening.Polyline().IDiscontinuityPoints();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES() + "\n");

            bool useYZ = false;
            bool useXZ = false;
            if (vertices.Min(x => x.X) == vertices.Max(x => x.X))
                useYZ = true;
            else if (vertices.Min(x => x.Y) == vertices.Max(x => x.Y))
                useXZ = true;

            foreach (Point p in vertices)
            {
                if (!useXZ && !useYZ)
                    gemOpening.Add(" " + Math.Abs((p.X - panelXYZ.X)).ToString() + " " + Math.Abs((p.Y - panelXYZ.Y)).ToString() + "\n");
                else if (useXZ)
                    gemOpening.Add(" " + Math.Abs((p.X - panelXYZ.X)).ToString() + " " + Math.Abs((p.Z - panelXYZ.Z)).ToString() + "\n");
                else if (useYZ)
                    gemOpening.Add(" " + Math.Abs((p.Y - panelXYZ.Y)).ToString() + " " + Math.Abs((p.Z - panelXYZ.Z)).ToString() + "\n");

            }

            return gemOpening;
        }

        [Description("Convert an IES string representation of an opening to a BHoM Environment Opening")]
        [Input("openingPts", "The string representations of coordinates that make up the opening")]
        [Input("openingType", "The IES representation of the opening type")]
        [Output("opening", "The BHoM Environment Opening converted from IES GEM format")]
        public static Opening ToBHoM(this List<string> openingPts, string openingType)
        {
            List<Point> points = openingPts.Select(x => x.ToBHoMPoint()).ToList();
            points.Add(points.First());
            /*for(int x = 0; x < points.Count; x++)
            {
                points[x].X += panelXY.X;
                points[x].Y += panelXY.Y;
                points[x].Z += panelXY.Z;
            }*/

            Polyline pLine = new Polyline { ControlPoints = points, };

            Opening opening = new Opening();
            opening.Edges = pLine.ToEdges();
            opening.Type = openingType.ToBHoMOpeningType();

            return opening;
        }
    }
}