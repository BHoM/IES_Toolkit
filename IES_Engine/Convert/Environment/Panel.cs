/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
        [Input("panelsAsShade", "The collection of BHoM Environment Panels that represent shading elements")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesSpace", "The IES string representation of shade for GEM")]
        public static List<string> ToIESShading(this List<Panel> panelsAsShade, SettingsIES settingsIES)
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
                gemPanel.Add(points.Count.ToString() + " 1\n");

                string s = points.Count.ToString();

                foreach(Point p in points)
                {
                    gemPanel.Add(p.ToIES(settingsIES));
                    s += " " + (points.IndexOf(p) + 1).ToString();
                }
                s += "\n";
                gemPanel.Add(s);
                gemPanel.Add("0\n");
            }

            return gemPanel;
        }
    }
}