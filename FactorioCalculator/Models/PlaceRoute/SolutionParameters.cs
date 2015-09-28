using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class SolutionParameters
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public ImmutableList<SourceStep> Sources { get; private set; }
        public ImmutableDictionary<SourceStep, Vector2> SourcePositions { get; private set; }

        public ImmutableList<SinkStep> Sinks { get; private set; }
        public ImmutableDictionary<SinkStep, Vector2> SinkPositions { get; private set; }

        public ImmutableList<ProductionStep> Buildings { get; private set; }
        public ImmutableDictionary<ProductionStep, Tuple<Vector2, BuildingRotation>> BuildingPositions { get; private set; }

        public ImmutableList<Tuple<IStep, Item, bool>> Connections { get; private set; }

        [ThreadStatic]
        private Random _random;

        private Random _globalRandom = new Random();

        public SolutionParameters(int width, int height)
        {
            Width = width;
            Height = height;

            Sources = ImmutableList<SourceStep>.Empty;
            SourcePositions = ImmutableDictionary<SourceStep, Vector2>.Empty;
            Sinks = ImmutableList<SinkStep>.Empty;
            SinkPositions = ImmutableDictionary<SinkStep, Vector2>.Empty;
            Buildings = ImmutableList<ProductionStep>.Empty;
            BuildingPositions = ImmutableDictionary<ProductionStep, Tuple<Vector2, BuildingRotation>>.Empty;
            Connections = ImmutableList<Tuple<IStep, Item, bool>>.Empty;
        }

        public static SolutionParameters FromFactory(int width, int height, RecipeGraph factory)
        {
            var result = new SolutionParameters(width, height);

            result.Sinks = factory.OutputNodes.Concat(factory.WasteNodes).ToImmutableList();
            result.SinkPositions = result.Sinks.Select((s) => new KeyValuePair<SinkStep, Vector2>(s, Vector2.Zero)).ToImmutableDictionary();

            result.Sources = factory.InputNodes.ToImmutableList();
            result.SourcePositions = result.Sources.Select((s) => new KeyValuePair<SourceStep, Vector2>(s, Vector2.Zero)).ToImmutableDictionary();

            result.Buildings = factory.Transformations.Cast<ProductionStep>().ToImmutableList();
            result.BuildingPositions = result.Buildings.Select((b) => new KeyValuePair<ProductionStep, Tuple<Vector2, BuildingRotation>>(b, new Tuple<Vector2, BuildingRotation>(Vector2.Zero, BuildingRotation.North))).ToImmutableDictionary();

            var buildingInputs = result.Buildings.SelectMany((b) => b.Recipe.Ingredients.Select((i) => new Tuple<IStep, Item, bool>(b, i.Item, true)));
            var buildingOutputs = result.Buildings.SelectMany((b) => b.Recipe.Results.Select((i) => new Tuple<IStep, Item, bool>(b, i.Item, false)));
            var sinkInputs = result.Sinks.Select((s) => new Tuple<IStep, Item, bool>(s, s.Item.Item, true));
            var sourceOutputs = result.Sources.Select((s) => new Tuple<IStep, Item, bool>(s, s.Item.Item, false));

            result.Connections = buildingInputs.Concat(buildingOutputs).Concat(sinkInputs).Concat(sourceOutputs).ToImmutableList();

            for(int i = 0; i < 4; i++)
                result = result.Modify(1);

            return result;
        }

        public SolutionParameters Copy()
        {
            return (SolutionParameters)this.MemberwiseClone();
        }

        public SolutionParameters Modify(double temperature)
        {
            if (_random == null)
                lock(_globalRandom)
                    _random = new Random(_globalRandom.Next());

            var result = Copy();
            result.ModifyConnections(temperature);
            result.ModifyBuildings(temperature);
            result.ModifyEdges(temperature);
            return result;
        }

        private void ModifyBuildings(double temperature)
        {
            var maxOffset = Math.Max(Width, Height);

            foreach (var building in Buildings)
            {
                var xOffset = Math.Floor((2 * _random.NextDouble() - 1) * maxOffset * temperature);
                var yOffset = Math.Floor((2 * _random.NextDouble() - 1) * maxOffset * temperature);

                var newPos = Reflect(BuildingPositions[building].Item1 + new Vector2(xOffset, yOffset), new Vector2(Width, Height) - building.Building.Size);
                var newRot = ((int)BuildingPositions[building].Item2 + (2 * _random.NextDouble() - 1) * temperature * 32 + 64) % 4;
                BuildingPositions = BuildingPositions.SetItem(building, new Tuple<Vector2, BuildingRotation>(newPos, (BuildingRotation)((int)newRot)));
            }
        }

        private void ModifyEdges(double temperature)
        {
            var maxOffset = 2 * (Width + Height) - 4;

            foreach (var sink in Sinks)
            {
                var newPos = IntToBound(BoundToInt(SinkPositions[sink]) + (int)((2 * _random.NextDouble() - 1) * maxOffset * temperature * 16));
                SinkPositions = SinkPositions.SetItem(sink, newPos);
            }

            foreach (var source in Sources)
            {
                var newPos = IntToBound(BoundToInt(SourcePositions[source]) + (int)((2 * _random.NextDouble() - 1) * maxOffset * temperature * 16));
                SourcePositions = SourcePositions.SetItem(source, newPos);
            }
        }

        private Vector2 Clamp(Vector2 input, Vector2 bound)
        {
            var x = input.X;
            var y = input.Y;

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            if (x > bound.X)
                x = bound.X;
            if (y > bound.Y)
                y = bound.Y;

            return new Vector2(x, y);
        }

        private Vector2 Reflect(Vector2 input, Vector2 bound)
        {
            var clipped = Clamp(input, bound);
            var diff = input - clipped;
            return Clamp(clipped - diff, bound);
        }

        private int BoundToInt(Vector2 pos)
        {
            if (pos.Y == 0)
                return (int)pos.X;
            if (pos.X == Width - 1)
                return (int)pos.Y + (int)Width - 1;
            if (pos.Y == Height - 1)
                return (int)Width - (int)pos.X - 3 + (int)Width + (int)Height;
            return (int)Height - (int)pos.Y + 2 * (int)Width + (int)Height - 4;
        }

        private Vector2 IntToBound(int pos)
        {
            int length = 2 * (Width + Height) - 4;
            if (pos < 0)
                pos += length;
            pos = pos % length;

            if (pos <= Width)
                return new Vector2(pos, 0);
            pos -= Width - 1;

            if (pos <= Height)
                return new Vector2(Width - 1, pos);
            pos -= Height - 1;

            if (pos <= Width)
                return new Vector2(Width - pos - 1, Height - 1);
            pos -= Width - 1;

            return new Vector2(0, Height - pos - 1);
        }

        private void ModifyConnections(double temperature)
        {
            if (Connections.Count <= 1)
                return;

            for (int i = 0; i < temperature * 10000; i++)
            {
                var from = _random.Next(0, Connections.Count);
                var to = _random.Next(0, Connections.Count);

                var temp = Connections[to];

                Connections = Connections.SetItem(to, Connections[from]);
                Connections = Connections.SetItem(from, temp);
            }
        }
    }
}
