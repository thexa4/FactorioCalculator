using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class SolutionGenerator
    {
        public RecipeGraph Factory { get; set; }
        public SearchSpace BestState { get; set; }
        public double LowestCost { get; set; }

        public SolidRouter SolidRouter { get; set; }
        public FluidRouter FluidRouter { get; set; }

        public SolutionGrader Grader { get; set; }

        Random _random = new Random();

        public SolutionGenerator(RecipeGraph factory)
        {
            Factory = factory;
            Grader = new SolutionGrader();
            LowestCost = double.MaxValue;
        }

        public double Step()
        {
            object lockObject = new object();
            var min = double.MaxValue;

            Parallel.For(0, 16, (i) =>
            {
                try
                {
                    SearchSpace solution = new SearchSpace();
                    var score = Double.MaxValue;
                    for (int j = 0; j < 32; j++)
                    {
                        var tuple = GenerateOption();
                        
                        if(tuple.Item2) {
                            solution = tuple.Item1;
                            score = Grader.CostForSolution(solution);
                            break;
                        }
                    }

                    Console.Write(".");
                    lock (lockObject)
                    {
                        if (score < LowestCost)
                        {
                            LowestCost = score;
                            BestState = solution;
                        }

                        if (score < min)
                            min = score;
                    }
                }
                catch (InvalidOperationException) { }
                catch (IndexOutOfRangeException) { }
            });
            return min;
        }

        public Tuple<SearchSpace, Boolean> GenerateOption()
        {
            var result = new SearchSpace(new Vector2(_random.Next(8,25), _random.Next(8,25)));

            var origins = new Dictionary<Item, List<ProductionBuilding>>();
            var destinations = new Dictionary<Item, List<ProductionBuilding>>();
            var buildingCache = new Dictionary<ProductionStep, ProductionBuilding>();

            var belts = new Dictionary<Item, List<FlowBuilding>>();

            var buildings = Factory.Transformations;
            foreach (ProductionStep building in buildings)
            {
                var physical = new ProductionBuilding(building.Recipe, building.Amount, building.Building, new Vector2(_random.Next(1, (int)result.Size.X - (int)building.Building.Size.X - 1), _random.Next(1, (int)result.Size.Y - (int)building.Building.Size.Y - 1)), (BuildingRotation)_random.Next(0, 4));
                buildingCache.Add(building, physical);
                foreach (var prev in building.Previous)
                {
                    if (prev is FlowStep)
                    {
                        var item = ((FlowStep)prev).Item.Item;
                        if(!destinations.ContainsKey(item))
                            destinations.Add(item, new List<ProductionBuilding>());
                        destinations[item].Add(physical);
                    }
                }

                result = result.AddComponent(physical);
            }

            if (Grader.CostForSolution(result) > 2 * result.Size.X * result.Size.Y)
                return new Tuple<SearchSpace, bool>(result, false);

            foreach(var source in Factory.InputNodes)
            {
                if (!origins.ContainsKey(source.Item.Item))
                    origins.Add(source.Item.Item, new List<ProductionBuilding>());
            }
            foreach(var sink in Factory.OutputNodes)
            {
                if (!destinations.ContainsKey(sink.Item.Item))
                    destinations.Add(sink.Item.Item, new List<ProductionBuilding>());
            }

            foreach (FlowStep step in Factory.Resources)
            {
                var item = step.Item.Item;

                foreach (IStep building in step.Previous)
                {
                    if (!(building is ProductionStep))
                        continue;
                    if (!origins.ContainsKey(item))
                        origins.Add(item, new List<ProductionBuilding>());
                    origins[item].Add(buildingCache[building as ProductionStep]);
                }
            }

            foreach (var item in origins.Keys)
            {
                if (!destinations.ContainsKey(item))
                    continue;

                if (item.ItemType == ItemType.Solid)
                {
                    var beltList = new List<Vector2>();

                    if (origins[item].Count == 0)
                    {
                        beltList.Add(GenerateEdgeCoord(result, false));
                    }
                    else
                    {
                        var from = origins[item][0];
                        for (int x = 0; x < from.Size.X; x++)
                            for (int y = 0; y < from.Size.Y; y++)
                                beltList.Add(from.Position + new Vector2(x, y));
                    }

                    if (destinations[item].Count == 0)
                    {
                        result = SolidRouter.Route(new ItemAmount(item, 1), result, beltList, new List<Vector2>() { GenerateEdgeCoord(result, false) });
                    }
                    else
                    {
                        foreach (var to in destinations[item])
                        {
                            var routePos = result.Routes.Count;
                            var dests = new List<Vector2>();

                            for (int x = 0; x < to.Size.X; x++)
                                for (int y = 0; y < to.Size.Y; y++)
                                    dests.Add(to.Position + new Vector2(x, y));

                            var prevCost = Grader.CostForSolution(result);
                            result = SolidRouter.Route(new ItemAmount(item, 1), result, beltList, dests);

                            var delta = Grader.CostForSolution(result) - prevCost;
                            Grader.CostForSolution(result);
                            var newRoutes = result.Routes.Skip(routePos);
                            foreach (var route in newRoutes)
                                if (route.Step.Building.Name.EndsWith("transport-belt"))
                                    beltList.Add(route.Position);
                        }
                    }
                }
            }
            return new Tuple<SearchSpace,bool>(result, true);
        }

        private Vector2 GenerateEdgeCoord(SearchSpace space, bool outside)
        {
            if (_random.NextDouble() > 0.5)
            {
                if (_random.NextDouble() > 0.5)
                    return new Vector2(_random.Next(0, (int)space.Size.X), outside ? -1 : 0);
                else
                    return new Vector2(_random.Next(0, (int)space.Size.X), space.Size.Y - (outside ? 1 : 0));
            }
            else
            {
                if (_random.NextDouble() > 0.5)
                    return new Vector2(outside ? -1 : 0, _random.Next(0, (int)space.Size.Y));
                else
                    return new Vector2(space.Size.X - (outside ? 1 : 0), _random.Next(0, (int)space.Size.Y));
            }
        }
    }
}
