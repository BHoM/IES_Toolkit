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
    
        [Description("Convert a collection of BHoM Environment Panels that represent a single volumetric space into the IES string representation for GEM format")]
        [Input("panelsAsSpace", "The collection of BHoM Environment Panels that represent a space")]
        [Output("iesSpace", "The IES string representation of the space for GEM")]
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
                gemSpace.Add(p.ToIES());

            foreach (Panel p in panelsAsSpace)
            {
                List<Point> v = p.Vertices();
                v.RemoveAt(v.Count - 1); //Remove the last point because we don't need duplicated points

                if (!p.NormalAwayFromSpace(panelsAsSpace))
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
       
        [Description("Convert an IES string representation of a space into a collection of BHoM Environment Panels")]
        [Input("iesSpace", "The IES representation of a space")]
        [Output("panelsAsSpace", "BHoM Environment Space")]
        public static List<Panel> ToBHoMPanels(this List<string> iesSpace)
        {
            List<Panel> panels = new List<Panel>();
            //Convert the strings which make up the IES Gem file back into BHoM panels.
            string spaceName = iesSpace[0]; //First string is the name

            int numCoordinates = System.Convert.ToInt32(iesSpace[1].Split(' ')[0]); //First number is the number of coordinates
            int numPanels = System.Convert.ToInt32(iesSpace[1].Split(' ')[1]); //Second number is the number of panels (faces) of the space

            List<string> iesPoints = new List<string>();
            for (int x = 0; x < numCoordinates; x++)
                iesPoints.Add(iesSpace[x + 2]);

            List<Point> bhomPoints = iesPoints.Select(x => x.ToBHoMPoint()).ToList();

            int count = numCoordinates + 2;
            while(count < iesSpace.Count)
            {
                //Convert to panels
                List<string> panelCoord = iesSpace[count].Trim().Split(' ').ToList();
                List<Point> pLinePts = new List<Point>();
                for (int y = 1; y < panelCoord.Count; y++)
                    pLinePts.Add(bhomPoints[System.Convert.ToInt32(panelCoord[y]) - 1]);

                pLinePts.Add(pLinePts.First());

                Polyline pLine = new Polyline { ControlPoints = pLinePts, };

                Panel panel = new Panel();
                panel.ExternalEdges = pLine.ToEdges();
                panel.Openings = new List<Opening>();

                count++;
                int numOpenings = System.Convert.ToInt32(iesSpace[count]);
                count++;
                int countOpenings = 0;
                while(countOpenings < numOpenings)
                {
                    string openingData = iesSpace[count];
                    int numCoords = System.Convert.ToInt32(openingData.Split(' ')[0]);
                    count++;

                    List<string> openingPts = new List<string>();
                    for (int x = 0; x < numCoords; x++)
                        openingPts.Add(iesSpace[count + x]);

                    panel.Openings.Add(openingPts.ToBHoM(openingData.Split(' ')[1]));

                    count += numCoords;
                    countOpenings++;
                }

                panels.Add(panel);
            }

            //Fix the openings now
            foreach(Panel p in panels)
            {
                for (int x = 0; x < p.Openings.Count; x++)
                {
                    p.Openings[x] = p.Openings[x].RepairOpening(p, panels);
                }
            }

            return panels;
        }
    }
}