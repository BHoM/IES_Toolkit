using System;
using System.Collections.Generic;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.IES.Settings
{
    [Description("Create a SettingsIES object for use with the IES Adapter")]
    public class SettingsIES : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("Set tolarance for planar surfaces, default is set to BH.oM.Geometry.Tolerance.Distance")]
        public double PlanarTolerance { get; set; } = BH.oM.Geometry.Tolerance.Distance;

        [Description("Set how many decimal places coordinates should have on export, default is set to 6")]
        public int DecimalPlaces { get; set; } = 6;

        /***************************************************/
    }
}
