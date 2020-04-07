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

            double degrees = BH.Engine.Geometry.Query.Angle(panelBottomLeftReference, panelBottomRightReference, panelTopRightReference);
            degrees = BH.Engine.Environment.Convert.ToDegrees(degrees);

            if(degrees > 90)
            {
                //This means something need to work out what
                if (panelBottomRightReference.X == panelBottomLeftReference.X)
                    panelBottomRightReference.X = panelTopRightReference.X;
                else
                    panelBottomRightReference.X = panelBottomLeftReference.X;
            }

            /*double minX = hostCurve.ControlPoints.Select(a => a.X).Min();
            if (minX < panelBottomRightReference.X)
            {
                Vector translateVectorX = new Vector { X = Math.Min(panelBottomRightReference.X, minX) - Math.Max(panelBottomRightReference.X, minX), Y = 0, Z = 0 };
                panelBottomRightReference = panelBottomRightReference.Translate(translateVectorX);
            }*/

            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

            TransformMatrix transformMatrix = BH.Engine.Geometry.Create.OrientationMatrixGlobalToLocal(localCartesian);

            Polyline openingTransformed = openingCurve.Transform(transformMatrix);

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = openingTransformed.ToEdges();

            return newOpening;
            /* Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

             TransformMatrix transformMatrix = BH.Engine.Geometry.Create.OrientationMatrixGlobalToLocal(localCartesian);

             hostCurve = hostCurve.Transform(transformMatrix);
             double minX = hostCurve.ControlPoints.Select(a => a.X).Min();
             if (minX < 0)
             {
                 Vector translateVectorX = new Vector { X = minX, Y = 0, Z = 0 };
                 panelBottomRightReference =panelBottomRightReference.Translate(translateVectorX);
             }

             double minY = hostCurve.ControlPoints.Select(a => a.Y).Min();
             if (minY < 0)
             {
                 Vector translateVectorY = new Vector { X = 0, Y = minY, Z = 0 };
                 panelBottomRightReference = panelBottomRightReference.Translate(translateVectorY);
             }

             localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

             transformMatrix = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(localCartesian);

             Polyline openingTransformed = openingCurve.Transform(transformMatrix);

             Opening newOpening = opening.GetShallowClone(true) as Opening;
             newOpening.Edges = openingTransformed.ToEdges();

             return newOpening; */


            /* Vector rotationVector = new Vector { X = 0, Y = 0, Z = -1 };  // negativ for att fa tillbaka!!
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
             newOpening.Edges = openingTranslated.ToEdges();   */
        }
    }
}