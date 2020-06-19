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

using BH.oM.IES.Settings;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Adapters.IES
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("3.0", "Deprecated to expose Decimal Places settings", null, "SettingsIES(double planarTolerance = BH.oM.Geometry.Distance.Tolerance, int decimalPlaces = 6")]
        [Input("planarTolerance", "Set tolarance for planar surfaces")]
        public static SettingsIES SettingsIES(double planarTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return SettingsIES(planarTolerance, 6);
        }

        [Description("Create a SettingsIES object for use with the IES Adapter")]
        [Input("planarTolerance", "Set tolarance for planar surfaces")]
        [Input("decimalPlaces", "Set how many decimal places coordinates should have on export")]
        [Output("settingsIES", "The IES settings to use with the IES adapter for pull and push")]
        [Deprecated("3.1", "Deprecated in favour of default create components produced by BHoM")]
        public static SettingsIES SettingsIES(double planarTolerance = BH.oM.Geometry.Tolerance.Distance, int decimalPlaces = 6)
        {
            return new SettingsIES
            {
                PlanarTolerance = planarTolerance,
                DecimalPlaces = decimalPlaces,
            };
        }            
    }
}
