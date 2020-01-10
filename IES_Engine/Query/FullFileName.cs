using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.IES.Settings;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.IES
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a FullFileName representation for use with the IES Adapter")]
        [Input("fileSettings", "The file settings to use with the IES adapter for pull and push")]
        [Output("fullFileName", "Name of GEM file including the file extension")]
        public static string FullFileName(this FileSettingsIES fileSettings)
        {
            return System.IO.Path.Combine(fileSettings.Directory, fileSettings.FileName + ".gem");
        }
    }
}
