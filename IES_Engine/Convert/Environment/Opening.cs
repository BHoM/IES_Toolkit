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
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Reflection;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;

using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.IES
{
    public static partial class Convert
    {

        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format")]
        [Input("opening", "The BHoM Environment Opening to convert")]
        [Input("panelsAsSpace", "The panels representing a single space which hosts this opening, used to check the orientation of the opening")]
        [Input("panelBottomRightReference", "The bottom right corner point of the host panel to calculate the opening points from for GEM format")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesOpening", "The string representation for IES GEM format")]
        public static List<string> ToIES(this Opening opening, Panel hostPanel, List<Panel> panelsAsSpace, SettingsIES settingsIES)
        {
            List<string> gemOpening = new List<string>();

            Polyline openingCurve = opening.Polyline();
            Polyline hostCurve = hostPanel.Polyline();

            Point panelBottomRightReference = hostPanel.BottomRight(panelsAsSpace);
            Point panelBottomLeftReference = hostPanel.BottomLeft(panelsAsSpace);
            Point panelTopRightReference = hostPanel.TopRight(panelsAsSpace);

            Vector xVector = panelBottomLeftReference - panelBottomRightReference;
            Vector yVector = panelTopRightReference - panelBottomRightReference;

            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

            Polyline hostTransformed = hostCurve.Orient(localCartesian, worldCartesian);
            Polyline openingTranslated = openingCurve.Orient(localCartesian, worldCartesian);

            double minX = hostTransformed.ControlPoints.Select(x => x.X).Min();
            double minY = hostTransformed.ControlPoints.Select(x => x.Y).Min();
            if (minX < 0)
            {
                Vector translateVectorX = new Vector { X = -minX, Y = 0, Z = 0 };
                openingTranslated = openingTranslated.Translate(translateVectorX);
            }
            if (minY < 0)
            {
                Vector translateVectorY = new Vector { X = 0, Y = -minY, Z = 0 };
                openingTranslated = openingTranslated.Translate(translateVectorY);
            }

            List<Point> vertices = openingTranslated.IDiscontinuityPoints();
            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settingsIES) + "\n");

            foreach (Point p in vertices)
                gemOpening.Add(" " + Math.Abs(p.X).ToString() + " " + Math.Abs(p.Y).ToString() + "\n");

            /*Vector rotationVector = new Vector { X = 0, Y = 0, Z = 1 };
            if (openingCurve.ControlPoints.Max(x => x.Z) - openingCurve.ControlPoints.Min(x => x.Z) <= BH.oM.Geometry.Tolerance.Distance)
            {
                rotationVector = new Vector { X = 1, Y = 0, Z = 0, }; //Handle horizontal openings
                panelBottomRightReference = openingCurve.Bounds().Max;
            }

            Vector zVector = new Vector { X = 0, Y = 1, Z = 0 };
            Plane openingPlane = openingCurve.IFitPlane();
            Vector planeNormal = openingPlane.Normal;

            Point xyRefPoint = new Point {X =  0, Y = 0, Z = 0};
            Vector translateVector = xyRefPoint - panelBottomRightReference;

            double rotationAngle = planeNormal.Angle(zVector);
            TransformMatrix rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

            Polyline openingTransformed = openingCurve.Transform(rotateMatrix);
            Polyline openingTranslated = openingTransformed.Translate(translateVector);

            List<Point> vertices = openingTranslated.IDiscontinuityPoints();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settingsIES) + "\n");

            if ((vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) >= BH.oM.Geometry.Tolerance.Distance)
            {
                foreach (Point p in vertices)
                {
                    Vector direction = p.RoundedPoint() - xyRefPoint.RoundedPoint();
                    gemOpening.Add(" " + Math.Abs(direction.X).ToString() + " " + Math.Abs(direction.Y).ToString() + "\n");
                }
            }
            else
            {
                foreach (Point p in vertices)
                {
                    Vector direction = p.RoundedPoint() - xyRefPoint.RoundedPoint();
                    gemOpening.Add(" " + Math.Abs(direction.X).ToString() + " " + Math.Abs(direction.Z).ToString() + "\n");
                }
            }*/

            return gemOpening;
        }

        [Description("Convert an IES string representation of an opening to a BHoM Environment Opening")]
        [Input("openingPts", "The string representations of coordinates that make up the opening")]
        [Input("openingType", "The IES representation of the opening type")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("opening", "The BHoM Environment Opening converted from IES GEM format")]
        public static Opening FromIES(this List<string> openingPts, string openingType, SettingsIES settingsIES)
        {
            List<Point> points = openingPts.Select(x => x.FromIES(settingsIES)).ToList();
            points.Add(points.First());

            Polyline pLine = new Polyline { ControlPoints = points, };

            Opening opening = new Opening();
            opening.Edges = pLine.ToEdges();
            opening.Type = openingType.FromIESOpeningType(settingsIES);

            return opening;
        }
    }
}