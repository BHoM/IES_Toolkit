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

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Produces an IES Adapter to allow interopability with IES GEM files and the BHoM")]
        [Input("fileSettings", "Input the file settings the IES Adapter should use, default null")]
        [Output("adapter", "Adapter to IES GEM")]
        public IESAdapter(FileSettings fileSettings = null)
        {
            if(fileSettings == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Please set the File Settings correctly to enable the IES Adapter to work correctly");
                return;
            }

            _fileSettings = fileSettings;

            AdapterId = "IES_Adapter";
            Config.MergeWithComparer = false;   //Set to true after comparers have been implemented
            Config.ProcessInMemory = false;
            Config.SeparateProperties = false;  //Set to true after Dependency types have been implemented
            Config.UseAdapterId = false;        //Set to true when NextId method and id tagging has been implemented
        }

        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            /*bool success = true;
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                success &= Create(list as dynamic, false);
            }

            return success ? objects.ToList() : new List<IObject>();*/

            throw new NotImplementedException();           
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private System.Diagnostics.Process m_IesProcess = new System.Diagnostics.Process();


        /***************************************************/
        /**** Public properties                         ****/
        /***************************************************/

        private FileSettings _fileSettings { get; set; } = null;
    }
}

