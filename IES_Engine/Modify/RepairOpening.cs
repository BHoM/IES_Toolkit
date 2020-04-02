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

namespace BH.Engine.IES
{
    public static partial class Modify
    {
        [Description("The openings when converted from IES do not have the right coordinates for 3D space, this method will repair them.")]
        [Input("opening", "The broken environment opening pulled from IES.")]
        [Input("host", "The host panel for the opening.")]
        [Input("panelsAsSpace", "A collection of panels defining the space around the opening.")]
        [Output("repairedOpening", "The repaired environment opening.")]
        public static Opening RepairOpening(this Opening opening, Panel host, /*Point panelBottomRightReference */List<Panel> panelAsSpace)
        {
          //  List<Point> vertices = host.Polyline().IDiscontinuityPoints(); // from start

            //List<string> gemOpening = new List<string>();

            Polyline openingCurve = opening.Polyline();

            Point panelBottomRightReference = host.BottomRight(panelAsSpace);

            Vector rotationVector = new Vector { X = 0, Y = 0, Z = -1 };  // negativ for att fa tillbaka!!
            if (openingCurve.ControlPoints.Max(x => x.Z) - openingCurve.ControlPoints.Min(x => x.Z) <= BH.oM.Geometry.Tolerance.Distance)
            {
                rotationVector = new Vector { X = -1, Y = 0, Z = 0, }; //Handle horizontal openings
                panelBottomRightReference = openingCurve.Bounds().Max;
            }

            Vector zVector = new Vector { X = 0, Y = -1, Z = 0 };  // negativ for att fa tillbaka!!
            Plane openingPlane = openingCurve.IFitPlane();
            Vector planeNormal = openingPlane.Normal;

            Point xyRefPoint = new Point { X = 0, Y = 0, Z = 0 };
            Vector translateVector = panelBottomRightReference - xyRefPoint;   //  andra ordning

            double rotationAngle = planeNormal.Angle(zVector);
            TransformMatrix rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

            Polyline openingTransformed = openingCurve.Transform(rotateMatrix);
            Polyline openingTranslated = openingTransformed.Translate(translateVector);

            //  List<Point> vertices = openingTranslated.IDiscontinuityPoints();

           // Polyline newPoly = new Polyline { ControlPoints = openingTranslated.IDiscontinuityPoints(), };  // from start

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = openingTranslated.ToEdges();

            return newOpening;
        }
    }
}