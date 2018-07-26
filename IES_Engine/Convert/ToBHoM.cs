using System;
using System.Collections.Generic;
using System.Linq;
using BHE = BH.oM.Environment;
using BHS = BH.oM.Structural;
using BHG = BH.oM.Geometry;
using BH.oM.Environment.Properties;
using BH.oM.Environment.Elements;

namespace BH.Engine.IES
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods - BHoM Objects             ****/
        /***************************************************/

        public static BHE.Elements.Building ToIES(this Building iesBuilding)
        {
            BHE.Elements.Building bHoMBuilding = new BHE.Elements.Building
            {
                //Latitude = tasBuilding.latitude,
                //Longitude = tasBuilding.longitude,
                //Elevation = tasBuilding.maxBuildingAltitude,

            };
            return bHoMBuilding;
        }
    }
}
