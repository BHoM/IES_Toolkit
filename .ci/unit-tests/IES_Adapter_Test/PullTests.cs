﻿using BH.Adapter.IES;
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

namespace BH.Tests.Adapter.IES
{
    public class PullTests
    {

        IESAdapter m_Adapter;
        PullConfigIES m_PullConfig;

        [OneTimeSetUp]
        [Description("On loading the tests, instantiate an adapter and pull config to be used in all tests, and get the file path and name of the model being used.")]
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
        [Description("When running a new test, clear any errors and warnings that have occured in previous test.")]
        public void Setup()
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
        }

        [TearDown]
        [Description("If any events occurred during a test, log the types and messages of the event in the console so that it is easier to debug.")]
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
        [Description("Test pulling panels with openings.")]
        public void PullPanelsWithOpenings()
        {
            //arrange request and pull config for pulling panels with openings
            FilterRequest request = new FilterRequest() { Type = typeof(Panel) };
            m_PullConfig.PullOpenings = true;

            //pull all panels from the model, including openings.
            List<Panel> panels = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<Panel>().ToList();
            //put all the panels that are shades into a list.
            List<Panel> shades = new List<Panel>();
            foreach (Panel panel in panels)
            {
                if (panel.IsShade())
                {
                    shades.Add(panel);
                }
            }

            //assert correct values.
            panels.Count.Should().Be(149, "Wrong number of panels pulled compared to expected."); //lol the number should actually be around 46 but the model being used is bad, and has duplicated panels.
            panels.OpeningsFromElements().Count.Should().Be(39, "Wrong number of openings pulled compared to expected."); //perhaps should be 13
            shades.Count.Should().Be(63, "Wrong number of shades being pulled compared to expected."); //perhaps should be 31
        }

        [Test]
        [Description("Test pulling spaces.")]
        public void PullSpaces()
        {
            //arrange request for pulling spaces.
            FilterRequest request = new FilterRequest() { Type = typeof(Space) };

            //pull all spaces from the model.
            List<Space> spaces = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<Space>().ToList();

            //assert correct values.
            spaces.Count.Should().Be(14, "Wrong number of panels pulled compared to expected."); //probably correct despite model issues
        }

        [Test]
        [Description("Test pulling panels without openings.")]
        public void PullPanelsWithoutOpenings()
        {
            //arrange request and pull config for pulling panels without openings.
            FilterRequest request = new FilterRequest() { Type = typeof(Panel) };
            m_PullConfig.PullOpenings = false;

            //pull all panels from the model, excluding openings.
            List<Panel> panels = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<Panel>().ToList();
            //add all the panels that are shades to a list.
            List<Panel> shades = new List<Panel>();
            foreach (Panel panel in panels) 
            {
                if (panel.IsShade())
                {
                    shades.Add(panel);
                }
            }

            //assert correct values.
            panels.Count.Should().Be(149, "Wrong number of panels pulled compared to expected."); //perhaps should be around 46
            panels.OpeningsFromElements().Count.Should().Be(0, "Wrong number of openings pulled compared to expected."); //should still be 0
            shades.Count.Should().Be(63, "Wrong number of shades being pulled compared to expected."); //perhaps should be 31
        }

        [Test]
        [Description("Test pulling all data from the model.")]
        public void PullFullModel()
        {
            FilterRequest request = new FilterRequest() { };
            m_PullConfig.PullOpenings = false;

            List<IBHoMObject> objects = m_Adapter.Pull(request, actionConfig: m_PullConfig).Cast<IBHoMObject>().ToList();
            List<Space> spaces = BH.Engine.Environment.Query.Spaces(objects).Cast<Space>().ToList();
            List<Panel> panels = BH.Engine.Environment.Query.Panels(objects).Cast<Panel>().ToList();
            //add all the panels that are shades to a list.
            List<Panel> shades = new List<Panel>();
            foreach (Panel panel in panels)
            {
                if (panel.IsShade())
                {
                    shades.Add(panel);
                }
            }

            panels.Count.Should().Be(149, "Wrong number of panels pulled compared to expected.");
            panels.OpeningsFromElements().Count.Should().Be(0, "Wrong number of openings pulled compared to expected.");
            spaces.Count.Should().Be(14, "Wrong number of panels pulled compared to expected.");
            shades.Count.Should().Be(63, "Wrong number of shades pulled compared to expected.");
        }
        
        //TODO - check that openings bool is working as expected.
        //TODO - check that shades are being pulled.
    }
}
