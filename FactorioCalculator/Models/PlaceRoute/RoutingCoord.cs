using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public struct RoutingCoord
    {
        public Vector2 Position;
        public CoordType Type;
        public BuildingRotation Rotation;

        public RoutingCoord(Vector2 position, CoordType type, BuildingRotation rotation)
        {
            Position = position;
            Type = type;
            Rotation = rotation;
        }

        public enum CoordType
        {
            Belt,
            PlacedItem,
            Inserter,
            Underflow,
            Pipe,
            Splitter,
        }
    }
}
