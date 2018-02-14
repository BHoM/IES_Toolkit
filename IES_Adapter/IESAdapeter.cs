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

        public IesAdapter(string iesFilePath = "", string gbXML_name = "MyTestXML", string gbXML_FilePath = @"C: \Users\smalmste\Desktop\")
        {
            //ies application
            if (!String.IsNullOrEmpty(iesFilePath) && System.IO.File.Exists(iesFilePath))
            {
                m_IesProcess.StartInfo.FileName = iesFilePath;
                m_IesProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

               // m_IesProcess.Start();

            }

           else
                ErrorLog.Add("The IES file does not exist");


            filepath = gbXML_FilePath;
            filename = gbXML_name;


            AdapterId = Engine.IES.Convert.AdapterID;
            Config.MergeWithComparer = false;   //Set to true after comparers have been implemented
            Config.ProcessInMemory = false;
            Config.SeparateProperties = false;  //Set to true after Dependency types have been implemented
            Config.UseAdapterId = false;        //Set to true when NextId method and id tagging has been implemented

        }


        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            bool success = true;
            MethodInfo miToList = typeof(Enumerable).GetMethod("Cast");
            foreach (var typeGroup in objects.GroupBy(x => x.GetType()))
            {
                MethodInfo miListObject = miToList.MakeGenericMethod(new[] { typeGroup.Key });

                var list = miListObject.Invoke(typeGroup, new object[] { typeGroup });

                success &= Create(list as dynamic, false);
            }

            return success ? objects.ToList() : new List<IObject>();
        
           
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private System.Diagnostics.Process m_IesProcess = new System.Diagnostics.Process();

      
        /***************************************************/
        /**** Public Fields                            ****/
        /***************************************************/

        public static string filepath = @"C: \Users\smalmste\Desktop\";
        public static string filename = "MyTestXML";



    }
}

