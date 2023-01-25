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
        [Description("Convert a BHoM Geometry Point into an IES string representation for GEM files")]
        [Input("pt", "BHoM Geometry Point to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesPt", "The IES string representation of the point")]
        public static string ToIES(this Point pt, SettingsIES settingsIES, int num = 3)
        {
            if (num == 3)
                return " " + Math.Round(pt.X, settingsIES.DecimalPlaces).ToString() + " " + Math.Round(pt.Y, settingsIES.DecimalPlaces).ToString() + " " + Math.Round(pt.Z, settingsIES.DecimalPlaces).ToString() + "\n";
            else
                return " " + Math.Round(pt.X, settingsIES.DecimalPlaces).ToString() + " " + Math.Round(pt.Y, settingsIES.DecimalPlaces).ToString() + "\n";
        }

        [Description("Convert an IES point representation to a BHoM point")]
        [Input("iesPt", "The IES string representation of a point to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("point", "A BHoM Geometry Point")]
        public static Point FromIES(this string iesPt, SettingsIES settingsIES)
        {
            try
            {
                iesPt = iesPt.Trim();
                string[] split = System.Text.RegularExpressions.Regex.Split(iesPt, @"\s+");
                return new Point
                {
                    X = System.Convert.ToDouble(split[0]),
                    Y = System.Convert.ToDouble(split[1]),
                    Z = (split.Length > 2 ? System.Convert.ToDouble(split[2]) : 0),
                };
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError("An error occurred in parsing that IES string to a BHoM point. Error was: " + e.ToString());
                return null;
            }
        }
    }
}


