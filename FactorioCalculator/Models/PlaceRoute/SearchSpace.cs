using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class Searchspace
    {
        public ImmutableList<CollisionBox<ProductionBuilding>> Components { get { return _components; } }
        public ImmutableList<CollisionBox<FlowBuilding>> Routes { get { return _routes; } }

        private ImmutableList<CollisionBox<ProductionBuilding>> _components;
        private ImmutableList<CollisionBox<FlowBuilding>> _routes;

        private ImmutableHashSet<IPhysicalBuilding>[,] _grid;

        public IEnumerable<IPhysicalBuilding> Buildings { get { return Components.Select((c) => c.Step as IPhysicalBuilding).Concat(Routes.Select((c) => c.Step as IPhysicalBuilding)); } }

        public Vector2 Size { get { return _size; } }
        private Vector2 _size;

        public Searchspace(Vector2 size) : this(size, ImmutableList<CollisionBox<ProductionBuilding>>.Empty, ImmutableList<CollisionBox<FlowBuilding>>.Empty) { }

        public Searchspace(Vector2 size, ImmutableList<CollisionBox<ProductionBuilding>> components, ImmutableList<CollisionBox<FlowBuilding>> routes, ImmutableHashSet<IPhysicalBuilding>[,] grid = null)
        {
            _size = size;
            _components = components;
            _routes = routes;

            _grid = new ImmutableHashSet<IPhysicalBuilding>[(int)_size.X, (int)_size.Y];
            if (grid == null)
            {
                
                for (int x = 0; x < _size.X; x++)
                    for (int y = 0; y < _size.Y; y++)
                        _grid[x, y] = ImmutableHashSet<IPhysicalBuilding>.Empty;
            }
            else
            {
                _grid = (ImmutableHashSet<IPhysicalBuilding>[,])grid.Clone();
            }
        }

        public Searchspace() { }

        public Searchspace AddComponent(ProductionBuilding building)
        {
            var result = new Searchspace(Size, _components.Add(new CollisionBox<ProductionBuilding>(building)), _routes, _grid);

            for (int x = 0; x < building.Size.X; x++)
                for (int y = 0; y < building.Size.Y; y++)
                {
                    var xpos = x + (int)building.Position.X;
                    var ypos = y + (int)building.Position.Y;
                    if (xpos < Size.X && ypos < Size.Y)
                        result._grid[xpos, ypos] = _grid[xpos, ypos].Add(building);
                }

            return result;
        }

        public Searchspace AddRoute(FlowBuilding building)
        {
            var result = new Searchspace(Size, _components, _routes.Add(new CollisionBox<FlowBuilding>(building)), _grid);
            
            for (int x = 0; x < building.Size.X; x++)
                for (int y = 0; y < building.Size.Y; y++)
                {
                    var xpos = x + (int)building.Position.X;
                    var ypos = y + (int)building.Position.Y;
                    if (xpos >= 0 && ypos >= 0 && xpos < Size.X && ypos < Size.Y)
                        result._grid[xpos, ypos] = _grid[xpos, ypos].Add(building);
                }

            return result;
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 position)
        {
            return CalculateCollisions(position, Vector2.One);
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 position, Vector2 size) {
            HashSet<IPhysicalBuilding> result = new HashSet<IPhysicalBuilding>();

            for (int x = 0; x < size.X; x++)
                for (int y = 0; y < size.Y; y++)
                {
                    var xpos = x + (int)position.X;
                    var ypos = y + (int)position.Y;
                    if (xpos >= 0 && ypos >= 0 && xpos < Size.X && ypos < Size.Y)
                        foreach (var building in _grid[xpos, ypos])
                            result.Add(building);
                }

            return result;
        }

        public bool IsValidSinkSourcePosition(Vector2 position)
        {
            Vector2 offset = position - Vector2.One - _size;
            return offset.X == 0 || offset.Y == 0 || position.X == 0 | position.Y == 0;
        }


        public override int GetHashCode()
        {
            int result = _size.GetHashCode();
            foreach (var building in Buildings)
                result ^= building.GetHashCode();

            return result;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Searchspace))
                return false;

            return Equals((Searchspace)obj);
        }

        public bool Equals(Searchspace other)
        {
            if (other._size != _size)
                return false;

            var otherBuildings = other.Buildings.OrderBy((b) => b.GetHashCode());

            return otherBuildings.SequenceEqual(Buildings.OrderBy((b) => b.GetHashCode()));
        }

        public static bool operator ==(Searchspace space1, Searchspace space2)
        {
            if (space1 == null)
                return space2 == null;

            return space1.Equals(space2);
        }

        public static bool operator !=(Searchspace space1, Searchspace space2)
        {
            if (space1 == null)
                return space2 != null;

            return !space1.Equals(space2);
        }
    }
}
