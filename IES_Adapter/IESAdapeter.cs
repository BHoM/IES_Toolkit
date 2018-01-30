using System;
using System.Collections.Generic;
using BH.oM.Queries;
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

        public IesAdapter(string iesFilePath = "")
        {
            //ies application
            if (!String.IsNullOrEmpty(iesFilePath) && System.IO.File.Exists(iesFilePath))
            {
                m_IesProcess.StartInfo.FileName = iesFilePath;
                m_IesProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                
                m_IesProcess.Start();
                
            }

            else if (!String.IsNullOrEmpty(iesFilePath))
            {
                m_IesProcess.StartInfo.FileName = iesFilePath;
                m_IesProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                m_IesProcess.Start();
            }
               
           else
                ErrorLog.Add("The IES file does not exist");

            AdapterId = Engine.IES.Convert.AdapterID;
            Config.MergeWithComparer = false;   //Set to true after comparers have been implemented
            Config.ProcessInMemory = false;
            Config.SeparateProperties = false;  //Set to true after Dependency types have been implemented
            Config.UseAdapterId = false;        //Set to true when NextId method and id tagging has been implemented

        }


        public override bool Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            bool success = true;
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                success &= Create(list as dynamic, false);
            }



            return success;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private System.Diagnostics.Process m_IesProcess = new System.Diagnostics.Process();


        /***************************************************/
    }
}

