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

        [Description("Create a FileSettings object for use with the IES Adapter")]
        [Input("fileName", "Name of GEM file, not including the file extension. Default 'BHoM_GEM_File'")]
        [Input("directory", "Path to GEM file. Defaults to your desktop")]
        [Output("fileSettings", "The file settings to use with the IES adapter for pull and push")]
        public static IESFileSettings IESFileSettings(string fileName = "BHoM_GEM_File", string directory = null)
        {
            if (directory == null)
                directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            if (System.IO.Path.HasExtension(fileName) && System.IO.Path.GetExtension(fileName) == ".gem")
            {
                BH.Engine.Reflection.Compute.RecordError("File name cannot contain a file extension");
                return null;
            }

            return new IESFileSettings
            {
                Directory = directory,
                FileName = fileName,
            };
        }
    }
}
