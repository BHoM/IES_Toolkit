using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Geometry;
using BH.Engine;
using BH.oM.Environment;

using System.IO;
using BH.Engine.IES;

using BH.oM.Adapter;

namespace BH.Adapter.IES
{
    public partial class IESAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override IEnumerable<IBHoMObject> IRead(Type type, IList indices = null, ActionConfig actionConfig = null)
        {
            return ReadFullGEM();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private IEnumerable<IBHoMObject> ReadFullGEM()
        {
            List<IBHoMObject> objects = new List<IBHoMObject>();

            StreamReader sr = new StreamReader(_fileSettings.FullFileName());

            List<string> iesStrings = new List<string>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
                iesStrings.Add(line);

            sr.Close();

            iesStrings.RemoveRange(0, 10); //Remove the first 10 items...
            bool endOfFile = false;
            while(!endOfFile)
            {
                int nextIndex = iesStrings.IndexOf("LAYER");
                if (nextIndex == -1)
                {
                    nextIndex = iesStrings.Count; //End of the file
                    endOfFile = true;
                }

                List<string> space = new List<string>();
                for (int x = 0; x < nextIndex; x++)
                    space.Add(iesStrings[x]);

                objects.AddRange(space.ToBHoMPanels(_settingsIES));

                if(!endOfFile)
                    iesStrings.RemoveRange(0, nextIndex + 10);
            }

            return objects;
        }
    }
}

