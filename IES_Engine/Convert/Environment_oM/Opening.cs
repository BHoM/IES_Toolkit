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

namespace BH.Engine.IES
{
    public static partial class Convert
    {

        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format")]
        [Input("opening", "The BHoM Environment Opening to convert")]
        [Input("panelsAsSpace", "The panels representing a single space which hosts this opening, used to check the orientation of the opening")]
        [Input("panelXYZ", "The bottom left corner point of the host panel to calculate the opening points from for GEM format")]
        [Output("iesOpening", "The string representation for IES GEM format")]
        public static List<string> ToIES(this Opening opening, List<Panel> panelsAsSpace, Point panelBottomRightReference)
        {
            /*List<string> gemOpening = new List<string>();
            List<Point> vertices = opening.Polyline().IDiscontinuityPoints();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES() + "\n");
            */
            /*Polyline openingCurve = opening.Polyline();
            Vector zVector = new Vector { X = 0, Y = 1, Z = 0 };
            Plane openingPlane = openingCurve.IFitPlane();
            Vector planeNormal = openingPlane.Normal;

            Point xyRefPoint = new Point {X =  0, Y = 0, Z = 0};
            Vector translateVector = xyRefPoint - panelBottomRightReference;

            Vector rotationVector = planeNormal.CrossProduct(zVector).Normalise();
            double rotationAngle = planeNormal.Angle(zVector);
            TransformMatrix rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

            Polyline openingTransformed = openingCurve.Transform(rotateMatrix).Translate(translateVector);

            List<Point> vertices = openingTransformed.IDiscontinuityPoints();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES() + "\n");

            foreach(Point p in vertices)
            {
                //Vector direction = p.RoundedPoint() - panelBottomRightReference.RoundedPoint();
                gemOpening.Add(" " + p.RoundedPoint().X.ToString() + " " + p.RoundedPoint().Z.ToString() + "\n");
            }*/


            /*double minDist = Math.Min(vertices.Max(x => x.X) - vertices.Min(x => x.X), vertices.Max(x => x.Y) - vertices.Min(x => x.Y));
            minDist = Math.Min(minDist, vertices.Max(x => x.Z) - vertices.Min(x => x.Z));

            bool useYZ = false;
            bool useXZ = false;
            if (vertices.Min(x => Math.Round(x.X, 6)) == vertices.Max(x => Math.Round(x.X, 6)) || (vertices.Max(x => x.X) - vertices.Min(x => x.X)) == minDist)
                useYZ = true;
            else if (vertices.Min(x => Math.Round(x.Y, 6)) == vertices.Max(x => Math.Round(x.Y, 6)) || (vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) == minDist)
                useXZ = true;

            foreach (Point pt in vertices)
            {
                Point p = pt.RoundedPoint();
                Point panelPt = panelBottomRightReference.RoundedPoint();
                Vector direction = p - panelPt;

                double xC = Math.Abs(direction.X);
                double yC = Math.Abs(direction.Y);
                double zC = Math.Abs(direction.Z);

                if (!useXZ && !useYZ)
                    gemOpening.Add(" " + xC.ToString() + " " + yC.ToString() + "\n");
                else if (useXZ)
                    gemOpening.Add(" " + xC.ToString() + " " + zC.ToString() + "\n");
                else if (useYZ)
                    gemOpening.Add(" " + yC.ToString() + " " + zC.ToString() + "\n");
            }

            return gemOpening;*/

            List<string> gemOpening = new List<string>();

            List<Point> vertices = opening.Polyline().IDiscontinuityPoints();
            if (!opening.Polyline().NormalAwayFromSpace(panelsAsSpace))
                vertices.Reverse();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES() + "\n");

            double minDist = Math.Min(vertices.Max(x => x.X) - vertices.Min(x => x.X), vertices.Max(x => x.Y) - vertices.Min(x => x.Y));
            minDist = Math.Min(minDist, vertices.Max(x => x.Z) - vertices.Min(x => x.Z));

            bool useYZ = false;
            bool useXZ = false;
            if (vertices.Min(x => Math.Round(x.X, 6)) == vertices.Max(x => Math.Round(x.X, 6)) || (vertices.Max(x => x.X) - vertices.Min(x => x.X)) == minDist)
                useYZ = true;
            else if (vertices.Min(x => Math.Round(x.Y, 6)) == vertices.Max(x => Math.Round(x.Y, 6)) || (vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) == minDist)
                useXZ = true;

            foreach (Point p in vertices)
            {
                if (!useXZ && !useYZ)
                    gemOpening.Add(" " + Math.Round(Math.Abs((p.X - panelBottomRightReference.X)), 6).ToString() + " " + Math.Round(Math.Abs((p.Y - panelBottomRightReference.Y)), 6).ToString() + "\n");
                else if (useXZ)
                    gemOpening.Add(" " + Math.Round(Math.Abs((p.X - panelBottomRightReference.X)), 6).ToString() + " " + Math.Round(Math.Abs((p.Z - panelBottomRightReference.Z)), 6).ToString() + "\n");
                else if (useYZ)
                    gemOpening.Add(" " + Math.Round(Math.Abs((p.Y - panelBottomRightReference.Y)), 6).ToString() + " " + Math.Round(Math.Abs((p.Z - panelBottomRightReference.Z)), 6).ToString() + "\n");

            }

            return gemOpening;
        }

        [Description("Convert an IES string representation of an opening to a BHoM Environment Opening")]
        [Input("openingPts", "The string representations of coordinates that make up the opening")]
        [Input("openingType", "The IES representation of the opening type")]
        [Output("opening", "The BHoM Environment Opening converted from IES GEM format")]
        public static Opening ToBHoM(this List<string> openingPts, string openingType)
        {
            List<Point> points = openingPts.Select(x => x.ToBHoMPoint()).ToList();
            points.Add(points.First());
            /*for(int x = 0; x < points.Count; x++)
            {
                points[x].X += panelXY.X;
                points[x].Y += panelXY.Y;
                points[x].Z += panelXY.Z;
            }*/

            Polyline pLine = new Polyline { ControlPoints = points, };

            Opening opening = new Opening();
            opening.Edges = pLine.ToEdges();
            opening.Type = openingType.ToBHoMOpeningType();

            return opening;
        }
    }
}