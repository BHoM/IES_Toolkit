/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base.Attributes;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.IES.Fragments;
using BH.oM.Environment.IES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Environment;
using BH.Engine.Geometry;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        [Description("Convert a collection of BHoM Environment Panels that represent a single volumetric space into the IES string representation for GEM format")]
        [Input("panelsAsSpace", "The collection of BHoM Environment Panels that represent a space")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesSpace", "The IES string representation of the space for GEM")]
        public static List<string> ToIES(this List<Panel> panelsAsSpace, PushConfigIES settingsIES)
        {
            if (panelsAsSpace == null || panelsAsSpace.Count == 0)
                return new List<string>();

            List<Panel> panels = panelsAsSpace.Where(x => x.ExternalEdges.Count > 0).ToList();

            if (panels.Count != panelsAsSpace.Count)
                BH.Engine.Base.Compute.RecordWarning("The space " + panelsAsSpace.ConnectedSpaceName() + " has panels which did not contain geometry. Panels without valid geometry cannot be converted for IES to handle and have been ignored.");

            List<string> gemSpace = new List<string>();

            if (panels.First().IsShade())
                gemSpace.AddRange(Create.ShadeTemplate().ToIES());
            else
                gemSpace.AddRange(Create.SpaceTemplate().ToIES());

            gemSpace.Add("IES " + panels.ConnectedSpaceName() + "\n");

            List<Point> spaceVertices = new List<Point>();
            foreach (Panel p in panels)
                spaceVertices.AddRange(p.Vertices().Select(x => x.RoundCoordinates(settingsIES.DecimalPlaces)));

            spaceVertices = spaceVertices.Distinct().ToList();

            gemSpace.Add(spaceVertices.Count.ToString() + " " + panels.Count.ToString() + "\n");

            foreach (Point p in spaceVertices)
                gemSpace.Add(p.ToIES(settingsIES));

            Point zero = new Point { X = 0, Y = 0, Z = 0 };

            foreach (Panel p in panels)
            {
                if (p.Type == PanelType.Air && p.Openings.Count == 0)
                    p.Openings.Add(new Opening { Edges = new List<Edge>(p.ExternalEdges), Type = OpeningType.Hole }); //Air walls need the polyline adding as an opening of type hole

                if(!p.NormalAwayFromSpace(panels, settingsIES.PlanarTolerance))
                    p.ExternalEdges = p.Polyline().Flip().ToEdges(); //Reverse the point order if the normal is not away from the space but the first adjacency is this space

                List<Point> v = p.Vertices();
                v.RemoveAt(v.Count - 1); //Remove the last point because we don't need duplicated points

                string s = v.Count.ToString() + " ";
                foreach (Point pt in v)
                    s += (spaceVertices.IndexOf(pt.RoundCoordinates(settingsIES.DecimalPlaces)) + 1) + " ";

                s += "\n";
                gemSpace.Add(s);

                if (p.Openings.Count == 0)
                    gemSpace.Add("0\n");
                else
                {
                    gemSpace.Add(p.Openings.Count.ToString() + "\n");

                    foreach (Opening o in p.Openings)
                        gemSpace.AddRange(o.ToIES(p, settingsIES));
                }
            }

            return gemSpace;
        }

        [Description("Convert an IES string representation of a space into a collection of BHoM Environment Panels")]
        [Input("iesSpace", "The IES representation of a space")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("panelsAsSpace", "BHoM Environment Space")]
        public static List<Panel> FromIES(this List<string> iesSpace, PullConfigIES settingsIES)
        {
            List<Panel> panels = new List<Panel>();

            //Convert the strings which make up the IES Gem file back into BHoM panels.
            string spaceName = iesSpace[0]; //First string is the name
            if (spaceName.StartsWith("IES"))
            {
                spaceName = spaceName.Substring(3);
                spaceName = spaceName.Trim();
            }

            int numCoordinates = System.Convert.ToInt32(iesSpace[1].Split(' ')[0]); //First number is the number of coordinates
            int numPanels = System.Convert.ToInt32(iesSpace[1].Split(' ')[1]); //Second number is the number of panels (faces) of the space

            List<string> iesPoints = new List<string>();
            for (int x = 0; x < numCoordinates; x++)
                iesPoints.Add(iesSpace[x + 2]);

            List<Point> bhomPoints = iesPoints.Select(x => x.FromIES(settingsIES)).ToList();

            //Number of coordinated + 2 to get on to the line of the fist panel in GEM
            int count = numCoordinates + 2;
            while (count < iesSpace.Count)
            {
                //Convert to panels
                List<string> panelCoord = iesSpace[count].Trim().Split(' ').ToList();
                List<Point> pLinePts = new List<Point>();
                for (int y = 1; y < panelCoord.Count; y++)
                    pLinePts.Add(bhomPoints[System.Convert.ToInt32(panelCoord[y]) - 1]);

                //Add first point to close polyline
                pLinePts.Add(pLinePts.First());

                Polyline pLine = new Polyline { ControlPoints = pLinePts, };


                Panel panel = new Panel();
                panel.ExternalEdges = pLine.ToEdges();
                panel.Openings = new List<Opening>();
                panel.ConnectedSpaces = new List<string>();
                panel.ConnectedSpaces.Add(spaceName);

                SurfaceIndexFragment fragment = new SurfaceIndexFragment();
                fragment.SurfaceID = panels.Count;
                panel.Fragments.Add(fragment);

                count++;
                int numOpenings = System.Convert.ToInt32(iesSpace[count]);
                count++;
                int countOpenings = 0;
                while (countOpenings < numOpenings)
                {
                    string openingData = iesSpace[count];
                    int numCoords = System.Convert.ToInt32(openingData.Split(' ')[0]);
                    count++;

                    List<string> openingPts = new List<string>();
                    for (int x = 0; x < numCoords; x++)
                        openingPts.Add(iesSpace[count + x]);

                    if(settingsIES.PullOpenings)
                        panel.Openings.Add(openingPts.FromIESOpening(openingData.Split(' ')[1], pLine, settingsIES));

                    count += numCoords;
                    countOpenings++;
                }

                panels.Add(panel);
            }

            return panels;
        }
    }
}
