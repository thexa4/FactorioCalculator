using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FactorioCalculator.Models.PlaceRoute;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using FactorioCalculator.Helper;

namespace CalculatorTest
{
    [TestClass]
    public class SolutionParametersTest
    {
        [TestMethod]
        public void SolutionEdges()
        {
            for(int x = 2; x < 16; x++)
                for(int y = 2; y < 16; y++)
                {
                    SolutionParameters parm = new SolutionParameters(x, y);
                    Vector2 prev = new Vector2(-1, 0);

                    for (int i = -2 * (x + y) + 4; i < 2 * (x + y) - 4; i++)
                    {
                        var newPos = parm.IndexToBound(i);
                        if (i >= 0)
                            Assert.AreEqual(i, parm.BoundToIndex(newPos), string.Format("{0}, {1}, {2}", x, y, i));
                        Assert.AreEqual(1, (prev - newPos).DistanceSquared(), string.Format("{0}, {1}, {2}", x, y, i));
                        prev = newPos;
                    }
                }

        }

        [TestMethod]
        public void SolutionModify()
        {
            var item1 = new Item("a");
            var item2 = new Item("b");
            Recipe r = new Recipe("");
            r.AddIngredient(new ItemAmount(item1, 1));
            r.AddResult(new ItemAmount(item2, 1));
            r.CraftingCategory = "test";

            Building b = new Building("");
            b.AddCraftingCategory("test");
            b.IngredientCount = 4;
            b.ProductionSpeed = 1;

            Library l = new Library();

            l.AddItem(item1);
            l.AddItem(item2);
            l.AddRecipe(r);
            l.AddBuilding(b);
            l.Initialize();

            var coarseGraph = RecipeGraph.FromLibrary(l, new Item[] { item1 }, new ItemAmount[] { new ItemAmount(item2, 0.01) }, (a) => 1);
            var denseGraph = TrivialSolutionFactory.CreateFactory(coarseGraph);

            var parameters = SolutionParameters.FromFactory(12, 12, denseGraph);

            Assert.AreEqual(parameters, parameters.Modify(0));
        }
    }
}
