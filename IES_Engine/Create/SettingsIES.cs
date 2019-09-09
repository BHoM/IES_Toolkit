using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.IES.Settings;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.IES
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a SettingsIES object for use with the IES Adapter")]
        [Input("planarTolerance", "Set tolarance for planar surfaces")]     // need to come up with description 
        [Output("settingsIES", "The IES settings to use with the IES adapter push")]
        public static SettingsIES SettingsIES(double planarTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return new SettingsIES
            {
                PlanarTolerance = planarTolerance,
            };
        }            
    }
}
