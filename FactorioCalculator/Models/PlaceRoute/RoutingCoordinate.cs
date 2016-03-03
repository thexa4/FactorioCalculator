using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public struct RoutingCoordinate
    {
        private Vector2 _position;
        private CoordinateType _type;
        private BuildingRotation _rotation;

        public Vector2 Position { get { return _position; } }
        public CoordinateType State { get { return _type; } }
        public BuildingRotation Rotation { get { return _rotation; } }

        public RoutingCoordinate(Vector2 position, CoordinateType type, BuildingRotation rotation)
        {
            _position = position;
            _type = type;
            _rotation = rotation;
        }


        public override int GetHashCode()
        {
            return new Tuple<Vector2, CoordinateType, BuildingRotation>(Position, State, Rotation).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RoutingCoordinate))
                return false;

            return Equals((RoutingCoordinate)obj);
        }

        public bool Equals(RoutingCoordinate other)
        {
            return other.Position == Position &&
                other.Rotation == Rotation &&
                other.State == State;
        }

        public static bool operator ==(RoutingCoordinate coordinate1, RoutingCoordinate coordinate2)
        {
            if (coordinate1 == null)
                return coordinate2 == null;

            return coordinate1.Equals(coordinate2);
        }

        public static bool operator !=(RoutingCoordinate coordinate1, RoutingCoordinate coordinate2)
        {
            if (coordinate1 == null)
                return coordinate2 != null;

            return !coordinate1.Equals(coordinate2);
        }

        public enum CoordinateType
        {
            Belt,
            PlacedItem,
            Inserter,
            Underflow,
            Pipe,
            PipeToGround,
            Splitter,
        }
    }
}
