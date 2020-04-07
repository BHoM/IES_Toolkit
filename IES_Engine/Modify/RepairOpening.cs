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

namespace BH.Engine.IES
{
    public static partial class Modify
    {
        [Description("The openings when converted from IES do not have the right coordinates for 3D space, this method will repair them.")]
        [Input("opening", "The broken environment opening pulled from IES.")]
        [Input("host", "The host panel for the opening.")]
        [Input("panelsAsSpace", "A collection of panels defining the space around the opening.")]
        [Output("repairedOpening", "The repaired environment opening.")]
        public static Opening RepairOpening(this Opening opening, Panel host, List<Panel> panelAsSpace)
        {
            Polyline openingCurve = opening.Polyline();
            Polyline hostCurve = host.Polyline();

            Point panelBottomRightReference = host.BottomRight(panelAsSpace);
            Point panelBottomLeftReference = host.BottomLeft(panelAsSpace);
            Point panelTopRightReference = host.TopRight(panelAsSpace);

            Vector xVector = panelBottomLeftReference - panelBottomRightReference;
            Vector yVector = panelTopRightReference - panelBottomRightReference;

            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

            Polyline hostTransformed = hostCurve.Orient(localCartesian, worldCartesian);
            Polyline openingTranslated = openingCurve.Clone();

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
    }
}