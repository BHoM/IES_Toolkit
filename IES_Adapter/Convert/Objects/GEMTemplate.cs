/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

            return gemLines;
        }
    }
}

