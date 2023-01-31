using BH.oM.Base.Attributes;
using BH.oM.IES.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;

namespace BH.Adapter.IES
{
    public static partial class Convert
    {
        [Description("Convert a BHoM Opening Type to an IES string representation for GEM format")]
        [Input("type", "The BHoM Opening Type to convert")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("iesOpeningType", "The IES string representation of the BHoM opening type")]
        public static string ToIES(this OpeningType type, SettingsIES settingsIES)
        {
            switch (type)
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
                case OpeningType.Hole:
                default:
                    return "2"; //Hole
            }
        }

        [Description("Convert an IES string representation of a Opening Type to a BHoM Opening Type")]
        [Input("iesOpeningType", "The IES string representation of an opening type")]
        [Input("settingsIES", "The IES settings to use with the IES adapter")]
        [Output("openingType", "The BHoM Opening Type")]
        public static OpeningType FromIESOpeningType(this string iesOpeningType, SettingsIES settingsIES)
        {
            if (iesOpeningType == "0")
                return OpeningType.Window;
            if (iesOpeningType == "1")
                return OpeningType.Door;
            if (iesOpeningType == "2")
                return OpeningType.Hole;

            return OpeningType.Undefined;
        }
    }
}
