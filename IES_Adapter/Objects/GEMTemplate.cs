using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.IES
{
    public class GEMTemplate
    {
        public virtual string Layer { get; set; }
        public virtual string Colour { get; set; }
        public virtual string Category { get; set; }
        public virtual string Type { get; set; }
        public virtual string SubType { get; set; }
        public virtual string ColourRGB { get; set; }
        public virtual string Name { get; set; }
        public virtual int VerticesCount { get; set; }
        public virtual int FaceCount { get; set; }
    }
}
