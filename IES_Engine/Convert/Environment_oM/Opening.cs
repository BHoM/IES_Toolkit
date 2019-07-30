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
        public static List<string> ToIES(this Opening opening, Point panelXY)
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
                    gemOpening.Add(" " + Math.Abs((p.X - panelXY.X)).ToString() + " " + Math.Abs((p.Y - panelXY.Y)).ToString() + "\n");
                else if (useXZ)
                    gemOpening.Add(" " + Math.Abs((p.X - panelXY.X)).ToString() + " " + Math.Abs((p.Z - panelXY.Z)).ToString() + "\n");
                else if (useYZ)
                    gemOpening.Add(" " + Math.Abs((p.Y - panelXY.Y)).ToString() + " " + Math.Abs((p.Z - panelXY.Z)).ToString() + "\n");

            }

            return gemOpening;
        }
    }
}