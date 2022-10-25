using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.oM.IES.Fragments
{
    [Description("Fragment containing the surface index from IES")]
    public class SurfaceIndexFragment : IFragment
    {
        public virtual int SurfaceID { get; set; } = -1;

    }
}
