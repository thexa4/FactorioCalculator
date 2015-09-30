using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FactorioCalculator.Models.PlaceRoute;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace CalculatorTest
{
    [TestClass]
    public class SolidRouterTest
    {
        [TestMethod]
        public void RouteSource()
        {
            SolidRouter r = new SolidRouter(
                new Building("basic-transport-belt"),
                new Building("basic-transport-belt-to-ground"),
                new Building("fast-transport-belt-to-ground"),
                new Building("express-transport-belt-to-ground"),

                new Building("basic-inserter"),
                new Building("fast-inserter"),
                new Building("long-handed-inserter"),

                new SolutionGrader()
            );
            Building assembler = new Building("assembler");
            assembler.Size = new Vector2(3,3);
            assembler.IngredientCount = 3;
            assembler.ProductionSpeed = 1;
            assembler.AddCraftingCategory("test");
            Recipe recipe = new Recipe("dummy");
            recipe.CraftingCategory = "test";
            Item item = new Item("dummy");

            Library l = new Library();
            l.AddBuilding(assembler);
            l.AddItem(item);
            l.AddRecipe(recipe);

            l.Initialize();

            Searchspace s = new Searchspace(new Vector2(13, 16));
            var physical = new ProductionBuilding(recipe, 1, assembler, new Vector2(6, 6), BuildingRotation.North);
            s = s.AddComponent(physical);
            
            var dict = new Dictionary<ProductionStep, Tuple<Vector2, BuildingRotation>>();
            dict.Add(physical, new Tuple<Vector2,BuildingRotation>(physical.Position, physical.Rotation));
            var param = new SolutionParameters(13, 16, ImmutableDictionary<SourceStep, Vector2>.Empty, ImmutableDictionary<SinkStep, Vector2>.Empty, dict.ToImmutableDictionary(), ImmutableList<Tuple<IStep, Item, bool>>.Empty);

            var dests = SolutionGenerator.BuildingToPlaceables(physical, param);

            var result = r.Route(new ItemAmount(item, 0.01), s, new RoutingCoordinate[] { new RoutingCoordinate(new Vector2(12, 2), RoutingCoordinate.CoordinateType.Belt, BuildingRotation.West) }, dests);
        }

        [TestMethod]
        public void RouteSink()
        {
            SolidRouter r = new SolidRouter(
                new Building("belt"),
                new Building("beltground3"),
                new Building("beltground2"),
                new Building("beltground1"),

                new Building("inserter1"),
                new Building("inserter2"),
                new Building("longInserter"),

                new SolutionGrader()
            );

            Building assembler = new Building("assembler");
            assembler.Size = new Vector2(3,3);
            assembler.IngredientCount = 3;
            assembler.ProductionSpeed = 1;
            assembler.AddCraftingCategory("test");
            Recipe recipe = new Recipe("dummy");
            recipe.CraftingCategory = "test";
            Item item = new Item("dummy");

            Library l = new Library();
            l.AddBuilding(assembler);
            l.AddItem(item);
            l.AddRecipe(recipe);

            l.Initialize();

            Searchspace s = new Searchspace(new Vector2(13, 10));
            var physical = new ProductionBuilding(recipe, 1, assembler, new Vector2(7, 3), BuildingRotation.North);
            s = s.AddComponent(physical);
            
            var dict = new Dictionary<ProductionStep, Tuple<Vector2, BuildingRotation>>();
            dict.Add(physical, new Tuple<Vector2,BuildingRotation>(physical.Position, physical.Rotation));
            var param = new SolutionParameters(13, 10, ImmutableDictionary<SourceStep, Vector2>.Empty, ImmutableDictionary<SinkStep, Vector2>.Empty, dict.ToImmutableDictionary(), ImmutableList<Tuple<IStep, Item, bool>>.Empty);

            var dests = SolutionGenerator.BuildingToPlaceables(physical, param);

            var result = r.Route(new ItemAmount(item, 0.01), s, dests, new RoutingCoordinate[] { new RoutingCoordinate(new Vector2(7, 9), RoutingCoordinate.CoordinateType.Belt, BuildingRotation.South) });
        }
    }
}
