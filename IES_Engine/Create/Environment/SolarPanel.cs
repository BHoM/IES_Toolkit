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
using System.Data;
using System.Runtime.InteropServices;

namespace BH.Engine.Adapters.IES
{
    public static partial class Convert
    {
        [Description("asdf")]
        [Input("settingsIES", "asdf")]
        [Output("iesSpace", "asdf")]
        public static string ToIESSolarPanel(this Point basePoint, int index, double width = 1, double height = 1, double tilt = 20, double orientation = 180)
        {
            string indexString = index.ToString().PadLeft(6, '0');
            indexString = $"PVP PV Panel [PV{indexString}]";

            string paramString = string.Join(" ", new List<string>
            {
                Math.Round(basePoint.X, 6).ToString(),
                Math.Round(basePoint.Y, 6).ToString(),
                Math.Round(basePoint.Z, 6).ToString(),
                Math.Round(width,6).ToString(),
                Math.Round(height,6).ToString(),
                Math.Round(orientation,6).ToString(),
                Math.Round(tilt,6).ToString(),
            });

            string gemPanel = string.Join("\n",
                new List<string>()
                    {
                        "LAYER",
                        "1",
                        "COLOUR",
                        "0",
                        "CATEGORY",
                        "3",
                        "TYPE",
                        "202",
                        "SUBTYPE",
                        "0",
                        "COLOURRGB",
                        "32767",
                        indexString,
                        paramString,
                    });

            return gemPanel;
        }

        [Description("asdf")]
        [Input("settingsIES", "asdf")]
        [Output("iesSpace", "asdf")]
        public static List<string> ToIESSolarPanels(this List<Point> basePoints, int startIndex, double width = 1, double height = 1, double tilt = 20, double orientation = 180)
        {
            List<string> gemPanels = new List<string>();

            for (int i = 0; i < basePoints.Count; i++)
            {
                gemPanels.Add(ToIESSolarPanel(basePoints[i], startIndex + i, width, height, tilt, orientation));
            }

            return gemPanels;
        }

    }
}


