using System;
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

        protected override IEnumerable<IObject> Read(Type type, IList indices = null)
        {
            if (type == typeof(Space))
                return ReadZones();
            if (type == typeof(BuildingElementPanel))
                return ReadPanels();
            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private IEnumerable<IObject> ReadZones(List<string> ids = null)
        {
            List<BHE.Elements.Space> bHoMSpaces = new List<Space>();
            gbXML.gbXML gbx = XMLReader.Load(filepath, filename);
            IEnumerable<IObject> bHoMObject = gbXML.gbXMLDeserializer.Deserialize(gbx);
           

            return bHoMObject;
        }

        /***************************************************/

        private List<Space> ReadPanels(List<string> ids = null)
        {
            BuildingElementPanel bHoMPanel = new BuildingElementPanel();
            //gbXML.gbXML gbXMLfile = gbXML.(new List<IObject> { bHoMPanel as IObject });
            //XMLWriter.Save(@"C: \Users\smalmste\Desktop\", "MyTestXml", gbXMLfile);
            throw new NotImplementedException();
        }
    }
}

