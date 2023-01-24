using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Create
    {
        public static GEMTemplate SpaceTemplate()
        {
            return new GEMTemplate()
            {
                Layer = "1",
                Colour = "0",
                Category = "1",
                Type = "1",
                SubType = "2001",
                ColourRGB = "16711690"
            };
        }
    }
}
