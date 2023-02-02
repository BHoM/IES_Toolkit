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
using BH.Engine.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        public static List<string> ToIES(this Opening opening, Panel hostPanel, SettingsIES settingsIES)
        {
            List<string> rtn = new List<string>();

            var coordSystem = hostPanel.Polyline().CoordinateSystem();
            var localToGlobal = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(coordSystem);

            var polyline = opening.Polyline().Transform(localToGlobal);

            rtn.Add($"{polyline.ControlPoints.Count.ToString()} {opening.Type.ToIES(settingsIES)}\n");

            foreach (var cPoint in polyline.ControlPoints)
                rtn.Add($" {cPoint.ToIES(settingsIES, false)}");

            return rtn;
        }
        public static Opening FromIESOpening(this List<string> openingIES, string openingType, SettingsIES settingsIES )
        {

            var polyline = new Polyline();

            foreach (var iesPt in openingIES)
                polyline.ControlPoints.Add(iesPt.FromIES(settingsIES));

            var coordSystem = polyline.CoordinateSystem();
            var globalToLocal = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(coordSystem);

            Opening opening = new Opening();

            opening.Edges = polyline.Transform(globalToLocal).ToEdges();
            opening.Type = openingType.FromIESOpeningType(settingsIES);

            return opening;
        }
    }
}
