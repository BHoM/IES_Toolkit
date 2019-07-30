using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determine whether a point falls on the left side of a line or not. The left side is defined as the left hand side of the line when standing on the start point and looking at the end point")]
        [Input("line", "The line to determine directionality")]
        [Input("check", "The point to check against")]
        [Output("isLeft", "True if the point is on the left hand side of the line. False if it is on the line or on the right hand side")]
        public static bool IsLeft(Line line, Point check)
        {
            if(line.Start.Z != line.End.Z)
            {
                //Change the line to not be 3D...
                Point s = line.Start;
                Point e = line.End;

                s.X = line.Start.Y;
                s.Y = line.Start.Z;
                s.Z = line.Start.X;
                e.X = line.End.Y;
                e.Y = line.End.Z;
                e.Z = line.End.X;
                line.Start = s;
                line.End = e;
            }

            return ((line.End.X - line.Start.X) * (check.Y - line.Start.Y) - (line.End.Y - line.Start.Y) * (check.X - line.Start.X)) > 0;
        }
    }
}
