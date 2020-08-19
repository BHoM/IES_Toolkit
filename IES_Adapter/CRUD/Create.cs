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
using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using System.IO;

using System.Linq;
using BH.Engine.Adapters.IES;

using BH.oM.Adapter;
using BH.Engine.Adapter;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            List<IBHoMObject> bhomObjects = objects.Select(x => (IBHoMObject)x).ToList();
            List<Panel> panels = bhomObjects.Panels();

            List<List<Panel>> panelsAsSpaces = panels.ToSpaces();
            List<Panel> panelsAsShade = panels.FilterPanelsByType(PanelType.Shade).Item1;

            StreamWriter sw = new StreamWriter(_fileSettings.GetFullFileName());

            try
            {
                foreach (List<Panel> space in panelsAsSpaces)
                {
                    List<string> output = space.ToIES(_settingsIES);
                    foreach (string s in output)
                        sw.Write(s);
                }

                panelsAsShade.ToIESShading(_settingsIES).ForEach(x => sw.Write(x));
            }
            catch(Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError("An error occurred in exporting the IES GEM file. Error is: " + e.ToString());
            }

            sw.Close();

            return true;
        }

        /***************************************************/

    }
}
