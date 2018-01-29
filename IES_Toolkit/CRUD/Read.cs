using System;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Properties;
using BH.oM.Environmental.Interface;
using BHG = BH.oM.Geometry;
using BH.Engine;

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
            return null;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private List<Space> ReadZones(List<string> ids = null)
        {
            throw new NotImplementedException();
        }
    }
}

