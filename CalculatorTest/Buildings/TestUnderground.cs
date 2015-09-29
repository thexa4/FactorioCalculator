using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FactorioCalculator.Models.Factory.Physical;
using FactorioCalculator.Models;
using FactorioCalculator.Models.PlaceRoute;

namespace CalculatorTest.Buildings
{
    [TestClass]
    public class TestUnderground
    {
        [TestMethod]
        public void UndergroundCollisions()
        {
            var item = new Item("test");
            var lib = new Library();
            lib.AddItem(item);
            lib.Initialize();

            var flow = new UndergroundFlow(new ItemAmount(item, 1), Vector2.One, Depth.Normal, BuildingRotation.South);
            var grader = new SolutionGrader();
            
            var space = new Searchspace(new Vector2(4, 4));
            space = space.AddRoute(flow);
            Assert.AreEqual(0, flow.CalculateCost(space, grader));

            var flow2 = new UndergroundFlow(new ItemAmount(item, 1), Vector2.One, Depth.Fast, BuildingRotation.South);
            var newSpace = space.AddRoute(flow2);
            Assert.AreEqual(0, flow.CalculateCost(newSpace, grader));

            flow2 = new UndergroundFlow(new ItemAmount(item, 1), Vector2.One, Depth.Normal, BuildingRotation.South);
            newSpace = space.AddRoute(flow2);
            Assert.IsTrue(flow.CalculateCost(newSpace, grader) > 0);

            flow2 = new UndergroundFlow(new ItemAmount(item, 1), Vector2.One, Depth.Normal, BuildingRotation.North);
            newSpace = space.AddRoute(flow2);
            Assert.IsTrue(flow.CalculateCost(newSpace, grader) > 0);

            flow2 = new UndergroundFlow(new ItemAmount(item, 1), Vector2.One, Depth.Normal, BuildingRotation.East);
            newSpace = space.AddRoute(flow2);
            Assert.AreEqual(0, flow.CalculateCost(newSpace, grader));

            var building = new PhysicalFlowBuilding(new ItemAmount(item, 1), new Building("test1"), Vector2.One, BuildingRotation.North);
            newSpace = space.AddRoute(building);
            Assert.AreEqual(0, flow.CalculateCost(newSpace, grader));
        }
    }
}
