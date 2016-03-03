using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using FactorioCalculator.Helper;
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
        public Searchspace BestState { get; set; }
        public Searchspace Preview { get; set; }
        public double LowestCost { get; set; }

        public SolidRouter SolidRouter { get; set; }
        public FluidRouter FluidRouter { get; set; }

        public SolutionGrader Grader { get; set; }
        public SolutionParameters BestParameters { get; set; }
        public double Temperature { get; set; }

        private List<Tuple<SolutionParameters, Searchspace, double>> _pool;
        public IEnumerable<Tuple<SolutionParameters, Searchspace, double>> Pool { get { return _pool; } }

        Random _random = new Random();

        public SolutionGenerator(RecipeGraph factory)
        {
            Factory = factory;
            Grader = new SolutionGrader();
        }

        public void Initialize()
        {
            Temperature = 1;
            _pool = new List<Tuple<SolutionParameters, Searchspace, double>>();
            for (int i = 0; i < 10; i++)
                _pool.Add(CreateRandom());

            var best = _pool.OrderBy((g) => g.Item3).First();
            BestParameters = best.Item1;
            BestState = best.Item2;
            LowestCost = best.Item3;
        }

        public Tuple<SolutionParameters, Searchspace, double> CreateRandom()
        {
            for (int i = 0; i < 32; i++)
            {
                var guess = SolutionParameters.FromFactory(_random.Next(10, 28), _random.Next(10, 28), Factory);
                try
                {
                    var solution = GenerateSolution(guess);
                    var cost = Grader.CostForSolution(solution);
                    return new Tuple<SolutionParameters, Searchspace, double>(guess, solution, cost);
                }
                catch (InvalidOperationException) { }
                catch (IndexOutOfRangeException) { }
            }
            return null;
        }

        public double Step()
        {
            var mutations = _pool.Select((parent) =>
            {
                try
                {
                    var child = parent.Item1.Modify(Temperature);
                    var solution = GenerateSolution(child);
                    var cost = Grader.CostForSolution(solution);

                    return new Tuple<SolutionParameters, Searchspace, double>(child, solution, cost);
                }
                catch (InvalidOperationException) { }
                catch (IndexOutOfRangeException) { }
                return new Tuple<SolutionParameters, Searchspace, double>(null, new Searchspace(), -1);
            }).Where((g) => g.Item1 != null);
            var additions = Enumerable.Range(0, 2).Select((i) => CreateRandom());
            var newPool = _pool.Concat(mutations).Concat(additions).OrderBy((g) => g.Item3);
            _pool = newPool.Take(10).ToList();

            var best = _pool.OrderBy((g) => g.Item3).First();
            BestParameters = best.Item1;
            BestState = best.Item2;
            LowestCost = best.Item3;

            var iterBest = additions.Concat(mutations).OrderBy((g) => g.Item3).First();

            Temperature *= 0.5;
            if (Temperature < 0.02)
                Temperature = 1;

            return iterBest.Item3;
        }

        public Searchspace GenerateSolution(SolutionParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            var result = new Searchspace(new Vector2(parameters.Width, parameters.Height));

            var buildings = parameters.BuildingPositions.Select((kvp) => new ProductionBuilding(kvp.Key.Recipe, kvp.Key.Amount, kvp.Key.Building, kvp.Value.Item1, kvp.Value.Item2));
            foreach (var building in buildings)
                result = result.AddComponent(building);

            var belts = new Dictionary<Item, List<RoutingCoordinate>>();
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
                    IEnumerable<RoutingCoordinate> sources = new RoutingCoordinate[] { };
                    IEnumerable<RoutingCoordinate> destinations = new RoutingCoordinate[] { };

                    //TODO: fix itemAmount
                    //TODO: fix fluids
                    if (connection.Item2.ItemType == ItemType.Solid)
                    {
                        if (isDestination)
                        {
                            sources = belts[connection.Item2];
                            destinations = BuildingToPlaceables(connection.Item1, parameters);
                        }
                        else
                        {
                            sources = BuildingToPlaceables(connection.Item1, parameters);
                            destinations = belts[connection.Item2];
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

                    if (connection.Item2.ItemType == ItemType.Solid)
                    {
                        var sources = BuildingToPlaceables(source.Item1, parameters);
                        var destinations = BuildingToPlaceables(destination.Item1, parameters);
                        result = SolidRouter.Route(new ItemAmount(connection.Item2, 1), result, sources, destinations);
                    }

                    if (connection.Item2.ItemType == ItemType.Fluid)
                    {
                        var sources = BuildingToPipes(source.Item1, parameters, connection.Item2);
                        var destinations = BuildingToPipes(destination.Item1, parameters, connection.Item2);
                        result = FluidRouter.Route(new ItemAmount(connection.Item2, 1), result, sources, destinations);
                    }
                }

                if (!belts.ContainsKey(connection.Item2))
                    belts[connection.Item2] = new List<RoutingCoordinate>();
                belts[connection.Item2].AddRange(RoutesToCoords(prev, result));
            }

            return result;
        }

        private IEnumerable<RoutingCoordinate> RoutesToCoords(Searchspace previous, Searchspace now)
        {
            var newRoutes = now.Routes.Skip(previous.Routes.Count);
            foreach (var route in newRoutes)
            {
                if (route.Step is Belt)
                    yield return new RoutingCoordinate(route.Position, RoutingCoordinate.CoordinateType.Splitter, route.Step.Rotation);
            }
        }

        public static IEnumerable<RoutingCoordinate> BuildingToPipes(IStep step, SolutionParameters parameters, Item item)
        {
            var sourceStep = step as SourceStep;
            if (sourceStep != null)
            {
                var pos = parameters.SourcePositions[sourceStep];
                var rotation = InwardDirectionForEdge(pos, new Vector2(parameters.Width, parameters.Height));
                yield return new RoutingCoordinate(pos, RoutingCoordinate.CoordinateType.PipeToGround, rotation);
            }

            var sinkStep = step as SinkStep;
            if (sinkStep != null)
            {
                var pos = parameters.SinkPositions[sinkStep];
                var rotation = InwardDirectionForEdge(pos, new Vector2(parameters.Width, parameters.Height));
                yield return new RoutingCoordinate(pos, RoutingCoordinate.CoordinateType.PipeToGround, rotation);
            }

            var productionStep = step as ProductionStep;
            if (productionStep != null)
            {
                List<FluidBox> boxes;
                List<ItemAmount> items;
                var productionBuilding = new ProductionBuilding(productionStep.Recipe, productionStep.Amount, productionStep.Building,
                    parameters.BuildingPositions[productionStep].Item1, parameters.BuildingPositions[productionStep].Item2);

                if (productionStep.Recipe.Ingredients.Where((i) => i.Item == item).Any())
                {
                    boxes = productionBuilding.FluidBoxes.Where((b) => !b.IsOutput).ToList();
                    items = productionBuilding.Recipe.Ingredients.ToList();
                }
                else
                {
                    boxes = productionBuilding.FluidBoxes.Where((b) => b.IsOutput).ToList();
                    items = productionBuilding.Recipe.Results.ToList();
                }
                for (int i = 0; i < boxes.Count; i++)
                {
                    var matchingIngredient = items[Math.Min(i, items.Count - 1)];
                    if (matchingIngredient.Item != item)
                        continue;

                    var box = boxes[i];

                    for (int d = 0; d < 4; d++)
                    {
                        var dir = (BuildingRotation)d;
                        var offsetDir = box.Position + dir.ToVector();
                        if (offsetDir.Clamp(productionBuilding.Size - Vector2.One) == offsetDir)
                            yield return new RoutingCoordinate(offsetDir + productionBuilding.Position, RoutingCoordinate.CoordinateType.PipeToGround, dir.Invert());
                    }
                }
            }
        }

        public static IEnumerable<RoutingCoordinate> BuildingToPlaceables(IStep step, SolutionParameters parameters)
        {
            if (step is ProductionStep)
            {
                var building = step as ProductionStep;
                var physical = new ProductionBuilding(building.Recipe, building.Amount, building.Building, parameters.BuildingPositions[building].Item1, parameters.BuildingPositions[building].Item2);
                for (int x = 0; x < physical.Size.X; x++)
                    for (int y = 0; y < physical.Size.Y; y++)
                        yield return new RoutingCoordinate(physical.Position + new Vector2(x, y), RoutingCoordinate.CoordinateType.Inserter, BuildingRotation.North);
                yield break;
            }

            if (step is SourceStep)
            {
                var pos = parameters.SourcePositions[step as SourceStep];
                var rotation = InwardDirectionForEdge(pos, new Vector2(parameters.Width, parameters.Height));
                yield return new RoutingCoordinate(pos, RoutingCoordinate.CoordinateType.Belt, rotation);
                yield break;
            }

            if (step is SinkStep)
            {
                var pos = parameters.SinkPositions[step as SinkStep];
                var rotation = InwardDirectionForEdge(pos, new Vector2(parameters.Width, parameters.Height)).Invert();
                yield return new RoutingCoordinate(pos, RoutingCoordinate.CoordinateType.Belt, rotation);
                yield break;
            }
        }

        private static BuildingRotation InwardDirectionForEdge(Vector2 position, Vector2 size)
        {
            if (position.X == 0)
                return BuildingRotation.East;
            if (position.Y == 0)
                return BuildingRotation.South;
            if (position.X == size.X - 1)
                return BuildingRotation.West;
            return BuildingRotation.North;
        }
    }
}
