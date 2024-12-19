/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Environment.IES;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        [Description("Convert a BHoM Opening Type to an IES string representation for GEM format")]
        [Input("type", "The BHoM Opening Type to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesOpeningType", "The IES string representation of the BHoM opening type")]
        public static string ToIES(this OpeningType type, PushConfigIES settingsIES)
        {
            switch (type)
            {
                case OpeningType.CurtainWall:
                case OpeningType.Glazing:
                case OpeningType.Rooflight:
                case OpeningType.RooflightWithFrame:
                case OpeningType.Window:
                case OpeningType.WindowWithFrame:
                    return "0";
                case OpeningType.Door:
                case OpeningType.VehicleDoor:
                    return "1";
                case OpeningType.Hole:
                default:
                    return "2"; //Hole
            }
        }

        [Description("Convert an IES string representation of a Opening Type to a BHoM Opening Type")]
        [Input("iesOpeningType", "The IES string representation of an opening type")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("openingType", "The BHoM Opening Type")]
        public static OpeningType FromIESOpeningType(this string iesOpeningType, PullConfigIES settingsIES)
        {
            if (iesOpeningType == "0")
                return OpeningType.Window;
            if (iesOpeningType == "1")
                return OpeningType.Door;
            if (iesOpeningType == "2")
                return OpeningType.Hole;

            return OpeningType.Undefined;
        }
    }
}


