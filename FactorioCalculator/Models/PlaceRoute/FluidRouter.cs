using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class FluidRouter
    {
        public Building PipeToGround;
        public Building Pipe;

        public SolutionGrader Grader;
        
        public SearchSpace Route(ItemAmount item, SearchSpace space, Vector2 position, BuildingRotation rotation, List<RoutingCoord> destinations)
        {
            AStar<FluidRouteState> star = new AStar<FluidRouteState>();
            star.StateGenerator = (s) => s.NextStates(Grader.CostForBuilding, PipeToGround, Pipe);
            star.EndStateValidator = ValidateEndState;

            foreach (var dest in destinations)
                star.AddDestination(dest);

            var startBuilding = new VirtualFlowStep(item, PipeToGround, position, rotation.Invert());
            var startState = new FluidRouteState(startBuilding, 0, position, space, Depth.None, rotation, true);
            star.AddState(startState);

            while (!star.Step()) { }

            return star.EndState.Space;
        }

        private bool ValidateEndState(FluidRouteState state, HashSet<RoutingCoord> destinations)
        {
            if (state.Depth != Depth.None)
                return false;

            if (state.Building.Building.Name != "pipe")
                return false;

            bool found = false;
            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
            foreach (var rotation in rotations)
                if (destinations.Where((d) => d.Position == state.Position + rotation.ToVector()).Any())
                    found = true;

            if (!found)
                return false;

            return true;
        }
    }
}
