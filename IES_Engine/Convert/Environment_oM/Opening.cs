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

namespace BH.Engine.IES
{
    public static partial class Convert
    {

        [Description("Convert a BHoM Environment Opening to an IES string representation of an opening for GEM format")]
        [Input("opening", "The BHoM Environment Opening to convert")]
        [Input("panelsAsSpace", "The panels representing a single space which hosts this opening, used to check the orientation of the opening")]
        [Input("panelXYZ", "The bottom left corner point of the host panel to calculate the opening points from for GEM format")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesOpening", "The string representation for IES GEM format")]
        public static List<string> ToIES(this Opening opening, List<Panel> panelsAsSpace, Point panelBottomRightReference, SettingsIES settings)
        {
            List<string> gemOpening = new List<string>();
            //List<Point> vertices = opening.Polyline().IDiscontinuityPoints();

            //gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES() + "\n");
            
            Polyline openingCurve = opening.Polyline();

            //if (!openingCurve.NormalAwayFromSpace(panelsAsSpace, settings.PlanarTolerance))
                //openingCurve = openingCurve.Flip();

            Vector zVector = new Vector { X = 0, Y = 1, Z = 0 };
            Plane openingPlane = openingCurve.IFitPlane();
            Vector planeNormal = openingPlane.Normal;

            Point xyRefPoint = new Point {X =  0, Y = 0, Z = 0};
            Vector translateVector = xyRefPoint - panelBottomRightReference;

            Vector rotationVector = new Vector { X = 0, Y = 0, Z = 1 };
            double rotationAngle = planeNormal.Angle(zVector);
            TransformMatrix rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

            Polyline openingTransformed = openingCurve.Transform(rotateMatrix);
            Polyline openingTranslated = openingTransformed.Translate(translateVector);

            List<Point> vertices = openingTranslated.IDiscontinuityPoints();

            if((vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) >= BH.oM.Geometry.Tolerance.Distance)
            {
                openingCurve = opening.Polyline().Flip();
                openingPlane = openingCurve.IFitPlane();
                planeNormal = openingPlane.Normal;

                rotationAngle = planeNormal.Angle(zVector);
                rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

                openingTransformed = openingCurve.Transform(rotateMatrix);
                openingTranslated = openingTransformed.Translate(translateVector);

                vertices = openingTranslated.IDiscontinuityPoints();
            }

            /*Point closestToZero = null;
            double currentDistance = 1e10;
            foreach(Point v in vertices)
            {
                if(v.Distance(xyRefPoint) < currentDistance)
                {
                    currentDistance = v.Distance(xyRefPoint);
                    closestToZero = v;
                }
            }

            List<Point> zeroOffVertices = new List<Point>();
            foreach(Point v1 in vertices)
            {
                Vector v2 = v1 - closestToZero;
                zeroOffVertices.Add(new Point { X = v2.X, Y = v2.Y, Z = v2.Z });
            }

            Polyline zeroedOff = new Polyline { ControlPoints = zeroOffVertices };
            vertices = new List<Point>(zeroOffVertices);

            int zeroIndex = vertices.IndexOf(xyRefPoint);
            int nextIndex = zeroIndex + 1;
            if (nextIndex >= vertices.Count)
                nextIndex = 0;
            if (vertices[zeroIndex].Distance(vertices[nextIndex]) <= BH.oM.Geometry.Tolerance.Distance)
                nextIndex++;

            Point goodPt = new Point { X = 0, Y = vertices[nextIndex].Y, Z = 0, };
            if (vertices[zeroIndex].X > (0 + BH.oM.Geometry.Tolerance.Distance))
                goodPt.Z = vertices[zeroIndex].Z;
            else
                goodPt.X = vertices[zeroIndex].X;

            Vector badPtToOrigin = vertices[nextIndex] - xyRefPoint;
            Vector goodPtToOrigin = goodPt - xyRefPoint;
            double angle = badPtToOrigin.Angle(goodPtToOrigin);

            TransformMatrix rotateMatrix2 = BH.Engine.Geometry.Create.RotationMatrix(xyRefPoint, zVector, angle);
            Polyline openingFinal = zeroedOff.Transform(rotateMatrix2);

            vertices = openingFinal.IDiscontinuityPoints();

            zeroOffVertices = new List<Point>();
            foreach(Point v3 in vertices)
                zeroOffVertices.Add(v3 + closestToZero);

            vertices = new List<Point>(zeroOffVertices);
            */
            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settings) + "\n");

            foreach(Point p in vertices)
            {
                Vector direction = p.RoundedPoint() - xyRefPoint.RoundedPoint();
                gemOpening.Add(" " + Math.Abs(direction.X).ToString() + " " + Math.Abs(direction.Z).ToString() + "\n");
                //gemOpening.Add(" " + direction.X.ToString() + " " + direction.Z.ToString() + "\n");
            }


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

            /*List<string> gemOpening = new List<string>();

            List<Point> vertices = opening.Polyline().IDiscontinuityPoints();
            if (!opening.Polyline().NormalAwayFromSpace(panelsAsSpace, settings.PlanarTolerance))
                vertices.Reverse();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settings) + "\n");

            double xDist = vertices.Max(x => x.X) - vertices.Min(x => x.X);
            double yDist = vertices.Max(x => x.Y) - vertices.Min(x => x.Y);
            double zDist = vertices.Max(x => x.Z) - vertices.Min(x => x.Z);

            double minDist = Math.Min(xDist, Math.Min(yDist, zDist));

            bool useYZ = false;
            bool useXZ = false;
            if (vertices.Min(x => Math.Round(x.X, 6)) == vertices.Max(x => Math.Round(x.X, 6)) || (vertices.Max(x => x.X) - vertices.Min(x => x.X)) == minDist)
                useYZ = true;
            else if (vertices.Min(x => Math.Round(x.Y, 6)) == vertices.Max(x => Math.Round(x.Y, 6)) || (vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) == minDist)
                useXZ = true;

            foreach (Point p in vertices)
            {
                /*if (minDist < 2)
                {
                    if (!useXZ && !useYZ)
                        gemOpening.Add(" " + Math.Round(Math.Abs((p.X - panelBottomRightReference.X)), 6).ToString() + " " + Math.Round(Math.Abs((p.Y - panelBottomRightReference.Y)), 6).ToString() + "\n");
                    if (useXZ)
                        gemOpening.Add(" " + Math.Round(Math.Abs((p.X - panelBottomRightReference.X)), 6).ToString() + " " + Math.Round(Math.Abs((p.Z - panelBottomRightReference.Z)), 6).ToString() + "\n");
                    else if (useYZ)
                        gemOpening.Add(" " + Math.Round(Math.Abs((p.Y - panelBottomRightReference.Y)), 6).ToString() + " " + Math.Round(Math.Abs((p.Z - panelBottomRightReference.Z)), 6).ToString() + "\n");
                }
                else
                {
                    Point pt = new Point { X = p.X, Y = p.Y, Z = 0 };
                    Point pt2 = new Point { X = panelBottomRightReference.X, Y = panelBottomRightReference.Y, Z = 0 };
                    //double distance = pt2.Distance(pt);
                    //gemOpening.Add(" " + Math.Round(Math.Abs(distance), 6).ToString() + " " + Math.Round(Math.Abs((p.Z - panelBottomRightReference.Z)), 6).ToString() + "\n");
                    Vector diff = pt - pt2;
                    gemOpening.Add(" " + Math.Round(Math.Abs(diff.X), 6) + " " + Math.Round(Math.Abs(diff.Y), 6) + "\n");
                }*/

            /*Point pt = new Point { X = p.X, Y = p.Y, Z = p.Z };
            Point pt2 = new Point { X = panelBottomRightReference.X, Y = panelBottomRightReference.Y, Z = panelBottomRightReference.Z };

            Vector diff = pt2 - pt;
            double minDiff = Math.Min(Math.Abs(diff.X), Math.Min(Math.Abs(diff.Y), Math.Abs(diff.Z)));

            if(diff.X == minDiff)
                gemOpening.Add(" " + Math.Round(Math.Abs(diff.Y), 6).ToString() + " " + Math.Round(Math.Abs(diff.Z), 6).ToString() + "\n");
            else if (diff.Y == minDiff)
                gemOpening.Add(" " + Math.Round(Math.Abs(diff.X), 6).ToString() + " " + Math.Round(Math.Abs(diff.Z), 6).ToString() + "\n");
            else
                gemOpening.Add(" " + Math.Round(Math.Abs(diff.X), 6).ToString() + " " + Math.Round(Math.Abs(diff.Y), 6).ToString() + "\n");
        }*/

            return gemOpening;
        }

        [Description("Convert an IES string representation of an opening to a BHoM Environment Opening")]
        [Input("openingPts", "The string representations of coordinates that make up the opening")]
        [Input("openingType", "The IES representation of the opening type")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("opening", "The BHoM Environment Opening converted from IES GEM format")]
        public static Opening ToBHoM(this List<string> openingPts, string openingType, SettingsIES settings)
        {
            List<Point> points = openingPts.Select(x => x.ToBHoMPoint(settings)).ToList();
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
            opening.Type = openingType.ToBHoMOpeningType(settings);

            return opening;
        }
    }
}