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
    public struct Searchspace
    {
        public ImmutableList<CollisionBox<ProductionBuilding>> Components { get { return _components; } }
        public ImmutableList<CollisionBox<FlowBuilding>> Routes { get { return _routes; } }

        private ImmutableList<CollisionBox<ProductionBuilding>> _components;
        private ImmutableList<CollisionBox<FlowBuilding>> _routes;

        public IEnumerable<IPhysicalBuilding> Buildings { get { return Components.Select((c) => c.Step as IPhysicalBuilding).Concat(Routes.Select((c) => c.Step as IPhysicalBuilding)); } }

        public Vector2 Size { get { return _size; } }
        private Vector2 _size;

        public Searchspace(Vector2 size) : this(size, ImmutableList<CollisionBox<ProductionBuilding>>.Empty, ImmutableList<CollisionBox<FlowBuilding>>.Empty) { }

        public Searchspace(Vector2 size, ImmutableList<CollisionBox<ProductionBuilding>> components, ImmutableList<CollisionBox<FlowBuilding>> routes)
        {
            _size = size;
            _components = components;
            _routes = routes;
        }

        public Searchspace AddComponent(ProductionBuilding building)
        {
            return new Searchspace(Size, _components.Add(new CollisionBox<ProductionBuilding>(building)), _routes);
        }

        public Searchspace AddRoute(FlowBuilding building)
        {
            return new Searchspace(Size, _components, _routes.Add(new CollisionBox<FlowBuilding>(building)));
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 position)
        {
            return CalculateCollisions(position, Vector2.One);
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 position, Vector2 size) {
            RectangleF source = new RectangleF((float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            foreach (var building in Buildings)
            {
                RectangleF test = new RectangleF((float)building.Position.X, (float)building.Position.Y,
                    (float)building.Size.X, (float)building.Size.Y);

                if (source.IntersectsWith(test))
                    yield return building;
            }
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
