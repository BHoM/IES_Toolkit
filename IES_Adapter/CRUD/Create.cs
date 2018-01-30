using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environmental;
using XML_Adapter;
using gbXML= XML_Adapter.gbXML;

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

            if (typeof(IObject).IsAssignableFrom(typeof(T)))
            {

                foreach (T obj in objects)
                {
                    success &= Create(obj as dynamic);
                }
            }

            return success;

        }

        /***************************************************/

        private bool Create(BHE.Elements.BuildingElementPanel bHoMBuildingElementPanel)
        {

            //Convert the BHoMPanel to gbXML
            BHG.Polyline bHoMPolyline = bHoMBuildingElementPanel.Curve;

            gbXML.Polyloop MakePolyloop(List < BHG.Point > pts);
            



            return true;
        }
    }
}
