using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Data.Requests;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.Engine.IES;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!System.IO.File.Exists(_fileSettings.FullFileName()))
            {
                BH.Engine.Reflection.Compute.RecordError("File does not exist to pull from");
                return new List<IBHoMObject>();
            }

            if (request != null && request is FilterRequest)
            {
                FilterRequest filterRequest = request as FilterRequest;

                return Read(filterRequest.Type);
            }
            else
                return ReadFullGEM();
        }
    }
}
