using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.IES.Settings;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string FullFileName(this IESFileSettings fileSettings)
        {
            return System.IO.Path.Combine(fileSettings.Directory, fileSettings.FileName + ".gem");
        }
    }
}
