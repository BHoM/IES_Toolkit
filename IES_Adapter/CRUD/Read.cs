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

        private List<Space> ReadZones(List<string> ids = null)
        {

            throw new NotImplementedException();
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

