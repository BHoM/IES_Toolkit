using System;
using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BHE = BH.oM.Environment;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

    }
}
