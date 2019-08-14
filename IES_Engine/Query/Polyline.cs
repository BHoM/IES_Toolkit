/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Bounding Box             ****/
        /***************************************************/

        public static Polyline ToPolyline(this BoundingBox box)
        {
            Point min = box.Min.RoundedPoint();
            Point max = box.Max.RoundedPoint();

            List<Point> pnts = new List<Point>();
            pnts.Add(min);

            /*if(min.X == max.X || min.Z == max.Z)
            {
                //YZ plane
                pnts.Add(new Point { X = min.X, Y = max.Y, Z = min.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = min.Y, Z = max.Z });
            }
            else if (min.Y == max.Y)
            {
                //XZ
                pnts.Add(new Point { X = min.X, Y = min.Y, Z = max.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = max.Y, Z = min.Z });
            }
            else
            {
                double x1 = min.X;
                double x2 = max.X;
                double y1 = min.Y;
                double y2 = max.Y;

                double xc = (x1 + x2) / 2;
                double xd = (x1 - x2) / 2;

                double yc = (y1 + y2) / 2;
                double yd = (y1 - y2) / 2;

                double x3 = xc - yd;
                double x4 = xc + yd;
                double y3 = yc + xd;
                double y4 = yc - xd;

                pnts.Add(new Point { X = x3, Y = y3, Z = min.Z });
                pnts.Add(max);
                pnts.Add(new Point { X = x4, Y = y4, Z = max.Z });
            }*/

            pnts.Add(new Point { X = min.X, Y = min.Y, Z = max.Z });
            pnts.Add(new Point { X = min.X, Y = max.Y, Z = max.Z });
            pnts.Add(new Point { X = min.X, Y = max.Y, Z = min.Z });

            pnts.Add(min);

            return new Polyline { ControlPoints = pnts };
        }
    }
}
