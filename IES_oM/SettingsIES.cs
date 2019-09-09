using System;
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.IES.Settings
{
    public class SettingsIES : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public double PlanarTolerance { get; set; } = BH.oM.Geometry.Tolerance.Distance; 

        /***************************************************/
    }
}
