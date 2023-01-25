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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;
using BH.oM.IES.Settings;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Adapters.IES
{
    public static partial class Convert
    {
        public static Point LowerLeftCorner(this Panel panel)
        {
            var pline = panel.Polyline();
            var refPlane = panel.UpperOrientatedPlane();

            var coords = pline.ControlPoints;
            coords = coords.Select(x => x.XyzToXy(refPlane)).ToList();

            var minX = coords.Min(pt => pt.X);
            var minY = coords.Min(pt => pt.Y);
            return new Point() { X = minX, Y = minY, Z = 0 }.XyToXyz(refPlane);
        }

        public static Point UpperRightCorner(this Panel panel)
        {
            var pline = panel.Polyline();
            var refPlane = panel.UpperOrientatedPlane();

            var coords = pline.ControlPoints;
            coords = coords.Select(x => x.XyzToXy(refPlane)).ToList();

            var maxX = coords.Max(pt => pt.X);
            var maxY = coords.Max(pt => pt.Y);
            return new Point() { X = maxX, Y = maxY, Z = 0 }.XyToXyz(refPlane);
        }

        public static Cartesian FlipPlane(this Cartesian refPlane)
        {
            return new Cartesian(
                refPlane.Origin,
                refPlane.X,
                -refPlane.Y,
                -refPlane.Z
            );
        }

        public static Cartesian UpperOrientatedPlane(this Panel hostPanel)
        {
            Polyline boundary = hostPanel.Polyline();
            Plane plane = boundary.FitPlane();

            Vector localX = (boundary.ControlPoints[1] - boundary.ControlPoints[0]).Normalise();

            Vector localY = plane.Normal.CrossProduct(localX);

            return new Cartesian(plane.Origin, localX, localY, plane.Normal);
        }

        /*public static Cartesian UpperOrientatedPlane(this Panel panel)
        {
            var plane = panel.Polyline().FitPlane();

            if (plane.Normal.Z == 1 || plane.Normal.Z == -1)
                return new Cartesian(plane.Origin, Vector.XAxis, Vector.YAxis, Vector.ZAxis);
            else
            {
                var projY = Vector.ZAxis.Project(plane.Normal);
                var projX = projY.Rotate(Math.PI / -2, plane.Normal);
                return new Cartesian(plane.Origin, projX, Vector.YAxis, Vector.ZAxis);
            }
        }*/

        public static Polyline PolygonInFace(this Opening opening, Panel hostPanel, Point origin, bool flip)
        {
            BH.oM.Geometry.CoordinateSystem.Cartesian coordinateSystem = hostPanel.UpperOrientatedPlane();

            if (flip)
                coordinateSystem = coordinateSystem.FlipPlane();

            if(origin != null)
                coordinateSystem.Origin = origin;

            TransformMatrix transformation = Engine.Geometry.Create.OrientationMatrix(coordinateSystem, new Cartesian());
            return Engine.Geometry.Create.Polyline(opening.Polyline().ControlPoints.Select(x => x.Transform(transformation)));


           /* if (origin == null && flip)
                coordinateSystem = coordinateSystem.FlipPlane();
            else
            {
                if (coordinateSystem.Z.IsParallel(Vector.ZAxis) != 0) //Parallel to Z
                {
                    Vector localX = flip ? -Vector.XAxis : Vector.XAxis;
                    Vector localY = coordinateSystem.Z.CrossProduct(localX);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(origin, localX, localY);
                }
                else
                {
                    Vector projY = Vector.ZAxis.Project(coordinateSystem.Z);
                    Vector projX = projY.Rotate(-Math.PI / 2, coordinateSystem.Z);
                    coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(origin, projX, projY);
                }
            }

            var vertices = opening.Polyline().ControlPoints;
            var pts2D = vertices.Select(x => x.XyzToXy(coordinateSystem)).ToList();

            return Engine.Geometry.Create.Polyline(pts2D);*/
        }

        public static Point XyzToXy(this Point pt, Cartesian refPlane)
        {
            var diff = new Vector()
            {
                X = pt.X - refPlane.Origin.X,
                Y = pt.Y - refPlane.Origin.Y,
                Z = pt.Z - refPlane.Origin.Z,
            };

            return new Point()
            {
                X = refPlane.X.DotProduct(diff),
                Y = refPlane.Y.DotProduct(diff),
                Z = 0
            };
        }

        public static Point XyToXyz(this Point pt, Cartesian refPlane)
        {
            var u = new Vector()
            {
                X = refPlane.X.X * pt.X,
                Y = refPlane.X.Y * pt.X,
                Z = refPlane.X.Z * pt.X,
            };

            var v = new Vector()
            {
                X = refPlane.Y.X * pt.Y,
                Y = refPlane.Y.Y * pt.Y,
                Z = refPlane.Y.Z * pt.Y,
            };

            return new Point()
            {
                X = refPlane.Origin.X + u.X + v.X,
                Y = refPlane.Origin.Y + u.Y + v.Y,
                Z = refPlane.Origin.Z + u.Z + v.Z,
            };
        }


        [Description("Convert a BHoM Opening Type to an IES string representation for GEM format")]
        [Input("type", "The BHoM Opening Type to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesOpeningType", "The IES string representation of the BHoM opening type")]
        public static string ToIES(this OpeningType type, SettingsIES settingsIES)
        {
            switch(type)
            {
                case OpeningType.CurtainWall:
                case OpeningType.Glazing:
                case OpeningType.Rooflight:
                case OpeningType.RooflightWithFrame:
                case OpeningType.Window:
                case OpeningType.WindowWithFrame:
                    return "0";
                case OpeningType.Door:
                case OpeningType.VehicleDoor:
                    return "1";
                case OpeningType.Hole:
                default:
                    return "2"; //Hole
            }
        }

        [Description("Convert an IES string representation of a Opening Type to a BHoM Opening Type")]
        [Input("iesOpeningType", "The IES string representation of an opening type")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("openingType", "The BHoM Opening Type")]
        public static OpeningType FromIESOpeningType(this string iesOpeningType, SettingsIES settingsIES)
        {
            if (iesOpeningType == "0")
                return OpeningType.Window;
            if (iesOpeningType == "1")
                return OpeningType.Door;
            if (iesOpeningType == "2")
                return OpeningType.Hole;

            return OpeningType.Undefined;
        }
    }
}


