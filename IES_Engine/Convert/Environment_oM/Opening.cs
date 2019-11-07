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
            
            Polyline openingCurve = opening.Polyline();

            Vector zVector = new Vector { X = 0, Y = 1, Z = 0 };
            Plane openingPlane = openingCurve.IFitPlane();
            Vector planeNormal = openingPlane.Normal;

            Point xyRefPoint = new Point {X =  0, Y = 0, Z = 0};
            Vector translateVector = xyRefPoint - panelBottomRightReference;

            Vector rotationVector = new Vector { X = 0, Y = 0, Z = 1 };
            if (openingCurve.ControlPoints.Max(x => x.Z) - openingCurve.ControlPoints.Min(x => x.Z) <= BH.oM.Geometry.Tolerance.Distance)
                rotationVector = new Vector { X = 1, Y = 0, Z = 0, }; //Handle horizontal openings


            double rotationAngle = planeNormal.Angle(zVector);
            TransformMatrix rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

            Polyline openingTransformed = openingCurve.Transform(rotateMatrix);
            Polyline openingTranslated = openingTransformed.Translate(translateVector);

            List<Point> vertices = openingTranslated.IDiscontinuityPoints();

            gemOpening.Add(vertices.Count.ToString() + " " + opening.Type.ToIES(settings) + "\n");

            if ((vertices.Max(x => x.Y) - vertices.Min(x => x.Y)) >= BH.oM.Geometry.Tolerance.Distance)
            {
                foreach (Point p in vertices)
                {
                    Vector direction = p.RoundedPoint() - xyRefPoint.RoundedPoint();
                    gemOpening.Add(" " + Math.Abs(direction.X).ToString() + " " + Math.Abs(direction.Y).ToString() + "\n");
                }

                /*openingCurve = opening.Polyline().Flip();
                openingPlane = openingCurve.IFitPlane();
                planeNormal = openingPlane.Normal;

                rotationAngle = planeNormal.Angle(zVector);
                rotateMatrix = BH.Engine.Geometry.Create.RotationMatrix(panelBottomRightReference, rotationVector, rotationAngle);

                openingTransformed = openingCurve.Transform(rotateMatrix);
                openingTranslated = openingTransformed.Translate(translateVector);

                vertices = openingTranslated.IDiscontinuityPoints();*/
            }
            else
            {
                foreach (Point p in vertices)
                {
                    Vector direction = p.RoundedPoint() - xyRefPoint.RoundedPoint();
                    gemOpening.Add(" " + Math.Abs(direction.X).ToString() + " " + Math.Abs(direction.Z).ToString() + "\n");
                }
            }            

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