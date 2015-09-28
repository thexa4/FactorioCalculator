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
        public SearchSpace Preview { get; set; }
        public double LowestCost { get; set; }

        public SolidRouter SolidRouter { get; set; }
        public FluidRouter FluidRouter { get; set; }

        public SolutionGrader Grader { get; set; }
        public SolutionParameters BestParameters { get; set; }
        public double Temperature { get; set; }

        public List<Tuple<SolutionParameters, SearchSpace, double>> Pool { get; set; }

        Random _random = new Random();

        public SolutionGenerator(RecipeGraph factory)
        {
            Factory = factory;
            Grader = new SolutionGrader();
        }

        public void Initialize()
        {
            Temperature = 1;
            Pool = new List<Tuple<SolutionParameters, SearchSpace, double>>();
            for (int i = 0; i < 10; i++)
                Pool.Add(CreateRandom());

            var best = Pool.OrderBy((g) => g.Item3).First();
            BestParameters = best.Item1;
            BestState = best.Item2;
            LowestCost = best.Item3;
        }

        public Tuple<SolutionParameters, SearchSpace, double> CreateRandom()
        {
            for (int i = 0; i < 32; i++)
            {
                var guess = SolutionParameters.FromFactory(_random.Next(10, 20), _random.Next(10, 20), Factory);
                try
                {
                    var solution = GenerateSolution(guess);
                    var cost = Grader.CostForSolution(solution);
                    return new Tuple<SolutionParameters, SearchSpace, double>(guess, solution, cost);
                }
                catch (InvalidOperationException) { }
                catch (IndexOutOfRangeException) { }
            }
            return null;
        }

        public double Step()
        {
            var mutations = Pool.AsParallel().Select((parent) =>
            {
                try {
                var child = parent.Item1.Modify(Temperature);
                var solution = GenerateSolution(child);
                var cost = Grader.CostForSolution(solution);

                return new Tuple<SolutionParameters, SearchSpace, double>(child, solution, cost);
                }
                catch (InvalidOperationException) { }
                catch (IndexOutOfRangeException) { }
                return new Tuple<SolutionParameters, SearchSpace, double>(null, new SearchSpace(), -1);
            }).Where((g) => g.Item1 != null);
            var additions = Enumerable.Range(0, 5).AsParallel().Select((i) => CreateRandom());
            var newPool = Pool.Concat(mutations).Concat(additions).OrderBy((g) => g.Item3);
            Pool = newPool.Take(10).ToList();

            var best = Pool.OrderBy((g) => g.Item3).First();
            BestParameters = best.Item1;
            BestState = best.Item2;
            LowestCost = best.Item3;

            var iterBest = additions.Concat(mutations).OrderBy((g) => g.Item3).First();

            Temperature *= 0.5;

            return iterBest.Item3;
        }

        public SolutionParameters Improve(SolutionParameters start, double cost, double temperature, int tries = 8)
        {
            var guesses = Enumerable.Range(0, tries).Select((i) => start.Modify(temperature));
            var evaluations = guesses.AsParallel().Select((guess) =>
            {

                return new Tuple<SolutionParameters, double>(guess, 0);
            });
            return start;
        }

        public SearchSpace GenerateSolution(SolutionParameters parameters)
        {
            var result = new SearchSpace(new Vector2(parameters.Width, parameters.Height));

            var buildings = parameters.BuildingPositions.Select((kvp) => new ProductionBuilding(kvp.Key.Recipe, kvp.Key.Amount, kvp.Key.Building, kvp.Value.Item1, kvp.Value.Item2));
            foreach (var building in buildings)
                result = result.AddComponent(building);

            var belts = new Dictionary<Item, List<RoutingCoord>>();
            List<Tuple<IStep, Item, bool>> todo = parameters.Connections.ToList();

            while (todo.Count != 0)
            {
                Preview = result;

                var connection = todo[0];
                todo.RemoveAt(0);

                var isDestination = connection.Item3;

                var prev = result;

                if (belts.ContainsKey(connection.Item2))
                {
                    IEnumerable<RoutingCoord> sources = new RoutingCoord[] { };
                    IEnumerable<RoutingCoord> destinations = new RoutingCoord[] { };

                    //TODO: fix itemAmount
                    //TODO: fix fluids
                    if (connection.Item2.ItemType == ItemType.Solid)
                    {
                        if (isDestination)
                        {
                            sources = belts[connection.Item2];
                            destinations = BuildingToPlacables(connection.Item1, parameters);
                            belts[connection.Item2].AddRange(destinations);
                        }
                        else
                        {
                            sources = BuildingToPlacables(connection.Item1, parameters);
                            destinations = belts[connection.Item2];
                            belts[connection.Item2].AddRange(sources);
                        }

                        result = SolidRouter.Route(new ItemAmount(connection.Item2, 1), result, sources, destinations);
                    }
                }
                else
                {
                    var firstMatching = todo.Where((t) => t.Item2 == connection.Item2 && t.Item3 != connection.Item3).First();
                    todo.Remove(firstMatching);
                    var both = new Tuple<IStep, Item, bool>[] { connection, firstMatching };

                    var source = both.Where((c) => c.Item3 == false).First();
                    var destination = both.Where((c) => c.Item3 == true).First();

                    var sources = BuildingToPlacables(source.Item1, parameters);
                    var destinations = BuildingToPlacables(destination.Item1, parameters);

                    result = SolidRouter.Route(new ItemAmount(connection.Item2, 1), result, sources, destinations);
                }

                if (!belts.ContainsKey(connection.Item2))
                    belts[connection.Item2] = new List<RoutingCoord>();
                belts[connection.Item2].AddRange(RoutesToCoords(prev, result));
            }

            return result;
        }

        private IEnumerable<RoutingCoord> RoutesToCoords(SearchSpace previous, SearchSpace now)
        {
            var newRoutes = now.Routes.Skip(previous.Routes.Count);
            foreach (var route in newRoutes)
            {
                if (route.Step.Building.Name.EndsWith("-transport-belt"))
                    yield return new RoutingCoord(route.Position, RoutingCoord.CoordType.Splitter, route.Step.Rotation);
            }
        }

        public IEnumerable<RoutingCoord> BuildingToPlacables(IStep step, SolutionParameters parameters)
        {
            if (step is ProductionStep)
            {
                var building = step as ProductionStep;
                var physical = new ProductionBuilding(building.Recipe, building.Amount, building.Building, parameters.BuildingPositions[building].Item1, parameters.BuildingPositions[building].Item2);
                for (int x = 0; x < physical.Size.X; x++)
                    for (int y = 0; y < physical.Size.Y; y++)
                        yield return new RoutingCoord(physical.Position + new Vector2(x, y), RoutingCoord.CoordType.PlacedItem, BuildingRotation.North);
                yield break;
            }

            if (step is SourceStep)
            {
                var pos = parameters.SourcePositions[step as SourceStep];
                var rotation = InwardVectorForEdge(pos, new Vector2(parameters.Width, parameters.Height));
                yield return new RoutingCoord(pos, RoutingCoord.CoordType.Belt, rotation);
                yield break;
            }

            if (step is SinkStep)
            {
                var pos = parameters.SinkPositions[step as SinkStep];
                var rotation = InwardVectorForEdge(pos, new Vector2(parameters.Width, parameters.Height)).Invert();
                yield return new RoutingCoord(pos, RoutingCoord.CoordType.Belt, rotation);
                yield break;
            }
        }

        private BuildingRotation InwardVectorForEdge(Vector2 position, Vector2 size)
        {
            if (position.X == 0)
                return BuildingRotation.East;
            if (position.Y == 0)
                return BuildingRotation.South;
            if (position.X == size.X - 1)
                return BuildingRotation.West;
            return BuildingRotation.North;
        }

        public Tuple<SearchSpace, Boolean> GenerateOption()
        {
            var result = new SearchSpace(new Vector2(_random.Next(8,25), _random.Next(8,25)));
            return new Tuple<SearchSpace, Boolean>(result, true);

            /*
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
            */
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
