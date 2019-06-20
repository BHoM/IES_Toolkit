﻿using System;
using System.Collections.Generic;
using BH.oM.Data.Requests;
using BH.Engine;
using BH.oM.Base;
using System.Linq;
using System.Reflection;

namespace BH.Adapter.IES
{
    public partial class IesAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        /*public IesAdapter( string gbXMLName, string gbXMLDirectoryPath)
        {

            Filepath = gbXMLDirectoryPath;
            Filename = gbXMLName;

            AdapterId = Engine.IES.Convert.AdapterID;
            Config.MergeWithComparer = false;   //Set to true after comparers have been implemented
            Config.ProcessInMemory = false;
            Config.SeparateProperties = false;  //Set to true after Dependency types have been implemented
            Config.UseAdapterId = false;        //Set to true when NextId method and id tagging has been implemented

        }*/

        public IesAdapter()
        {
            BH.Engine.Reflection.Compute.RecordError("The IES Adapter has not been fully implemented yet and cannot be used. For queries please contact the Building Environments Development team (https://github.com/BuroHappoldEngineering/BuildingEnvironments_Toolkit/wiki/Roles)");
            throw new NotImplementedException();
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

        public string Filepath { get; set; }
        public string Filename { get; set; }
        

    }
}

