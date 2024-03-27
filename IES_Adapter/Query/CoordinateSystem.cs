/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Geometry;

namespace BH.Adapter.IES
{
    public static partial class Query
    {
        public static Cartesian CoordinateSystem(this Polyline pLine, double distanceTolerance = Tolerance.MacroDistance, double angleTolerance = Tolerance.Angle)
        {
            Vector locZ = pLine.Normal(distanceTolerance);
            
            Cartesian baseCartesiean;
            
            if (locZ.IsParallel(Vector.ZAxis, angleTolerance) != 0)
                baseCartesiean = new Cartesian(pLine.ControlPoints[0], -Vector.XAxis, locZ.CrossProduct(-Vector.XAxis), locZ);
            else
            {
                Vector locY = (Vector.ZAxis - Vector.ZAxis.DotProduct(locZ) * locZ).Normalise();
                Vector locX = locY.CrossProduct(locZ);
                baseCartesiean = new Cartesian(pLine.ControlPoints[0], locX, locY, locZ);
            }

            TransformMatrix locGlob1 = BH.Engine.Geometry.Create.OrientationMatrixLocalToGlobal(baseCartesiean);
            Point minPtLocal = pLine.ControlPoints.Select(x => x.Transform(locGlob1)).ToList().Bounds().Min;
            TransformMatrix globToLoc = BH.Engine.Geometry.Create.OrientationMatrixGlobalToLocal(baseCartesiean);
            baseCartesiean.Origin = minPtLocal.Transform(globToLoc);
            
            return baseCartesiean;
        }
    }
}

