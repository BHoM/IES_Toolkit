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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        [Description("Convert a collection of BHoM Environment Panels that represent shading elements into the IES string representation for GEM format")]
        [Input("panelsAsShade", "The collection of BHoM Environment Panels that represent shading elements")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesSpace", "The IES string representation of shade for GEM")]
        public static List<string> ToIESShading(this List<Panel> panelsAsShade, SettingsIES settingsIES)
        {
            List<Panel> panels = panelsAsShade.Where(x => x.ExternalEdges.Count > 0).ToList();

            if (panels.Count != panelsAsShade.Count)
                BH.Engine.Base.Compute.RecordWarning("The panels for shading contain panels which did not contain geometry. Panels without valid geometry cannot be converted for IES to handle and have been ignored.");

            List<string> gemPanel = new List<string>();

            for (int x = 0; x < panels.Count; x++)
            {
                var template = Create.ShadeTemplate();
                template.Layer = "64";
                template.Colour = "0";

                gemPanel.AddRange(template.ToIES());

                gemPanel.Add("IES IES_SHD_" + (x + 1).ToString() + "\n");

                List<Point> points = panels[x].Vertices().Select(y => y.RoundCoordinates(settingsIES.DecimalPlaces)).ToList();
                points = points.Distinct().ToList();
                gemPanel.Add(points.Count.ToString() + " 1\n");

                string s = points.Count.ToString();

                foreach (Point p in points)
                {
                    gemPanel.Add(p.ToIES(settingsIES));
                    s += " " + (points.IndexOf(p) + 1).ToString();
                }
                s += "\n";
                gemPanel.Add(s);

                // Add Openings
                if (panels[x].Openings.Count == 0)
                    gemPanel.Add("0\n");
                else
                {
                    gemPanel.Add(panels[x].Openings.Count.ToString() + "\n");

                    foreach (Opening o in panels[x].Openings)
                        gemPanel.AddRange(o.ToIES(panels[x], settingsIES));
                }
            }

            return gemPanel;
        }

        [Description("Convert an IES string representation of a space into a collection of BHoM Environment Panels")]
        [Input("iesPanel", "The IES representation of a space.")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("panelsAsSpace", "BHoM Environment Space")]
        public static Panel FromIESShading(this List<string> iesPanel, SettingsIES settingsIES, PanelType panelType)
        {
            Panel panel = new Panel();

            int numCoordinates = System.Convert.ToInt32(iesPanel[1].Split(' ')[0]);

            List<string> iesPoints = new List<string>(); //Add the coordinate pts to a list
            for (int x = 0; x < numCoordinates; x++)
                iesPoints.Add(iesPanel[x + 2]);

            List<Point> bhomPoints = iesPoints.Select(x => x.FromIES(settingsIES)).ToList();

            int count = numCoordinates + 2; //Number of coordinates + 2 to get on to the line of the panel in GEM

            //Convert to panels
            List<string> panelCoord = iesPanel[count].Trim().Split(' ').ToList();
            List<Point> pLinePts = new List<Point>();
            for (int y = 1; y < panelCoord.Count; y++)
                pLinePts.Add(bhomPoints[System.Convert.ToInt32(panelCoord[y]) - 1]); //Add coordinate points in order

            pLinePts.Add(pLinePts.First()); //Add first point to close polyline

            Polyline pLine = new Polyline { ControlPoints = pLinePts, };

            panel.ExternalEdges = pLine.ToEdges();
            panel.Type = panelType;
            panel.Openings = new List<Opening>();

            //Add Openings
            count++;
            int numOpenings = System.Convert.ToInt32(iesPanel[count]); //Number of openings
            count++;
            int countOpenings = 0;
            while (countOpenings < numOpenings)
            {
                string openingData = iesPanel[count];
                int numCoords = System.Convert.ToInt32(openingData.Split(' ')[0]);
                count++;

                List<string> openingPts = new List<string>();
                for (int x = 0; x < numCoords; x++)
                    openingPts.Add(iesPanel[count + x]);

                //panel.Openings.Add(openingPts.FromIES(openingData.Split(' ')[1], settingsIES));

                count += numCoords;
                countOpenings++;
            }

            /*if (settingsIES.PullOpenings)
            {
                //This if-statement is a fix for the IES implementation of translucent shades, 
                // where these shades contain openings defined by 3d coordinates, which differs from how 
                // IES traditionally defines openings with 2d coordinates. 
                if (panel.Type != PanelType.TranslucentShade)
                {
                    for (int x = 0; x < panel.Openings.Count; x++)
                    {
                        panel.Openings[x] = panel.Openings[x].RepairOpening(panel, new List<Panel> { panel });
                    }
                }
            }
            else
            {
                panel.Openings = new List<Opening>();
            }*/

            return panel;
        }
    }
}


