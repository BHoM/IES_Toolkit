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
                AngleTolerance = BH.oM.Geometry.Tolerance.Angle,
                DecimalPlaces = 6,
                DistanceTolerance = BH.oM.Geometry.Tolerance.MacroDistance,
                File = file
            };
            m_PullConfig = new PullConfigIES()
            {
                AngleTolerance = BH.oM.Geometry.Tolerance.Angle,
                DistanceTolerance = BH.oM.Geometry.Tolerance.Distance,
                File = file
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
            File.Delete(m_PushConfig.File.GetFullFileName());
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
        [Description("Test to check if panels are being pushed correctly with openings.")]
        public void PushPanelsWithOpenings()
        {
            
        }
    }
}
