using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environmental;
using XML_Adapter;
using BH.oM.XML;
using BH.Engine.IES;
using System.Xml;

namespace BH.Adapter.IES
{
    public partial class IesAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            BH.oM.XML.gbXML gbx = new BH.oM.XML.gbXML();

            if (typeof(IBHoMObject).IsAssignableFrom(typeof(T)))
            {
                gbXML.gbXMLSerializer.Serialize(objects, gbx, false);
                XMLWriter.Save(Filepath, Filename + "_TAS", gbx);
                gbx = new BH.oM.XML.gbXML();
                gbXML.gbXMLSerializer.Serialize(objects, gbx, true);
                XMLWriter.Save(Filepath, Filename + "_IES", gbx);
            }

            return true;
        }

        /***************************************************/

        private bool Create(BHE.Elements.BuildingElementPanel bHoMBuildingElementPanel, BH.oM.XML.gbXML gbx, bool isIES)
        {
            gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMBuildingElementPanel as IBHoMObject }, gbx, isIES);

            return true;
        }

        /***************************************************/

        private bool Create(BHE.Elements.Space bHoMSpace, BH.oM.XML.gbXML gbx, bool isIES)
        {
            gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMSpace as IBHoMObject }, gbx, isIES);

            return true;
        }

        /***************************************************/

        private bool Create(BHE.Elements.Building bHoMBuilding, BH.oM.XML.gbXML gbx, bool isIES)
        {
            gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMBuilding as IBHoMObject }, gbx, isIES);

            return true;
        }

        /***************************************************/

        private bool Create(BHE.Elements.BuildingElement bHoMBuildingElement, BH.oM.XML.gbXML gbx, bool isIES)
        {
            gbXML.gbXMLSerializer.Serialize(new List<IBHoMObject> { bHoMBuildingElement as IBHoMObject }, gbx, isIES);

            return true;
        }

        /***************************************************/

    }
}
