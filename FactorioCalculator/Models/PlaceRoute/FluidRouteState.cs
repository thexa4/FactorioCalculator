using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    struct FluidRouteState : IRouteState
    {
        public double Cost { get { return _cost; } }
        private double _cost;

        public Vector2 Position { get { return _position; } }
        private Vector2 _position;

        public SearchSpace Space { get { return _space; } }
        private SearchSpace _space;

        public Depth Depth { get { return _depth; } }
        private Depth _depth;

        public BuildingRotation Direction { get { return _direction; } }
        private BuildingRotation _direction;

        public IPhysicalBuilding Building { get { return _building; } }
        private IPhysicalBuilding _building;

        public bool HasJustSurfaced { get { return _hasJustSurfaced; } }
        private bool _hasJustSurfaced;

        public int UndergroundLength { get { return _undergroundLength; } }
        private int _undergroundLength;

        public FlowBuilding FlowBuilding { get { return _building as FlowBuilding; } }

        public RoutingCoord RoutingCoord { get { return new RoutingCoord(_position, PlaceRoute.RoutingCoord.CoordType.Pipe, BuildingRotation.North); } }

        public FluidRouteState(IPhysicalBuilding building, double cost, Vector2 position, SearchSpace space, Depth depth = Depth.None, BuildingRotation direction = BuildingRotation.North, bool hasJustSurfaced = false, int undergroundLength = 0)
        {
            _building = building;
            _cost = cost;
            _position = position;
            _space = space;
            _depth = depth;
            _direction = direction;
            _hasJustSurfaced = hasJustSurfaced;
            _undergroundLength = undergroundLength;
        }

        public IEnumerable<FluidRouteState> NextStates(Func<SearchSpace, IPhysicalBuilding, double> costFunction, Building pipeToGround, Building pipe)
        {
            if (_depth == Depth.Fluid)
            {
                Vector2 nextPos = _position + _direction.ToVector();

                if (_undergroundLength < 9)
                {
                    // Continue
                    var flow = new UndergroundFlow(FlowBuilding.Item, nextPos, Depth.Fluid, _direction);
                    yield return new FluidRouteState(flow, _cost + costFunction(_space, flow), flow.Position, _space.AddRoute(flow), flow.FlowDepth, flow.Rotation, false, _undergroundLength + 1);
                }

                // Surface
                var surface = new FlowBuilding(FlowBuilding.Item, pipeToGround, nextPos, _direction.Invert());
                yield return new FluidRouteState(surface, _cost + costFunction(_space, surface), surface.Position, _space.AddRoute(surface), Depth.None, _direction, true);
            }
            else
            {
                if (HasJustSurfaced)
                {
                    Vector2 nextPos = _position + _direction.ToVector();
                    // Continue
                    var cont = new FlowBuilding(FlowBuilding.Item, pipe, nextPos, BuildingRotation.North);
                    yield return new FluidRouteState(cont, _cost + costFunction(_space, cont), cont.Position, _space.AddRoute(cont));

                    // Dive
                    var dive = new FlowBuilding(FlowBuilding.Item, pipeToGround, nextPos, _direction);
                    yield return new FluidRouteState(dive, _cost + costFunction(_space, dive), dive.Position, _space.AddRoute(dive), Depth.Fluid, _direction);

                }
                else
                {
                    BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
                    foreach (var rotation in rotations)
                    {
                        Vector2 nextPos = _position + rotation.ToVector();

                        // Straight
                        var cont = new FlowBuilding(FlowBuilding.Item, pipe, nextPos, BuildingRotation.North);
                        yield return new FluidRouteState(cont, _cost + costFunction(_space, cont), cont.Position, _space.AddRoute(cont));

                        // Dive
                        var dive = new FlowBuilding(FlowBuilding.Item, pipeToGround, nextPos, rotation);
                        yield return new FluidRouteState(dive, _cost + costFunction(_space, dive), dive.Position, _space.AddRoute(dive), Depth.Fluid, rotation);
                    }
                }
            }
        }
    }
}
