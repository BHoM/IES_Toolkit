using System;
using System.Collections.Generic;
using BH.oM.Data.Requests;
using BH.Engine;
using BH.oM.Base;
using System.Linq;
using System.Reflection;
using System.IO;

using BH.oM.IES.Settings;

using System.ComponentModel;
using BH.oM.Reflection.Attributes;

using BH.Engine.IES;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Produces an IES Adapter to allow interopability with IES GEM files and the BHoM")]
        [Input("fileSettings", "Input fileSettings to get the file name and directory the IES Adapter should use")]
        [Input("settingsIES", "Input additional settings the adapter should use.")]
        [Output("adapter", "Adapter to IES GEM")]
        public IESAdapter(BH.oM.Adapter.FileSettings fileSettings = null, SettingsIES settingsIES = null)
        {
            // This asks the base adapter to only Create the objects.
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly;

            if (fileSettings == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Please set the File Settings correctly to enable the IES Adapter to work correctly");
                return;
            }

            if (!Path.HasExtension(fileSettings.FileName) || Path.GetExtension(fileSettings.FileName) != ".gem")
            {
                BH.Engine.Reflection.Compute.RecordError("File Name must contain a file extension");
                return;
            }

            if (settingsIES == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Please set some IES Settings on the IES Adapter before pushing");
                return;
            }            

            _fileSettings = fileSettings;
            _settingsIES = settingsIES;

            AdapterIdName = "IES_Adapter";
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private BH.oM.Adapter.FileSettings _fileSettings { get; set; } = null;
        private SettingsIES _settingsIES { get; set; } = null; 
    }
}

