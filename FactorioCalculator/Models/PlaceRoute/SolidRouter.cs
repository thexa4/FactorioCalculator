using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class SolidRouter
    {
        public Building Belt;
        public Building BeltGroundNormal;
        public Building BeltGroundFast;
        public Building BeltGroundExpress;
        public Building Inserter;
        public Building LongInserter;
        public Building FastInserter;

        public SolutionGrader Grader;

        public SearchSpace Route(ItemAmount item, SearchSpace space, IEnumerable<RoutingCoord> startPositions, IEnumerable<RoutingCoord> destinations)
        {
            AStar<SolidRouteState> star = new AStar<SolidRouteState>();
            star.StateGenerator = (s) => s.NextStates(Grader.CostForBuilding, Belt, BeltGroundNormal, BeltGroundFast, BeltGroundExpress, Inserter, LongInserter, FastInserter);
            star.EndStateValidator = ValidateEndState;

            foreach (var dest in destinations)
                star.AddDestination(dest);

            foreach (var position in startPositions)
            {
                switch (position.Type)
                {
                    case RoutingCoord.CoordType.Belt:
                        var startBuilding = new FlowBuilding(item, Belt, position.Position, position.Rotation);
                        space = space.AddRoute(startBuilding);
                        var startState = new SolidRouteState(startBuilding, 0, position.Position, space, RoutingCoord.CoordType.Belt, Depth.None, position.Rotation);
                        star.AddState(startState);
                        break;
                    case RoutingCoord.CoordType.PlacedItem:
                        var startPlacedBuilding = new FlowBuilding(item, new Building("placed-item"), position.Position, BuildingRotation.North);
                        space = space.AddRoute(startPlacedBuilding);
                        var startPlacedState = new SolidRouteState(startPlacedBuilding, 0, position.Position, space, RoutingCoord.CoordType.PlacedItem);
                        star.AddState(startPlacedState);
                        break;
                }
            }

            while (!star.Step()) { }

            return star.EndState.Space;
        }

        private bool ValidateEndState(SolidRouteState state, HashSet<RoutingCoord> destinations)
        {
            if (!destinations.Where((d) => d.Position == state.Position).Any())
                return false;

            var destination = destinations.Where((d) => d.Position == state.Position).First();

            if (state.TransportState != destination.Type)
                return false;

            return true;
        }
    }
}