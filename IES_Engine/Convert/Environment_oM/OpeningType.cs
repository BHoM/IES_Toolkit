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

        [Description("Convert a BHoM Opening Type to an IES string representation for GEM format")]
        [Input("type", "The BHoM Opening Type to convert")]
        [Output("iesOpeningType", "The IES string representation of the BHoM opening type")]
        public static string ToIES(this OpeningType type, SettingsIES settings)
        {
            switch(type)
            {
                case OpeningType.CurtainWall:
                case OpeningType.Glazing:
                case OpeningType.Rooflight:
                case OpeningType.RooflightWithFrame:
                case OpeningType.Window:
                case OpeningType.WindowWithFrame:
                    return "0";
                case OpeningType.Door:
                case OpeningType.VehicleDoor:
                    return "1";
                default:
                    return "2"; //Hole
            }
        }

        [Description("Convert an IES string representation of a Opening Type to a BHoM Opening Type")]
        [Input("iesOpeningType", "The IES string representation of an opening type")]
        [Output("openingType", "The BHoM Opening Type")]
        public static OpeningType ToBHoMOpeningType(this string iesOpeningType, SettingsIES settings)
        {
            if (iesOpeningType == "0")
                return OpeningType.Window;
            if (iesOpeningType == "1")
                return OpeningType.Door;

            return OpeningType.Undefined;
        }
    }
}