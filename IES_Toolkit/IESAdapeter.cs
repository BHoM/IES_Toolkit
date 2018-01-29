using System;
using System.Collections.Generic;
using BH.oM.Queries;

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
            //if (!String.IsNullOrEmpty(iesFilePath) && System.IO.File.Exists(iesFilePath))
                //m_IesDocumentInstance.open(iesFilePath);

           // else if (!String.IsNullOrEmpty(iesFilePath))
                //m_IesDocumentInstance.create(iesFilePath); //TODO: what if an existing file has the same name? 

           // else
                //ErrorLog.Add("The IES file does not exist");

            //AdapterId = BH.Engine.IES.Convert.AdapterID;
            Config.MergeWithComparer = false;   //Set to true after comparers have been implemented
            Config.ProcessInMemory = false;
            Config.SeparateProperties = false;  //Set to true after Dependency types have been implemented
            Config.UseAdapterId = false;        //Set to true when NextId method and id tagging has been implemented

        }



        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //private TBD.TBDDocumentClass m_IesDocumentInstance = new TBD.TBDDocumentClass();


        /***************************************************/
    }
}

