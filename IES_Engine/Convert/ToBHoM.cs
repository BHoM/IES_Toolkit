using System;
using System.Collections.Generic;
using System.Linq;
using BHE = BH.oM.Environmental;
using BHS = BH.oM.Structural;
using BHG = BH.oM.Geometry;
using BH.oM.Environmental.Properties;
using BH.oM.Environmental.Elements;

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
