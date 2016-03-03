using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorTest.Buildings
{
    [TestClass]
    public class TestProductionBuilding
    {
        [TestMethod]
        public void TestRotations()
        {
            Building b = new Building("test");
            b.Size = Vector2.One * 3;
            b.AddCraftingCategory("test");
            b.ProductionSpeed = 2;
            b.IngredientCount = 3;
            b.AddFluidBox(new FluidBox(true, Vector2.Zero));

            Recipe r = new Recipe("test");
            r.CraftingCategory = "test";

            var phys1 = new ProductionBuilding(r, 1, b, Vector2.Zero, BuildingRotation.North);
            var phys2 = new ProductionBuilding(r, 1, b, Vector2.Zero, BuildingRotation.East);
            var phys3 = new ProductionBuilding(r, 1, b, Vector2.Zero, BuildingRotation.South);
            var phys4 = new ProductionBuilding(r, 1, b, Vector2.Zero, BuildingRotation.West);

            Assert.AreEqual(Vector2.Zero, phys1.FluidBoxes.First().Position);
            Assert.AreEqual(new Vector2(2, 0), phys2.FluidBoxes.First().Position);
            Assert.AreEqual(new Vector2(2, 2), phys3.FluidBoxes.First().Position);
            Assert.AreEqual(new Vector2(0, 2), phys4.FluidBoxes.First().Position);
        }
    }
}
