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

        protected override bool Create<T>(IEnumerable<T> objects)
        {
            List<IBHoMObject> bhomObjects = objects.Select(x => (IBHoMObject)x).ToList();
            List<Panel> panels = bhomObjects.Panels();

            List<List<Panel>> panelsAsSpaces = panels.ToSpaces();
            List<Panel> panelsAsShade = panels.PanelsByType(PanelType.Shade);

            StreamWriter sw = new StreamWriter(_fileSettings.FullFileName());

            try
            {
                foreach (List<Panel> space in panelsAsSpaces)
                {
                    List<string> output = space.ToIES(_settingsIES);
                    foreach (string s in output)
                        sw.Write(s);
                }

                panelsAsShade.ToIESShading(_settingsIES).ForEach(x => sw.Write(x));
            }
            catch(Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError("An error occurred in exporting the IES GEM file. Error is: " + e.ToString());
            }

            sw.Close();

            return true;
        }

        /***************************************************/

    }
}
