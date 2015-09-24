using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    class FluidRouter
    {
        public double TouchLeakCost = 1;
        public double CollisionCost = 2;
        public Building PipeToGround;
        public Building Pipe;

        public double CostForPlacement(FluidRouteState state, IPhysicalBuilding building)
        {
            var collisions = state.Space.CalculateCollisions(building.Position, building.Size);
            
            if (building is UndergroundFlow)
            {
                var underflow = collisions.Where((b) => b is UndergroundFlow || b.Building == PipeToGround)
                    .Where((b) => b.Rotation == building.Rotation || b.Rotation == building.Rotation.Invert());
                if (underflow.Any())
                    return CollisionCost;
                return 0;
            }

            if (building.Building == PipeToGround)
            {
                var underflow = collisions.Where((b) => b is UndergroundFlow || b.Building == PipeToGround)
                    .Where((b) => b.Rotation == building.Rotation || b.Rotation == building.Rotation.Invert());
                if (underflow.Any())
                    return CollisionCost;

                var rest = collisions.Where((b) => !(b is UndergroundFlow));
                if (rest.Any())
                    return CollisionCost;
            }

            if (building.Building == Pipe)
            {
                var spot = collisions.Where((b) => !(b is UndergroundFlow));
                if (spot.Any())
                    return CollisionCost;

                var pipeBuilding = (FlowBuilding)building;

                BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
                foreach (var rotation in rotations)
                {
                    var neighbors = state.Space.CalculateCollisions(building.Position + rotation.ToVector());
                    var misMatch = neighbors.Where((b) => b.Building == Pipe && b is FlowBuilding).Cast<FlowBuilding>()
                        .Where((f) => f.Item.Item != pipeBuilding.Item.Item);

                    if (misMatch.Any())
                        return TouchLeakCost;
                }
            }

            return 0;
        }

        public SearchSpace Route(ItemAmount item, SearchSpace space, Vector2 position, BuildingRotation rotation, List<Vector2> destinations)
        {
            AStar<FluidRouteState> star = new AStar<FluidRouteState>();
            star.StateGenerator = (s) => s.NextStates(CostForPlacement, PipeToGround, Pipe);

            foreach (var dest in destinations)
                star.AddDestination(dest);

            var startBuilding = new VirtualFlowStep(item, PipeToGround, position, rotation.Invert());
            var startState = new FluidRouteState(startBuilding, 0, position, space, Depth.None, rotation, true);
            star.AddState(startState);

            while (!star.Step()) { }

            return star.EndState.Space;
        }
    }
}
