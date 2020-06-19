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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Adapters.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Bounding Box             ****/
        /***************************************************/

        [Description("Get a 2d polyline representation of a BHoM BoundingBox")]
        [Input("box", "The BHoM bounding box")]
        [Output("polyline", "The 2d polyline representation of the box")]
        public static Polyline Polyline(this BoundingBox box)
        {
            Point min = box.Min.RoundedPoint();
            Point max = box.Max.RoundedPoint();

            double minDist = Math.Min(max.X - min.X, max.Y - min.Y);
            minDist = Math.Min(minDist, max.Z - min.Z);

            List<Point> pnts = new List<Point>();
            pnts.Add(min);

            if((max.X - min.X) == minDist || (max.Z - min.Z) == minDist)
            {
                //YZ plane
                double x1 = min.Z;
                double x2 = max.Z;
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

                pnts.Add(new Point { X = min.X, Y = y3, Z = x3 });
                pnts.Add(max);
                pnts.Add(new Point { X = max.X, Y = y4, Z = x4 });
            }
            else if ((max.Y - min.Y) == minDist)
            {
                //XZ
                double x1 = min.X;
                double x2 = max.X;
                double y1 = min.Z;
                double y2 = max.Z;

                double xc = (x1 + x2) / 2;
                double xd = (x1 - x2) / 2;

                double yc = (y1 + y2) / 2;
                double yd = (y1 - y2) / 2;

                double x3 = xc - yd;
                double x4 = xc + yd;
                double y3 = yc + xd;
                double y4 = yc - xd;

                pnts.Add(new Point { X = x3, Y = min.Y, Z = y3 });
                pnts.Add(max);
                pnts.Add(new Point { X = x4, Y = max.Y, Z = y4 });
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
            }

            pnts.Add(min);

            return new Polyline { ControlPoints = pnts };
        }
    }
}
