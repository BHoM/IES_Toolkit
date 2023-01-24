using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public static partial class Create
    {
        public static GEMTemplate ShadeTemplate()
        {
            return new GEMTemplate()
            {
                Layer = "1",
                Colour = "62",
                Category = "1",
                Type = "4",
                SubType = "0",
                ColourRGB = "65280"
            };
        }
    }
}
