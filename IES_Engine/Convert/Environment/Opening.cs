///*
// * This file is part of the Buildings and Habitats object Model (BHoM)
// * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
// *
// * Each contributor holds copyright over their respective contributions.
// * The project versioning (Git) records all such contribution source information.
// *                                           
// *                                                                              
// * The BHoM is free software: you can redistribute it and/or modify         
// * it under the terms of the GNU Lesser General Public License as published by  
// * the Free Software Foundation, either version 3.0 of the License, or          
// * (at your option) any later version.                                          
// *                                                                              
// * The BHoM is distributed in the hope that it will be useful,              
// * but WITHOUT ANY WARRANTY; without even the implied warranty of               
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
// * GNU Lesser General Public License for more details.                          
// *                                                                            
// * You should have received a copy of the GNU Lesser General Public License     
// * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using BH.oM.Geometry;
//using BH.oM.Base.Attributes;
//using System.ComponentModel;
//using BH.oM.Environment.Elements;
//using BH.Engine.Environment;
//using BH.Engine.Geometry;
//using BH.oM.IES.Settings;

//using BH.oM.Geometry.CoordinateSystem;

//namespace BH.Engine.Adapters.IES
//{
//    public static partial class Convert
//    {

//        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format - this is for vertical openings (openings on walls)")]
//        [Input("opening", "The BHoM Environment Opening to convert")]
//        [Input("panelsAsSpace", "The panels representing a single space which hosts this opening, used to check the orientation of the opening")]
//        [Input("settingsIES", "The IES settings to use with the IES adapter")]
//        [Output("iesOpening", "The string representation for IES GEM format")]
//        public static List<string> ToIES(this Opening opening, Panel hostPanel, List<Panel> panelsAsSpace, SettingsIES settingsIES)
//        {
//            List<string> gemOpening = new List<string>();

//            Polyline openingCurve = opening.Polyline();
//            Polyline hostCurve = hostPanel.Polyline();

//            if (hostCurve.ControlPoints.Select(x => x.Z).Max() == hostCurve.ControlPoints.Select(x => x.Z).Min())
//                return opening.ToIES(hostPanel, settingsIES); //Horizontal openings are handled slightly differently

//            Point panelBottomRightReference = hostPanel.BottomRight(panelsAsSpace);
//            Point panelBottomLeftReference = hostPanel.BottomLeft(panelsAsSpace);
//            Point panelTopRightReference = hostPanel.TopRight(panelsAsSpace);

//            Vector xVector = panelBottomLeftReference - panelBottomRightReference;
//            xVector.Z = 0;
//            Vector yVector = panelTopRightReference - panelBottomRightReference;

//            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
//            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
//            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(panelBottomRightReference, xVector, yVector);

//            Polyline hostTransformed = hostCurve.Orient(localCartesian, worldCartesian);
//            Polyline openingTranslated = openingCurve.Orient(localCartesian, worldCartesian);

//            //If the orientation to 0,0,0 returns a negative X or Y point, translate the opening appropriately so that the bottom right reference would (if we wanted it) become 0,0,0 of the bounds of the host panel
//            double minX = hostTransformed.ControlPoints.Select(x => x.X).Min();
//            double minY = hostTransformed.ControlPoints.Select(x => x.Y).Min();
//            if (minX < 0)
//            {
//                Vector translateVectorX = new Vector { X = -minX, Y = 0, Z = 0 };
//                openingTranslated = openingTranslated.Translate(translateVectorX);
//            }
//            if (minY < 0)
//            {
//                Vector translateVectorY = new Vector { X = 0, Y = -minY, Z = 0 };
//                openingTranslated = openingTranslated.Translate(translateVectorY);
//            }

//            List<Point> vertices = openingTranslated.IDiscontinuityPoints();
//            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settingsIES) + "\n");

//            foreach (Point p in vertices)
//                gemOpening.Add(" " + Math.Abs(Math.Round(p.X, settingsIES.DecimalPlaces)).ToString() + " " + Math.Abs(Math.Round(p.Y, settingsIES.DecimalPlaces)).ToString() + "\n");

//            return gemOpening;
//        }

//        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format - this is for horizontal openings (openings on roofs and floors)")]
//        [Input("opening", "The BHoM Environment Opening to convert")]
//        [Input("settingsIES", "The IES settings to use with the IES adapter")]
//        [Output("iesOpening", "The string representation for IES GEM format")]
//        public static List<string> ToIES(this Opening opening, Panel hostPanel, SettingsIES settingsIES)
//        {
//            List<string> gemOpening = new List<string>();

//            Point zeroReference = null;
//            BoundingBox bounds = hostPanel.Bounds();
//            Vector xVector = new Vector { X = -1, Y = 0, Z = 0 };
//            Vector yVector = new Vector { X = 0, Y = 1, Z = 0 };

//            if (hostPanel.Type == PanelType.Floor || hostPanel.Type == PanelType.FloorExposed || hostPanel.Type == PanelType.FloorRaised)
//                zeroReference = new Point { X = bounds.Max.X, Y = bounds.Min.Y, Z = bounds.Min.Z };
//            else
//            {
//                zeroReference = new Point { X = bounds.Max.X, Y = bounds.Max.Y, Z = bounds.Max.Z };
//                yVector.Y = -1;
//            }

//            Polyline openingCurve = opening.Polyline();

//            Point worldOrigin = new Point { X = 0, Y = 0, Z = 0 };
//            Cartesian worldCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(worldOrigin, Vector.XAxis, Vector.YAxis);
//            Cartesian localCartesian = BH.Engine.Geometry.Create.CartesianCoordinateSystem(zeroReference, xVector, yVector);

//            Polyline openingTranslated = openingCurve.Orient(localCartesian, worldCartesian);

//            List<Point> vertices = openingTranslated.IDiscontinuityPoints();
//            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settingsIES) + "\n");

//            foreach (Point p in vertices)
//                gemOpening.Add(" " + Math.Abs(p.X).ToString() + " " + Math.Abs(p.Y).ToString() + "\n");

//            return gemOpening;
//        }

//        [Description("Convert an IES string representation of an opening to a BHoM Environment Opening")]
//        [Input("openingPts", "The string representations of coordinates that make up the opening")]
//        [Input("openingType", "The IES representation of the opening type")]
//        [Input("settingsIES", "The IES settings to use with the IES adapter")]
//        [Output("opening", "The BHoM Environment Opening converted from IES GEM format")]
//        public static Opening FromIES(this List<string> openingPts, string openingType, SettingsIES settingsIES)
//        {
//            List<Point> points = openingPts.Select(x => x.FromIES(settingsIES)).ToList();
//            points.Add(points.First());

//            Polyline pLine = new Polyline { ControlPoints = points, };

//            Opening opening = new Opening();
//            opening.Edges = pLine.ToEdges();
//            opening.Type = openingType.FromIESOpeningType(settingsIES);

//            return opening;
//        }
//    }
//}


