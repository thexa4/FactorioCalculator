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

        public ImmutableDictionary<SourceStep, Vector2> SourcePositions { get; private set; }

        public ImmutableDictionary<SinkStep, Vector2> SinkPositions { get; private set; }

        public ImmutableDictionary<ProductionStep, Tuple<Vector2, BuildingRotation>> BuildingPositions { get; private set; }

        public ImmutableList<Tuple<IStep, Item, bool>> Connections { get; private set; }

        [ThreadStatic]
        private Random _random;

        private Random _globalRandom = new Random();

        public SolutionParameters(int width, int height)
        {
            Width = width;
            Height = height;

            SourcePositions = ImmutableDictionary<SourceStep, Vector2>.Empty;
            SinkPositions = ImmutableDictionary<SinkStep, Vector2>.Empty;
            BuildingPositions = ImmutableDictionary<ProductionStep, Tuple<Vector2, BuildingRotation>>.Empty;
            Connections = ImmutableList<Tuple<IStep, Item, bool>>.Empty;
        }

        public SolutionParameters(int width, int height,
            ImmutableDictionary<SourceStep, Vector2> sourcePositions,
            ImmutableDictionary<SinkStep, Vector2> sinkPositions, 
            ImmutableDictionary<ProductionStep, Tuple<Vector2, BuildingRotation>> buildingPositions,
            ImmutableList<Tuple<IStep, Item, bool>> connections)
        {
            Width = width;
            Height = height;
            SourcePositions = sourcePositions;
            SinkPositions = sinkPositions;
            BuildingPositions = buildingPositions;
            Connections = connections;
        }

        public static SolutionParameters FromFactory(int width, int height, RecipeGraph factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            var result = new SolutionParameters(width, height);

            result.SinkPositions = factory.OutputNodes.Concat(factory.WasteNodes).Select((s) => new KeyValuePair<SinkStep, Vector2>(s, Vector2.Zero)).ToImmutableDictionary();

            result.SourcePositions = factory.InputNodes.Select((s) => new KeyValuePair<SourceStep, Vector2>(s, Vector2.Zero)).ToImmutableDictionary();

            var buildings = factory.Transformations.Cast<ProductionStep>().ToImmutableList();
            result.BuildingPositions = buildings.Select((b) => new KeyValuePair<ProductionStep, Tuple<Vector2, BuildingRotation>>(b, new Tuple<Vector2, BuildingRotation>(Vector2.Zero, BuildingRotation.North))).ToImmutableDictionary();

            var buildingInputs = buildings.SelectMany((b) => b.Recipe.Ingredients.Select((i) => new Tuple<IStep, Item, bool>(b, i.Item, true)));
            var buildingOutputs = buildings.SelectMany((b) => b.Recipe.Results.Select((i) => new Tuple<IStep, Item, bool>(b, i.Item, false)));
            var sinkInputs = result.SinkPositions.Keys.Select((s) => new Tuple<IStep, Item, bool>(s, s.Item.Item, true));
            var sourceOutputs = result.SourcePositions.Keys.Select((s) => new Tuple<IStep, Item, bool>(s, s.Item.Item, false));

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
                lock (_globalRandom)
                    _random = new Random(_globalRandom.Next());

            var result = Copy();
            if (_random.NextDouble() < temperature)
                result.ModifySize(temperature);
            if (_random.NextDouble() < temperature)
                result.ModifyBuildings(temperature);
            if (_random.NextDouble() < temperature)
                result.ModifyEdges(temperature);

            result.ModifyConnections(temperature);
            return result;
        }

        private void ModifySize(double temperature)
        {
            if (_random.NextDouble() > 0.5)
            {
                Width += _random.Next(-1, 2);
                if (Width < 5)
                    Width = 5;
                Height += _random.Next(-1, 2);
                if (Height < 5)
                    Height = 5;
            }
            else
            {
                var xoff = _random.Next(-1, 2);
                var yoff = _random.Next(-1, 2);

                Width += xoff;
                Height += yoff;

                var offset = new Vector2(xoff, yoff);
                var bounds = new Vector2(Width, Height);

                SourcePositions = SourcePositions.SetItems(SourcePositions.Select((kvp) => new KeyValuePair<SourceStep, Vector2>(kvp.Key, Clamp(kvp.Value + offset, bounds))));
                SinkPositions = SinkPositions.SetItems(SinkPositions.Select((kvp) => new KeyValuePair<SinkStep, Vector2>(kvp.Key, Clamp(kvp.Value + offset, bounds))));
                BuildingPositions = BuildingPositions.SetItems(BuildingPositions.Select((kvp) => new KeyValuePair<ProductionStep, Tuple<Vector2, BuildingRotation>>(kvp.Key, new Tuple<Vector2, BuildingRotation>(kvp.Value.Item1 + offset, kvp.Value.Item2))));
            }
        }

        private void ModifyBuildings(double temperature)
        {
            var maxOffset = Math.Max(Width, Height);

            foreach (var building in BuildingPositions.Keys)
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

            foreach (var sink in SinkPositions.Keys)
            {
                var newPos = IndexToBound(BoundToIndex(SinkPositions[sink]) + (int)((2 * _random.NextDouble() - 1) * maxOffset * temperature));
                SinkPositions = SinkPositions.SetItem(sink, newPos);
            }

            foreach (var source in SourcePositions.Keys)
            {
                var newPos = IndexToBound(BoundToIndex(SourcePositions[source]) + (int)((2 * _random.NextDouble() - 1) * maxOffset * temperature));
                SourcePositions = SourcePositions.SetItem(source, newPos);
            }
        }

        public static Vector2 Clamp(Vector2 input, Vector2 bound)
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

        public static Vector2 Reflect(Vector2 input, Vector2 bound)
        {
            var clipped = Clamp(input, bound);
            var diff = input - clipped;
            return Clamp(clipped - diff, bound);
        }

        private static double Nfmod(double a, double b)
        {
            return a - b * Math.Floor(a / b);
        }

        public int BoundToIndex(Vector2 position)
        {
            if (position.Y == 0)
                return (int)position.X;
            if (position.X == Width - 1)
                return (int)position.Y + (int)Width - 1;
            if (position.Y == Height - 1)
                return (int)Width - (int)position.X - 3 + (int)Width + (int)Height;
            return (int)Height - (int)position.Y + 2 * (int)Width + (int)Height - 4;
        }

        public Vector2 IndexToBound(int index)
        {
            int length = 2 * (Width + Height) - 4;
            index = (int)Nfmod(index, length);

            if (index <= Width - 1)
                return new Vector2(index, 0);
            index -= Width - 1;

            if (index <= Height - 1)
                return new Vector2(Width - 1, index);
            index -= Height - 1;

            if (index <= Width - 1)
                return new Vector2(Width - index - 1, Height - 1);
            index -= Width - 1;

            return new Vector2(0, Height - index - 1);
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

        public override bool Equals(object obj)
        {
            var other = obj as SolutionParameters;
            if (other != null)
            {
                return other.Width == Width &&
                    other.Height == Height &&
                    SourcePositions.SequenceEqual(other.SourcePositions) &&
                    SinkPositions.SequenceEqual(other.SinkPositions) &&
                    BuildingPositions.SequenceEqual(other.BuildingPositions) &&
                    Connections.SequenceEqual(other.Connections);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int result = new Tuple<int, int>(Width, Height).GetHashCode();
            foreach (var item in SourcePositions)
                result ^= item.GetHashCode();
            foreach (var item in SinkPositions)
                result ^= item.GetHashCode();
            foreach (var item in BuildingPositions)
                result ^= item.GetHashCode();
            foreach (var item in Connections)
                result ^= item.GetHashCode();

            return result;
        }
    }
}
