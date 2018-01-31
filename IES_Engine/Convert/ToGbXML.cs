using System;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BHE = BH.oM.Environmental;
using BHS = BH.oM.Structural;
using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Environmental.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine;
using XML_Adapter;
using gbXML = XML_Adapter.gbXML;

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

            gbXML.gbXML gbXMLfile = gbXML.gbXMLSerializer.Serialize(new List<IObject> { bHoMBuildingElementPanel as IObject });
            //XMLWriter.Save(@"C: \Users\smalmste\Desktop\", "MyTestXml", gbXMLfile);
            return gbXMLfile;
        }

        /***************************************************/

        /*public static gbXML.gbXML ToGbXML(BHE.Elements.Space bHoMSpace)
        {
            gbXML.gbXML gbXMLfile = gbXML.gbXMLSerializer.Serialize(new List<IObject> { bHoMSpace as IObject });
            return gbXMLfile;

        }*/




    }
}
