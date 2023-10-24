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
            FilterRequest request = new FilterRequest();
            m_PushConfig.ShadesAs3D = true;
            m_PullConfig.ShadesAs3D = true;
            m_PullConfig.PullOpenings = true;
            List<Panel> panels = Engine.Adapters.File.Compute.ReadFromJsonFile(Path.Combine(m_PullConfig.File.Directory, "Test Model 3D Shades.json"), true).Where(x => x.GetType() == typeof(Panel)).Cast<Panel>().ToList();

            m_Adapter.Push(panels, actionConfig: m_PushConfig);

            List<IBHoMObject> objs = m_Adapter.Pull(request, actionConfig:m_PullConfig).Cast<IBHoMObject>().ToList();

            List<Space> pulledSpaces = BH.Engine.Environment.Query.Spaces(objs).Cast<Space>().ToList();
            List<Panel> pulledPanels = BH.Engine.Environment.Query.Panels(objs).Cast<Panel>().ToList();

            List<Panel> shades = new List<Panel>();
            foreach (Panel panel in pulledPanels)
            {
                if (panel.IsShade())
                {
                    shades.Add(panel);
                }
            }

            pulledPanels.Count.Should().Be(121, "Wrong number of panels pulled compared to expected.");
            pulledPanels.OpeningsFromElements().Count.Should().Be(33, "Wrong number of openings pulled compared to expected.");
            shades.Count.Should().Be(62, "Wrong number of shades being pulled compared to expected.");
            pulledSpaces.Count.Should().Be(14, "Wrong number of spaces pulled compared to expected.");
        }

        [Test]
        [Description("Test to check if panels are being pushed correctly with 2D shades.")]
        public void PushPanelsWith2DShades()
        {
            FilterRequest request = new FilterRequest();
            m_PushConfig.ShadesAs3D = false;
            m_PullConfig.ShadesAs3D = false;
            m_PullConfig.PullOpenings = true;
            List<Panel> panels = Engine.Adapters.File.Compute.ReadFromJsonFile(Path.Combine(m_PullConfig.File.Directory, "Test Model 2D Shades.json"), true).Where(x => x.GetType() == typeof(Panel)).Cast<Panel>().ToList();

            m_Adapter.Push(panels, actionConfig: m_PushConfig);

            List<IBHoMObject> objs = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<IBHoMObject>().ToList();

            List<Space> pulledSpaces = BH.Engine.Environment.Query.Spaces(objs).Cast<Space>().ToList();
            List<Panel> pulledPanels = BH.Engine.Environment.Query.Panels(objs).Cast<Panel>().ToList();

            List<Panel> shades = new List<Panel>();
            foreach (Panel panel in pulledPanels)
            {
                if (panel.IsShade())
                {
                    shades.Add(panel);
                }
            }

            pulledPanels.Count.Should().Be(121, "Wrong number of panels pulled compared to expected.");
            pulledPanels.OpeningsFromElements().Count.Should().Be(33, "Wrong number of openings pulled compared to expected.");
            shades.Count.Should().Be(62, "Wrong number of shades being pulled compared to expected.");
            pulledSpaces.Count.Should().Be(9, "Wrong number of spaces pulled compared to expected.");
        }


    }
}