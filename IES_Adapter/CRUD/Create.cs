using System;
using BH.Adapter;
using System.Collections.Generic;
using BH.oM.Base;
using BHG = BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using System.IO;

using System.Linq;
using BH.Engine.IES;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            List<IBHoMObject> bhomObjects = objects.Select(x => (IBHoMObject)x).ToList();
            List<Panel> panels = bhomObjects.Panels();

            List<List<Panel>> panelsAsSpaces = panels.ToSpaces();

            StreamWriter sw = new StreamWriter(_fileSettings.FullFileName());

            foreach (List<Panel> space in panelsAsSpaces)
            {
                List<string> output = space.ToIES();
                foreach (string s in output)
                    sw.Write(s);
            }

            sw.Close();

            return true;
        }

        /***************************************************/

    }
}
