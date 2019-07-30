using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.IES
{
    public static partial class Modify
    {
        [Description("Modifys a BHoM Geometry Point to be rounded to the number of provided decimal places")]
        [Input("point", "The BHoM Geometry Point to modify")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6")]
        [Output("point", "The modified BHoM Geometry Point")]
        public static Point RoundedPoint(this Point point, int decimalPlaces = 6)
        {
            return new Point
            {
                X = Math.Round(point.X, decimalPlaces),
                Y = Math.Round(point.Y, decimalPlaces),
                Z = Math.Round(point.Z, decimalPlaces),
            };
        }
    }
}