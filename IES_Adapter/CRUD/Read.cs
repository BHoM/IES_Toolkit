/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Geometry;
using BH.Engine;
using BH.oM.Environment;

using System.IO;
using BH.Engine.Adapters.IES;

using BH.oM.Adapter;
using BH.Engine.Adapter;
using BH.Engine.Environment;
using BH.Engine.Geometry;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList indices = null, ActionConfig actionConfig = null)
        {
            if (type == typeof(Space))
                return ReadSpaces();
            else if (type == typeof(Panel))
                return ReadPanels();

            return ReadFullGEM();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<IBHoMObject> ReadFullGEM()
        {
            List<Panel> panels = ReadPanels();
            List<Space> spaces = ReadSpaces(panels);

            List<IBHoMObject> objects = new List<IBHoMObject>();
            objects.AddRange(panels);
            objects.AddRange(spaces);

            return objects;
        }

        private List<Panel> ReadPanels()
        {
            if (!System.IO.File.Exists(_fileSettings.GetFullFileName()))
            {
                BH.Engine.Base.Compute.RecordError("File does not exist to pull from");
                return new List<Panel>();
            }

            StreamReader sr = new StreamReader(_fileSettings.GetFullFileName());

            List<string> iesStrings = new List<string>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
                iesStrings.Add(line);

            sr.Close();

            int linesToSkip = 10;

            if (iesStrings.First() != "LAYER") //Check if it is a 2019 GEM file
            {
                iesStrings.RemoveRange(0, 4);
                linesToSkip = 12;
            }

            List<Panel> panels = new List<Panel>();

            PanelType panelType = PanelType.Undefined;
            if (iesStrings[7] == "4")
                panelType = PanelType.Shade;
            else if (iesStrings[7] == "1" && iesStrings[9] == "2102")
            {
                panelType = PanelType.TranslucentShade;
                linesToSkip += 2;
            }

            iesStrings.RemoveRange(0, linesToSkip); //Remove the first 10 items...
            bool endOfFile = false;

            while (!endOfFile)
            {
                int nextIndex = iesStrings.IndexOf("LAYER");
                if (nextIndex == -1)
                {
                    nextIndex = iesStrings.Count; //End of the file
                    endOfFile = true;
                }

                List<string> space = new List<string>();
                for (int x = 0; x < nextIndex; x++)
                    space.Add(iesStrings[x]);

                if (panelType == PanelType.Shade || panelType == PanelType.TranslucentShade)
                    panels.Add(space.FromIESShading(_settingsIES, panelType)); //Make a shade panel
                else
                    panels.AddRange(space.FromIES(_settingsIES));

                if (!endOfFile)
                {
                    if (iesStrings[nextIndex + 7] == "4")
                        panelType = PanelType.Shade;
                    else if (iesStrings[nextIndex + 7] == "1" && iesStrings[nextIndex + 9] == "2102")
                        panelType = PanelType.TranslucentShade;
                    else
                        panelType = PanelType.Undefined;

                    iesStrings.RemoveRange(0, nextIndex + linesToSkip);
                }
            }

            return panels;
        }

        private IEnumerable<IBHoMObject> ReadSpaces()
        {
            _settingsIES.PullOpenings = false; //Override and not pull openings when pulling spaces
            return ReadSpaces(ReadPanels());
        }

        private List<Space> ReadSpaces(List<Panel> panels)
        {
            List<List<Panel>> panelsAsSpaces = panels.ToSpaces();

            List<Space> spaces = new List<Space>();
            foreach (List<Panel> space in panelsAsSpaces)
            {
                Polyline perim = space.FloorGeometry();
                Point centre = perim != null ? perim.Centroid() : null;

                if (perim == null)
                    BH.Engine.Base.Compute.RecordWarning("The space " + space.ConnectedSpaceName() + " did not return a valid floor geometry from its panels. The geometry is null but the space has been pulled. You may wish to investigate and fix manually.");

                spaces.Add(new Space
                {
                    Perimeter = perim,
                    Location = centre,
                    Name = space.ConnectedSpaceName(),
                });
            }

            return spaces;
        }
    }
}


