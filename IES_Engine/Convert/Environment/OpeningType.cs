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
        public static List<string> OpeningToIES(this Opening opening, Panel hostPanel)
        {
            var panelNormal = hostPanel.Polyline().ControlPoints.FitPlane2().Normal;
            var origin = new Point();
            var flip = false;

            if (panelNormal.Z is (-1|1))
            {
                origin = hostPanel.UpperRightCorner();
                flip = true;
            }
            /*else if (panelNormal.Angle())
            {

            }*/
            else
            {
                origin = hostPanel.LowerLeftCorner();
                flip = false;
            }

            var verts_2d = opening.PolygonInFace(hostPanel,origin, flip);

            List<string> fuckingHell = new List<string>();

            fuckingHell.Add($"{verts_2d.ControlPoints.Count} 0\n");

            foreach (var hell in verts_2d.ControlPoints)
                fuckingHell.Add($"{(hell.ToIES(new oM.IES.Settings.SettingsIES(), 2))}\n");

            return fuckingHell;
        }

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
        /*
                public static Cartesian UpperOrientatedPlane(this Panel hostPanel)
                {
                    Polyline boundary = hostPanel.Polyline();
                    Plane plane = boundary.FitPlane();

                    Vector localX = (boundary.ControlPoints[1] - boundary.ControlPoints[0]).Normalise();

                    Vector localY = plane.Normal.CrossProduct(localX);

                    return new Cartesian(plane.Origin, localX, localY, plane.Normal);
                }
                */
        public static Cartesian UpperOrientatedPlane(this Panel panel)
        {
            var plane = panel.Polyline().ControlPoints.FitPlane2();

            //if (plane.Normal.Z == 1 || plane.Normal.Z == -1)
            if (plane.Normal.IsParallel(Vector.ZAxis) != 0)
                return BH.Engine.Geometry.Create.CartesianCoordinateSystem(plane.Origin, Vector.XAxis, plane.Normal.CrossProduct(Vector.XAxis));// new Cartesian(plane.Origin, Vector.XAxis, Vector.YAxis, Vector.ZAxis);
            else
            {

                var projY = Vector.ZAxis.Project(plane);
                var projX = plane.Normal.CrossProduct(projY);
                return Engine.Geometry.Create.CartesianCoordinateSystem(plane.Origin, projX, projY);
            }
        }
        public static Plane FitPlane2(this List<Point> verts)
        {
            verts.RemoveAt(verts.Count - 1);
            var cprods = new List<Vector>();
            var baseVert = verts[0];
            for (int x = 0; x < verts.Count - 2; x++)
            {
                cprods.Add(NormalFrom3Pts(baseVert, verts[x + 1], verts[x + 2]));
            }

            var normal = new Vector() { X = 0, Y = 0, Z = 0 };
            foreach (var cprodx in cprods)
            {
                normal.X += cprodx.X;
                normal.Y += cprodx.Y;
                normal.Z += cprodx.Z;
            }
            //normalise the vector
            Vector normalVec = null;

            if ((normal.X != 0) || (normal.Y != 0) || (normal.Z != 0))
            {
                var ds = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
                normalVec = new Vector()
                {
                    X = normal.X / ds,
                    Y = normal.Y / ds,
                    Z = normal.Z / ds
                };
            }
            else
            {
                normalVec = new Vector()
                {
                    X = 0,
                    Y = 0,
                    Z = 1
                };
            }
            return new Plane()
            {
                Origin = verts[0],
                Normal = normalVec
            };
        }

        public static Vector NormalFrom3Pts(this Point pt1, Point pt2, Point pt3)
        {
            var v1 = new Vector
            {
                X = pt2.X - pt1.X,
                Y = pt2.Y - pt1.Y,
                Z = pt2.Z - pt1.Z
            };
            var v2 = new Vector
            {
                X = pt3.X - pt1.X,
                Y = pt3.Y - pt1.Y,
                Z = pt3.Z - pt1.Z
            };

            return new Vector
            {
                X = v1.Y * v2.Z - v1.Z * v2.Y,
                Y = -v1.X * v2.Z + v1.Z * v2.X,
                Z = v1.X * v2.Y - v1.Y * v2.X,
            };
        }

        public static Vector Rotate2(this Vector vec, double angle, Vector axis)
        {
            var x = vec.X; var y = vec.Y; var z = vec.Z;
            var u = axis.X; var v = axis.Y; var w = axis.Z;
            //Extracted common factors for simplicity and efficiency
            var r2 = Math.Pow(u, 2) + Math.Pow(v, 2) + Math.Pow(w, 2);
            var r = Math.Sqrt(r2);
            var ct = Math.Cos(angle);
            var st = Math.Sin(angle) / r;
            var dt = (u * x + v * y + w * z) * (1 - ct) / r2;

            return new Vector()
            {
                X = u * dt + x * ct + (-w * y + v * z) * st,
                Y = v * dt + y * ct + (w * x - u * z) * st,
                Z = w * dt + z * ct + (-v * x + u * y) * st,
            };
        }


        public static Polyline PolygonInFace(this Opening opening, Panel hostPanel, Point origin = null, bool flip = false)
        {
            BH.oM.Geometry.CoordinateSystem.Cartesian coordinateSystem = hostPanel.UpperOrientatedPlane();

            /*if (flip)
                coordinateSystem = coordinateSystem.FlipPlane();

            if(origin != null)
                coordinateSystem.Origin = origin;

            TransformMatrix transformation = Engine.Geometry.Create.OrientationMatrix(coordinateSystem, new Cartesian());
            return Engine.Geometry.Create.Polyline(opening.Polyline().ControlPoints.Select(x => x.Transform(transformation)));*/


            if (origin == null && flip)
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

            return Engine.Geometry.Create.Polyline(pts2D);
        }

        public static Point XyzToXy(this Point pt, Cartesian refPlane)
        {
            /*var diff = new Vector()
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
            };*/


            TransformMatrix m = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(refPlane);

            return pt.Transform(m);
        }

        public static Point XyToXyz(this Point pt, Cartesian refPlane)
        {
           /* var u = new Vector()
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
            };*/

            TransformMatrix m = BH.Engine.Geometry.Create.OrientationMatrixGlobalToLocal(refPlane);
            return pt.Transform(m);
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


