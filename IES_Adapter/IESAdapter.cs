/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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
using BH.oM.Base.Attributes;

using BH.Engine.Adapters.IES;

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
                BH.Engine.Base.Compute.RecordError("Please set the File Settings correctly to enable the IES Adapter to work correctly");
                return;
            }

            if (!Path.HasExtension(fileSettings.FileName) || Path.GetExtension(fileSettings.FileName) != ".gem")
            {
                BH.Engine.Base.Compute.RecordError("File Name must contain a file extension");
                return;
            }

            if (settingsIES == null)
            {
                BH.Engine.Base.Compute.RecordError("Please set some IES Settings on the IES Adapter before pushing");
                return;
            }            

            _fileSettings = fileSettings;
            _settingsIES = settingsIES;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private BH.oM.Adapter.FileSettings _fileSettings { get; set; } = null;
        private SettingsIES _settingsIES { get; set; } = null; 
    }
}



