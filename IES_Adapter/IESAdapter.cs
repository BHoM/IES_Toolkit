using System;
using System.Collections.Generic;
using BH.oM.Data.Requests;
using BH.Engine;
using BH.oM.Base;
using System.Linq;
using System.Reflection;

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
        [Input("iesFileSettings", "Input the file settings the IES Adapter should use, default null")]
        [Input("settingsIES", "Input additional settings the adapter should use.")]
        [Output("adapter", "Adapter to IES GEM")]
        public IESAdapter(FileSettingsIES iesFileSettings = null, SettingsIES settingsIES = null)
        {
            // This asks the base adapter to only Create the objects.
            m_AdapterSettings.DefaultPushType = oM.Adapter.PushType.CreateOnly;

            if (iesFileSettings == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Please set the File Settings correctly to enable the IES Adapter to work correctly");
                return;
            }

            if (settingsIES == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Please set some IES Settings on the IES Adapter before pushing");
                return;
            }            

            _fileSettings = iesFileSettings;
            _settingsIES = settingsIES;

            AdapterIdName = "IES_Adapter";
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private FileSettingsIES _fileSettings { get; set; } = null;
        private SettingsIES _settingsIES { get; set; } = null; 
    }
}

