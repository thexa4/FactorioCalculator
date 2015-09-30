using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FactorioCalculator.Models.PlaceRoute;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;

namespace CalculatorTest
{
    [TestClass]
    public class SearchspaceTest
    {
        [TestMethod]
        public void SearchstateCollisions()
        {
            var state = new Searchspace(new Vector2(4, 4));

            var item = new Item("test");
            var building = new Building("test");
            building.Size = 2 * Vector2.One;
            var flow = new FlowBuilding(new ItemAmount(item, 1), building, Vector2.One, BuildingRotation.North);

            var newState = state.AddRoute(flow);

            Assert.AreEqual(0, state.CalculateCollisions(Vector2.One).Count());
            Assert.AreEqual(1, newState.CalculateCollisions(Vector2.One).Count());

            Assert.AreEqual(0, newState.CalculateCollisions(Vector2.Zero).Count());
            Assert.AreEqual(1, newState.CalculateCollisions(Vector2.One, 2 * Vector2.One).Count());
        }
    }
}
