/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Adapter.IES;
using BH.oM.Adapter;
using BH.oM.Data.Requests;
using BH.oM.Environment.Elements;
using BH.oM.Environment.IES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using BH.Engine.Environment;
using BH.oM.Base;
using BH.Engine.Adapter;
using BH.oM.Geometry;
using NUnit.Framework;
using BH.Engine.Base;

namespace BH.Tests.Adapter.IES
{
    public class PushTests
    {
        IESAdapter m_Adapter;
        PushConfigIES m_PushConfig;
        PullConfigIES m_PullConfig;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            List<string> paths = currentDirectory.Split('\\').ToList();
            paths = paths.Take(paths.IndexOf(".ci") + 2).ToList();
            string ModelsPath = Path.Join(string.Join("\\", paths), "Models");
            FileSettings file = new FileSettings()
            {
                Directory = ModelsPath,
                FileName = "PushedModel.gem"
            };

            m_Adapter = new IESAdapter();

            m_PushConfig = new PushConfigIES()
            {
                PlanarTolerance = 1.0e-6,
                AngleTolerance = Tolerance.Angle,
                DecimalPlaces = 6,
                DistanceTolerance = Tolerance.MacroDistance,
                File = file
            };

            m_PullConfig = new PullConfigIES()
            {
                AngleTolerance = Tolerance.Angle,
                DistanceTolerance = Tolerance.Distance,
                File = file
            };
        }

        [SetUp]
        public void Setup()
        {
            Engine.Base.Compute.ClearCurrentEvents();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(m_PushConfig.File.GetFullFileName());
            var events = Engine.Base.Query.CurrentEvents();
            if (events.Any())
            {
                Console.WriteLine("BHoM Events raised during execution:");
                foreach (var ev in events)
                {
                    Console.WriteLine($"{ev.Type}: {ev.Message}");
                }
            }
        }

        [Test]
        [Description("Test to check if panels are being pushed correctly with 3D shades.")]
        public void PushPanelsWith3DShades()
        {
            //Arrange objects to push.
            FilterRequest request = new FilterRequest();
            m_PushConfig.ShadesAs3D = true;
            m_PullConfig.ShadesAs3D = true;
            m_PullConfig.PullOpenings = true;

            List<Panel> panels = Engine.Adapters.File.Compute.ReadFromJsonFile(Path.Combine(m_PullConfig.File.Directory, "Test Model 3D Shades.json"), true).Where(x => x.GetType() == typeof(Panel)).Cast<Panel>().ToList();

            int count = 0;
            foreach (Panel panel in panels)
            {
                panel.Name = count.ToString();
                count++;
            }

            //Push, then pull and sort objects by name.
            m_Adapter.Push(panels, actionConfig: m_PushConfig);

            List<IBHoMObject> objs = m_Adapter.Pull(request, actionConfig:m_PullConfig).Cast<IBHoMObject>().ToList();

            List<Space> pulledSpaces = BH.Engine.Environment.Query.Spaces(objs);
            List<Panel> pulledPanels = BH.Engine.Environment.Query.Panels(objs);

            pulledPanels = BH.Engine.Data.Query.OrderBy(pulledPanels, "Name");

            //Assert objects are identical to the ones pushed.
            for (int i = 0; i < panels.Count; i++)
            {
                pulledPanels[i].IsIdentical(panels[i]).Should().BeTrue($"The panel pulled with name: {pulledPanels[i].Name}, was not identical to the panel originally pushed with the same name.");
            }

            pulledPanels.Count().Should().Be(panels.Count, "Wrong number of panels pulled compared to expected.");
            pulledSpaces.Count.Should().Be(14, "Wrong number of spaces pulled compared to expected.");
        }

        [Test]
        [Description("Test to check if panels are being pushed correctly with 2D shades.")]
        public void PushPanelsWith2DShades()
        {
            //Arrange objects to push.
            FilterRequest request = new FilterRequest();
            m_PushConfig.ShadesAs3D = false;
            m_PullConfig.ShadesAs3D = false;
            m_PullConfig.PullOpenings = true;

            List<Panel> panels = Engine.Adapters.File.Compute.ReadFromJsonFile(Path.Combine(m_PullConfig.File.Directory, "Test Model 2D Shades.json"), true).Where(x => x.GetType() == typeof(Panel)).Cast<Panel>().ToList();

            int count = 0;
            foreach (Panel panel in panels)
            {
                panel.Name = count.ToString();
                count++;
            }

            //Push, then pull and sort objects by name.
            m_Adapter.Push(panels, actionConfig: m_PushConfig);

            List<IBHoMObject> objs = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<IBHoMObject>().ToList();

            List<Space> pulledSpaces = BH.Engine.Environment.Query.Spaces(objs);
            List<Panel> pulledPanels = BH.Engine.Environment.Query.Panels(objs);

            pulledPanels = BH.Engine.Data.Query.OrderBy(pulledPanels, "Name");

            //Assert objects are identical to the ones pushed.
            for (int i = 0; i < panels.Count; i++)
            {
                pulledPanels[i].IsIdentical(panels[i]).Should().BeTrue($"The panel pulled with name: {pulledPanels[i].Name}, was not identical to the panel originally pushed with the same name.");
            }

            pulledPanels.Count().Should().Be(panels.Count, "Wrong number of panels pulled compared to expected.");
            pulledSpaces.Count.Should().Be(9, "Wrong number of spaces pulled compared to expected."); //As the shades are being pushed, then pulled as 2D, they are not connected to any spaces, so there are 5 fewer spaces.
        }
    }
}
