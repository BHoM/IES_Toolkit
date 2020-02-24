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
        public static Opening RepairOpening(this Opening opening, Panel host, List<Panel> panelsAsSpace)
        {
            List<Point> vertices = host.Polyline().IDiscontinuityPoints();
            bool useYZ = false;
            bool useXZ = false;
            if (vertices.Min(y => y.X) == vertices.Max(y => y.X))
                useYZ = true;
            else if (vertices.Min(y => y.Y) == vertices.Max(y => y.Y))
                useXZ = true;

            Point panelXYZ = host.BottomRight(panelsAsSpace);
            Point right = host.TopRight(panelsAsSpace);

            bool increaseX = (panelXYZ.X <= right.X);
            bool increaseY = (panelXYZ.Y <= right.Y);
            bool increaseZ = (panelXYZ.Z <= right.Z);

            //The openings when converted from IES don't have the right coordinates for 3D space, this method will repair them
            Polyline pLine = opening.Polyline();
            List<Point> pts = pLine.ControlPoints;

            List<Point> newPts = new List<Point>();
            foreach(Point p in pts)
            {
                newPts.Add(new Point());
                if (!useXZ && !useYZ)
                {
                    newPts.Last().X = (increaseX ? p.X + panelXYZ.X : Math.Abs(p.X - panelXYZ.X));
                    newPts.Last().Y = (increaseY ? p.Y + panelXYZ.Y : Math.Abs(p.Y - panelXYZ.Y));
                    newPts.Last().Z = panelXYZ.Z;
                }
                else if (useXZ)
                {
                    newPts.Last().X = (increaseX ? p.X + panelXYZ.X : Math.Abs(p.X - panelXYZ.X));
                    newPts.Last().Y = panelXYZ.Y;
                    newPts.Last().Z = (increaseZ ? p.Y + panelXYZ.Z : Math.Abs(p.Y - panelXYZ.Z));
                }
                else if (useYZ)
                {
                    newPts.Last().X = panelXYZ.X;
                    newPts.Last().Y = (increaseY ? p.X + panelXYZ.Y : Math.Abs(p.X - panelXYZ.Y));
                    newPts.Last().Z = (increaseZ ? p.Y + panelXYZ.Z : Math.Abs(p.Y - panelXYZ.Z));
                }
            }

            Polyline newPoly = new Polyline { ControlPoints = newPts, };

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = newPoly.ToEdges();

            return newOpening;
        }
    }
}