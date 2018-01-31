using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environmental;
using XML_Adapter;
using gbXML = XML_Adapter.gbXML;
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
            bool success = true;

            gbXML.gbXML gbx = new gbXML.gbXML();

            if (typeof(IObject).IsAssignableFrom(typeof(T)))
            {

                foreach (T obj in objects)
                {
                    success &= Create(obj as dynamic, gbx);
                }

            }

            if(success)
                XMLWriter.Save(@"C: \Users\smalmste\Desktop\", "MyTestXml", gbx);
            return success;

        }

        /***************************************************/

        private bool Create(BHE.Elements.BuildingElementPanel bHoMBuildingElementPanel, gbXML.gbXML gbx)
        {

            gbXML.gbXMLSerializer.Serialize(new List<IObject> { bHoMBuildingElementPanel as IObject }, gbx);

            return true;
        }

        private bool Create(BHE.Elements.Space bHoMSpace, gbXML.gbXML gbx)
        {

            gbXML.gbXMLSerializer.Serialize(new List<IObject> { bHoMSpace as IObject }, gbx);

            return true;
        }



    }
}
