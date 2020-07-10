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

using BH.oM.Geometry;
using BH.oM.Environment.Elements;

using BH.Engine.Environment;
using BH.Engine.Geometry;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Adapters.IES
{
    public static partial class Modify
    {
        [Description("The openings when converted from IES do not have the right coordinates for 3D space, this method will repair them.")]
        [Input("opening", "The broken environment opening pulled from IES.")]
        [Input("host", "The host panel for the opening.")]
        [Input("panelsAsSpace", "A collection of panels defining the space around the opening.")]
        [Output("repairedOpening", "The repaired environment opening.")]
        public static Opening RepairOpening(this Opening opening, Panel host, List<Panel> panelsAsSpace)
        {
            Polyline openingCurve = opening.Polyline();
            Polyline hostCurve = host.Polyline();

            if (hostCurve.ControlPoints.Select(x => x.Z).Max() == hostCurve.ControlPoints.Select(x => x.Z).Min())
            {
                //Horizontal openings are handled slightly differently
                if (panelsAsSpace.Select(x => x.Polyline().ControlPoints.Select(y => y.Z).Max()).Max() == hostCurve.ControlPoints.Select(x => x.Z).Max())
                    return opening.RepairOpening(host, PanelType.Roof); //If the maximum Z level of the space is equal to the Z level of this panel
                else
                    return opening.RepairOpening(host, PanelType.Floor);
            }

            Point panelBottomRightReference = host.BottomRight(panelsAsSpace);
            Point panelBottomLeftReference = host.BottomLeft(panelsAsSpace);
            Point panelTopRightReference = host.TopRight(panelsAsSpace);

            if(panelBottomRightReference == null || panelBottomLeftReference == null || panelTopRightReference == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("An error occurred in attempting to repair opening with GUID " + opening.BHoM_Guid + " hosted by the panel with GUID " + host.BHoM_Guid + " . The opening on this panel may not be correctly pulled and should be investigated.");
                return opening;
            }

            Vector xVector = panelBottomLeftReference - panelBottomRightReference;
            xVector.Z = 0;
            Vector yVector = panelTopRightReference - panelBottomRightReference;

            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

            Polyline hostTransformed = hostCurve.Orient(localCartesian, worldCartesian);
            Polyline openingTranslated = openingCurve.Clone();

            //If the orientation to 0,0,0 returns a negative X or Y point, translate the opening appropriately so that the bottom right reference would (if we wanted it) become 0,0,0 of the bounds of the host panel
            double minX = hostTransformed.ControlPoints.Select(x => x.X).Min();
            double minY = hostTransformed.ControlPoints.Select(x => x.Y).Min();
            if (minX < 0)
            {
                Vector translateVectorX = new Vector { X = minX, Y = 0, Z = 0 };
                openingTranslated = openingTranslated.Translate(translateVectorX);
            }
            if(minY < 0)
            {
                Vector translateVectorY = new Vector { X = 0, Y = minY, Z = 0 };
                openingTranslated = openingTranslated.Translate(translateVectorY);
            }

            Polyline openingTransformed = openingTranslated.Orient(worldCartesian, localCartesian);

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = openingTransformed.ToEdges();

            return newOpening;
        }

        [Description("The openings when converted from IES do not have the right coordinates for 3D space, this method will repair them. This method is for horizontal openings (openings on roofs and floors)")]
        [Input("opening", "The broken environment opening pulled from IES.")]
        [Input("hostPanel", "The host panel for the opening.")]
        [Input("hostType", "Determines whether the host panel is the floor of the space it is part of, or the ceiling.")]
        [Output("repairedOpening", "The repaired environment opening.")]
        public static Opening RepairOpening(this Opening opening, Panel hostPanel, PanelType hostType)
        {
            Point zeroReference = null;
            BoundingBox bounds = hostPanel.Bounds();
            Vector xVector = new Vector { X = -1, Y = 0, Z = 0 };
            Vector yVector = new Vector { X = 0, Y = 1, Z = 0 };

            if (hostType == PanelType.Floor || hostType == PanelType.FloorExposed || hostType == PanelType.FloorRaised)
                zeroReference = new Point { X = bounds.Max.X, Y = bounds.Min.Y, Z = bounds.Min.Z };
            else
            {
                zeroReference = new Point { X = bounds.Max.X, Y = bounds.Max.Y, Z = bounds.Max.Z };
                yVector.Y = -1;
            }

            Polyline openingCurve = opening.Polyline();

            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(zeroReference, xVector, yVector);

            Polyline openingTranslated = openingCurve.Orient(worldCartesian, localCartesian);

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = openingTranslated.ToEdges();

            return newOpening;
        }
    }
}