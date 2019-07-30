using System;
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.IES.Settings
{
    public class FileSettings : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string FileName { get; set; } = "";
        public string Directory { get; set; } = "";

        /***************************************************/
    }
}
