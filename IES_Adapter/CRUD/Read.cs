using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Environmental.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine;
using gbXML = XML_Adapter.gbXML;
using BHE = BH.oM.Environmental;
using XML_Adapter;

namespace BH.Adapter.IES
{
    public partial class IesAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> Read(Type type, IList indices = null)
        {
            if (type == typeof(Space))
                return ReadSpaces();
            if (type == typeof(BuildingElementPanel))
                return ReadPanels();
            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Space> ReadSpaces(List<string> ids = null)
        {
            BH.oM.XML.gbXML gbx = XMLReader.Load(Filepath, Filename);
            IEnumerable<IBHoMObject> bHoMObject = gbXML.gbXMLDeserializer.Deserialize(gbx);
            return bHoMObject.Where(x => x is BHE.Elements.Space).Cast<Space>().ToList();
        }

        /***************************************************/

        private List<BuildingElementPanel> ReadPanels(List<string> ids = null)
        {
            BH.oM.XML.gbXML gbx = XMLReader.Load(Filepath, Filename);
            IEnumerable<IBHoMObject> bHoMObject = gbXML.gbXMLDeserializer.Deserialize(gbx);
            return bHoMObject.Where(x => x is BHE.Elements.BuildingElementPanel).Cast<BuildingElementPanel>().ToList();
        }
    }
}

