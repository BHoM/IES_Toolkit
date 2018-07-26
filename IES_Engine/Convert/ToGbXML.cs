using System;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BHE = BH.oM.Environment;
using BHS = BH.oM.Structural;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Environment.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine;
using XML_Adapter;

namespace BH.Engine.IES
{
    public static partial class Convert
    {

        /***************************************************/
        /**** Public Methods - Geometry                 ****/
        /***************************************************/

        /*public static gbXML.gbXML ToGbXML(BHE.Elements.BuildingElementPanel bHoMBuildingElementPanel)
        {
            //List<BHE.Elements.BuildingElementPanel> panelList = new List<BHE.Elements.BuildingElementPanel>();

            gbXML.gbXML gbXMLfile = gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMBuildingElementPanel as IBHoMObject });
            //XMLWriter.Save(@"C: \Users\smalmste\Desktop\", "MyTestXml", gbXMLfile);
            return gbXMLfile;
        }

        /***************************************************/

        /*public static gbXML.gbXML ToGbXML(BHE.Elements.Space bHoMSpace)
        {
            gbXML.gbXML gbXMLfile = gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMSpace as IBHoMObject });
            return gbXMLfile;

        }*/




    }
}
