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
       public static List<string> ToIES(this List<Panel> panelsAsSpace)
       {
            List<string> gemSpace = new List<string>();

            gemSpace.Add("LAYER\n");
            gemSpace.Add("1\n");
            gemSpace.Add("COLOUR\n");
            gemSpace.Add("1\n");
            gemSpace.Add("CATEGORY\n");
            gemSpace.Add("1\n");
            gemSpace.Add("TYPE\n");
            gemSpace.Add("1\n");
            gemSpace.Add("COLOURRGB\n");
            gemSpace.Add("16711690\n");

            gemSpace.Add("IES " + panelsAsSpace.ConnectedSpaceName() + "\n");

            List<Point> spaceVertices = new List<Point>();
            foreach(Panel p in panelsAsSpace)
                spaceVertices.AddRange(p.Vertices().Select(x => x.RoundedPoint()));

            spaceVertices = spaceVertices.Distinct().ToList();

            gemSpace.Add(spaceVertices.Count.ToString() + " " + panelsAsSpace.Count.ToString() + "\n");

            foreach (Point p in spaceVertices)
                gemSpace.Add(" " + p.X.ToString() + " " + p.Y.ToString() + " " + p.Z.ToString() + "\n");

            foreach (Panel p in panelsAsSpace)
            {
                List<Point> v = p.Vertices();
                v.RemoveAt(v.Count - 1); //Remove the last point because we don't need duplicated points

                if (!p.NormalAwayFromSpace(panelsAsSpace) && p.ConnectedSpaces[0] == panelsAsSpace.ConnectedSpaceName())
                    v.Reverse(); //Reverse the point order if the normal is not away from the space but the first adjacency is this space

                string s = v.Count.ToString() + " ";
                foreach(Point pt in v)
                    s += (spaceVertices.IndexOf(pt.RoundedPoint()) + 1) + " ";

                s += "\n";
                gemSpace.Add(s);

                if (p.Openings.Count == 0)
                    gemSpace.Add("0\n");
                else
                {
                    gemSpace.Add(p.Openings.Count.ToString() + "\n");
                    Point panelXY = p.BottomLeft(panelsAsSpace);
                    foreach (Opening o in p.Openings)
                        gemSpace.AddRange(o.ToIES(panelXY));
                }
            }

            return gemSpace;
       }
    }
}