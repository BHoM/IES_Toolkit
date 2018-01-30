using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHE = BH.oM.Environmental;

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



            return true;
        }
    }
}
