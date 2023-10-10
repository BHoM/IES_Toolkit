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
using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using System.IO;

using System.Linq;

using BH.oM.Adapter;
using BH.Engine.Adapter;
using BH.oM.Environment.IES;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            PushConfigIES pushConfig = (PushConfigIES)actionConfig;
            if(pushConfig == null)
            {
                BH.Engine.Base.Compute.RecordError("Please provide a valid IES Push Config to push data to IES.");
                return false;
            }

            if(pushConfig.File == null)
            {
                BH.Engine.Base.Compute.RecordError("Please provide a valid file location to push IES data to.");
                return false;
            }

            string fullFilePath = pushConfig.File.GetFullFileName();
            if (!Path.HasExtension(fullFilePath) || Path.GetExtension(fullFilePath) != ".gem")
            {
                BH.Engine.Base.Compute.RecordError("File Name must contain a GEM file extension.");
                return false;
            }

            List<IBHoMObject> bhomObjects = objects.Select(x => (IBHoMObject)x).ToList();
            List<Panel> panels = bhomObjects.Panels();

            Output<List<Panel>, List<Panel>> filteredPanels = panels.FilterPanelsByType(new List<PanelType>() { PanelType.Shade, PanelType.TranslucentShade} );

            List<List<Panel>> panelsAsSpaces = filteredPanels.Item2.ToSpaces();
            List<Panel> panelsAsShade = filteredPanels.Item1;

            if (pushConfig.ShadesAs3D)
                panelsAsSpaces.AddRange(panelsAsShade.ToSpaces());

            StreamWriter sw = new StreamWriter(fullFilePath);

            sw.WriteLine("COM GEM data file exported by BHoM");
            sw.WriteLine("CAT"); //Lol - Default GEM files use ANT
            sw.WriteLine("SITE");
            sw.WriteLine("51.378  2.3648  0.000  0.000");

            try
            {
                foreach (List<Panel> space in panelsAsSpaces)
                {
                    List<string> output = space.ToIES(pushConfig);
                    foreach (string s in output)
                        sw.Write(s);
                }

                if (panelsAsShade.Count > 0 && !pushConfig.ShadesAs3D)
                    panelsAsShade.ToIESShading(pushConfig).ForEach(x => sw.Write(x));
            }
            catch(Exception e)
            {
                BH.Engine.Base.Compute.RecordError("An error occurred in exporting the IES GEM file. Error is: " + e.ToString());
            }

            sw.Close();

            return true;
        }
    }
}