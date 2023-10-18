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

namespace BH.Tests.Adapter.IES
{
    public class PullTests
    {

        IESAdapter m_Adapter;
        PullConfigIES m_PullConfig;

        [OneTimeSetUp]
        public void OneTimeSetUp() 
        {
            //starts the IES adapter and instantiates the ActionConfig for pulling IES data.
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            List<string> paths = currentDirectory.Split('\\').ToList();
            paths = paths.Take(paths.IndexOf(".ci") + 2).ToList();
            string GEMPath = Path.Join(string.Join("\\", paths), "Models");
            m_Adapter = new IESAdapter();
            m_PullConfig = new PullConfigIES()
            {
                ShadesAs3D = true,
                PullOpenings = true,
                AngleTolerance = BH.oM.Geometry.Tolerance.Angle,
                DistanceTolerance = BH.oM.Geometry.Tolerance.MacroDistance,
                File = new FileSettings() 
                {
                    Directory = GEMPath,
                    FileName = "IES Pull Test Model.gem"
                },
            };
        }

        [SetUp]
        public void Setup()
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
        }

        [TearDown]
        public void TearDown()
        {
            var events = BH.Engine.Base.Query.CurrentEvents();
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
        [Description("Test pulling panels.")]
        public void PullPanels()
        {
            FilterRequest request = new FilterRequest() { Type=typeof(Panel) };

            List<Panel> panels = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<Panel>().ToList();

            panels.Count.Should().Be(149, "Wrong number of panels pulled compared to expected.");
        }

        [Test]
        [Description("Test pulling spaces.")]
        public void PullSpace()
        {
            FilterRequest request = new FilterRequest() { Type = typeof(Space) };

            List<Space> spaces = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<Space>().ToList();

            spaces.Count.Should().Be(14, "Wrong number of panels pulled compared to expected.");
        }
    }
}
