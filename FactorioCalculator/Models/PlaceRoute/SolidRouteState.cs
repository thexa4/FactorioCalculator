using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    struct SolidRouteState : IRouteState
    {
        public double Cost { get { return _cost; } }
        private double _cost;

        public Vector2 Position { get { return _position; } }
        private Vector2 _position;

        public Searchspace Space { get { return _space; } }
        private Searchspace _space;

        public Depth Depth { get { return _depth; } }
        private Depth _depth;

        public BuildingRotation Direction { get { return _direction; } }
        private BuildingRotation _direction;

        public IPhysicalBuilding Building { get { return _building; } }
        private IPhysicalBuilding _building;
 
        public int UndergroundLength { get { return _undergroundLength; } }
        private int _undergroundLength;

        public FlowBuilding FlowBuilding { get { return _building as FlowBuilding; } }

        public RoutingCoordinate.CoordinateType TransportState { get { return _transportState; } }
        private RoutingCoordinate.CoordinateType _transportState;

        public RoutingCoordinate RoutingCoord { get { return new RoutingCoordinate(_position, _transportState, _direction); } }

        public SolidRouteState(IPhysicalBuilding building, double cost, Vector2 position, Searchspace space, RoutingCoordinate.CoordinateType transportState, Depth depth = Depth.None, BuildingRotation direction = BuildingRotation.North, int undergroundLength = 0)
        {
            _building = building;
            _cost = cost;
            _position = position;
            _space = space;
            _depth = depth;
            _direction = direction;
            _undergroundLength = undergroundLength;
            _transportState = transportState;
        }

        public IEnumerable<SolidRouteState> NextStates(Func<Searchspace, IPhysicalBuilding, double> costFunction,
            Building belt, Building beltGroundNormal, Building beltGroundFast,
            Building beltGroundExpress, Building inserter, Building longInserter,
            Building fastInserter, Building splitter)
        {
            switch (_transportState)
            {
                case RoutingCoordinate.CoordinateType.Belt:
                    return GenerateBeltStates(costFunction, belt)
                        .Concat(GeneratePlacedStates(costFunction))
                        .Concat(GenerateBeltToGround(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress))
                        .Concat(GenerateSplitterStates(costFunction, splitter));
                case RoutingCoordinate.CoordinateType.Inserter:
                    return GeneratePlacedStates(costFunction)
                        .Concat(GenerateBeltStates(costFunction, belt))
                        .Concat(GenerateBeltToGround(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress));
                case RoutingCoordinate.CoordinateType.PlacedItem:
                    return GenerateInserters(costFunction, inserter, longInserter, fastInserter);
                case RoutingCoordinate.CoordinateType.Underflow:
                    return GenerateUnderFlow(costFunction)
                        .Concat(GenerateGroundToBelt(costFunction, beltGroundNormal, beltGroundFast, beltGroundExpress));
            }

            return new SolidRouteState[] { };
        }

        private IEnumerable<SolidRouteState> GeneratePlacedStates(Func<Searchspace, IPhysicalBuilding, double> costFunction)
        {
            var placedBuilding = new PlacedItem(((FlowBuilding)_building).Item, _position);
            placedBuilding.Previous.Add(Building);
            yield return new SolidRouteState(placedBuilding, _cost + costFunction(_space, placedBuilding), _position, _space.AddRoute(placedBuilding), RoutingCoordinate.CoordinateType.PlacedItem);
        }

        private IEnumerable<SolidRouteState> GenerateSplitterStates(Func<Searchspace, IPhysicalBuilding, double> costFunction, Building splitter)
        {
            var offsets = new BuildingRotation[]{
                            BuildingRotation.West,
                            BuildingRotation.North,
                            BuildingRotation.West,
                            BuildingRotation.North,
                        };
            var offsetDir = offsets[(int)Direction];
            Vector2 nextpos = _position + (_transportState == RoutingCoordinate.CoordinateType.Belt ? _direction.ToVector() : Vector2.Zero);

            var endPoint1 = nextpos + offsetDir.ToVector();
            var endPoint2 = nextpos - offsetDir.ToVector();

            var target = FlowBuilding.Item.Item;
            var dir = Direction;

            var matches1 = Space.CalculateCollisions(endPoint1).OfType<Belt>().Where((b) => b.Item.Item == target && b.Rotation == dir);
            var matches2 = Space.CalculateCollisions(endPoint2).OfType<Belt>().Where((b) => b.Item.Item == target && b.Rotation == dir);

            if (matches1.Any())
            {
                var building = new Splitter(FlowBuilding.Item, splitter, endPoint1, Direction);
                building.Previous.Add(Building);
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), endPoint1, _space.AddRoute(building), RoutingCoordinate.CoordinateType.Splitter);
            }

            if (matches2.Any())
            {
                var building = new Splitter(FlowBuilding.Item, splitter, nextpos, Direction);
                building.Previous.Add(Building);
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), endPoint2, _space.AddRoute(building), RoutingCoordinate.CoordinateType.Splitter);
            }
        }

        private IEnumerable<SolidRouteState> GenerateBeltStates(Func<Searchspace, IPhysicalBuilding, double> costFunction, Building belt)
        {
            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
            Vector2 nextpos = _position + (_transportState == RoutingCoordinate.CoordinateType.Belt ? _direction.ToVector() : Vector2.Zero);
            foreach (var rotation in rotations)
            {
                var building = new Belt(((FlowBuilding)_building).Item, belt, nextpos, rotation);
                building.Previous.Add(Building);
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), nextpos, _space.AddRoute(building), RoutingCoordinate.CoordinateType.Belt, Depth.None, rotation);
            }
        }

        private IEnumerable<SolidRouteState> GenerateBeltToGround(Func<Searchspace, IPhysicalBuilding, double> costFunction, Building groundNormal, Building groundFast, Building groundExpress)
        {
            Vector2 nextpos = _position + (_transportState == RoutingCoordinate.CoordinateType.Belt ? _direction.ToVector() : Vector2.Zero);
            var buildingNormal = new GroundToUnderground(((FlowBuilding)_building).Item, groundNormal, nextpos, _direction, Depth.Normal, false);
            var buildingFast = new GroundToUnderground(((FlowBuilding)_building).Item, groundFast, nextpos, _direction, Depth.Fast, false);
            var buildingExpress = new GroundToUnderground(((FlowBuilding)_building).Item, groundExpress, nextpos, _direction, Depth.Express, false);
            buildingNormal.Previous.Add(Building);
            buildingFast.Previous.Add(Building);
            buildingExpress.Previous.Add(Building);
            yield return new SolidRouteState(buildingNormal, _cost + costFunction(_space, buildingNormal), nextpos, _space.AddRoute(buildingNormal), RoutingCoordinate.CoordinateType.Underflow, Depth.Normal, _direction);
            yield return new SolidRouteState(buildingFast, _cost + costFunction(_space, buildingFast), nextpos, _space.AddRoute(buildingFast), RoutingCoordinate.CoordinateType.Underflow, Depth.Fast, _direction);
            yield return new SolidRouteState(buildingExpress, _cost + costFunction(_space, buildingExpress), nextpos, _space.AddRoute(buildingExpress), RoutingCoordinate.CoordinateType.Underflow, Depth.Express, _direction);
        }

        private IEnumerable<SolidRouteState> GenerateUnderFlow(Func<Searchspace, IPhysicalBuilding, double> costFunction)
        {
            Vector2 nextpos = _position + _direction.ToVector();
            var building = new UndergroundFlow(((FlowBuilding)_building).Item, nextpos, _depth, _direction);
            building.Previous.Add(Building);
            if (_undergroundLength < 4)
                yield return new SolidRouteState(building, _cost + costFunction(_space, building), nextpos, _space.AddRoute(building), RoutingCoordinate.CoordinateType.Underflow, _depth, _direction, _undergroundLength + 1);
        }

        private IEnumerable<SolidRouteState> GenerateGroundToBelt(Func<Searchspace, IPhysicalBuilding, double> costFunction, Building groundNormal, Building groundFast, Building groundExpress)
        {
            Vector2 nextpos = _position + _direction.ToVector();
            var buildingNormal = new GroundToUnderground(((FlowBuilding)_building).Item, groundNormal, nextpos, _direction, Depth.Normal, true);
            var buildingFast = new GroundToUnderground(((FlowBuilding)_building).Item, groundFast, nextpos, _direction, Depth.Fast, true);
            var buildingExpress = new GroundToUnderground(((FlowBuilding)_building).Item, groundExpress, nextpos, _direction, Depth.Express, true);
            buildingNormal.Previous.Add(Building);
            buildingFast.Previous.Add(Building);
            buildingExpress.Previous.Add(Building);
            
            if (_depth == Depth.Normal)
                yield return new SolidRouteState(buildingNormal, _cost + costFunction(_space, buildingNormal), nextpos, _space.AddRoute(buildingNormal), RoutingCoordinate.CoordinateType.Belt, Depth.None, _direction);
            if (_depth == Depth.Fast)
                yield return new SolidRouteState(buildingFast, _cost + costFunction(_space, buildingFast), nextpos, _space.AddRoute(buildingFast), RoutingCoordinate.CoordinateType.Belt, Depth.None, _direction);
            if (_depth == Depth.Express)
                yield return new SolidRouteState(buildingExpress, _cost + costFunction(_space, buildingExpress), nextpos, _space.AddRoute(buildingExpress), RoutingCoordinate.CoordinateType.Belt, Depth.None, _direction);
        }

        private IEnumerable<SolidRouteState> GenerateInserters(Func<Searchspace, IPhysicalBuilding, double> costFunction, Building inserter, Building longInserter, Building fastInserter)
        {
            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };

            foreach (var rotation in rotations)
            {
                Vector2 nextpos = _position;
                var buildingInserter = new PhysicalFlowBuilding(((FlowBuilding)_building).Item, inserter, nextpos + rotation.ToVector(), rotation);
                var buildingLongInserter = new PhysicalFlowBuilding(((FlowBuilding)_building).Item, longInserter, nextpos + 2 * rotation.ToVector(), rotation);
                var buildingFastInserter = new PhysicalFlowBuilding(((FlowBuilding)_building).Item, fastInserter, nextpos + rotation.ToVector(), rotation);

                buildingInserter.Previous.Add(Building);
                buildingLongInserter.Previous.Add(Building);
                buildingFastInserter.Previous.Add(Building);

                yield return new SolidRouteState(buildingInserter, _cost + costFunction(_space, buildingInserter), nextpos + 2 * rotation.ToVector(), _space.AddRoute(buildingInserter), RoutingCoordinate.CoordinateType.Inserter, Depth.None, BuildingRotation.North);
                yield return new SolidRouteState(buildingLongInserter, _cost + costFunction(_space, buildingLongInserter), nextpos + 4 * rotation.ToVector(), _space.AddRoute(buildingLongInserter), RoutingCoordinate.CoordinateType.Inserter, Depth.None, BuildingRotation.North);
                yield return new SolidRouteState(buildingFastInserter, _cost + costFunction(_space, buildingFastInserter), nextpos + 2 * rotation.ToVector(), _space.AddRoute(buildingFastInserter), RoutingCoordinate.CoordinateType.Inserter, Depth.None, BuildingRotation.North);
            }
        }
    }
}
