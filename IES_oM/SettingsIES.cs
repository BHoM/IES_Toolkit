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
using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.IES.Settings
{
    [Description("Create a SettingsIES object for use with the IES Adapter")]
    public class SettingsIES : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Set tolarance for planar surfaces, default is set to BH.oM.Geometry.Tolerance.Distance")]
        public virtual double PlanarTolerance { get; set; } = BH.oM.Geometry.Tolerance.Distance;

        [Description("Set how many decimal places coordinates should have on export, default is set to 6")]
        public virtual int DecimalPlaces { get; set; } = 6;

        /***************************************************/
    }
}
