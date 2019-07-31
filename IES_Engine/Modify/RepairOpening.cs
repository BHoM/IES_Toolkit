using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Environment.Elements;

using BH.Engine.Environment;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.IES
{
    public static partial class Modify
    {
        public static Opening RepairOpening(this Opening opening, Point panelXYZ)
        {
            //The openings when converted from IES don't have the right coordinates for 3D space, this method will repair them
            Polyline pLine = opening.Polyline();
            List<Point> pts = pLine.ControlPoints;

            List<Point> newPts = new List<Point>();
            foreach(Point p in pts)
            {
                newPts.Add(new Point());
                newPts.Last().X += panelXYZ.X;
                newPts.Last().Y += panelXYZ.Y;
                newPts.Last().Z += panelXYZ.Z;
            }

            Polyline newPoly = new Polyline { ControlPoints = newPts, };

            Opening newOpening = opening.GetShallowClone(true) as Opening;
            newOpening.Edges = newPoly.ToEdges();

            return newOpening;
        }
    }
}