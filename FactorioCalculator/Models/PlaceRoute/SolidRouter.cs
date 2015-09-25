using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    class SolidRouter
    {
        public Building Belt;
        public Building BeltGroundNormal;
        public Building BeltGroundFast;
        public Building BeltGroundExpress;
        public Building Inserter;
        public Building LongInserter;
        public Building FastInserter;

        public SolutionGrader Grader;

        public SearchSpace Route(ItemAmount item, SearchSpace space, List<Vector2> startPositions, List<Vector2> destinations)
        {
            AStar<SolidRouteState> star = new AStar<SolidRouteState>();
            star.StateGenerator = (s) => s.NextStates(Grader.CostForBuilding, Belt, BeltGroundNormal, BeltGroundFast, BeltGroundExpress, Inserter, LongInserter, FastInserter);
            star.EndStateValidator = ValidateEndState;

            foreach (var dest in destinations)
                star.AddDestination(dest);

            foreach (var position in startPositions)
            {
                var startBuilding = new FlowBuilding(item, new Building("placed-item"), position, BuildingRotation.North);
                space = space.AddRoute(startBuilding);
                var startState = new SolidRouteState(startBuilding, 0, position, space, TransportState.PlacedItem);
                star.AddState(startState);
            }

            while (!star.Step()) { }

            return star.EndState.Space;
        }

        private bool ValidateEndState(SolidRouteState state, HashSet<Vector2> destinations)
        {
            if (!destinations.Contains(state.Position))
                return false;

            if (state.TransportState == TransportState.PlacedItem)
                return false;

            return true;
        }
    }
}