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
        public Building Belt { get; protected set; }
        public Building BeltGroundNormal { get; protected set; }
        public Building BeltGroundFast { get; protected set; }
        public Building BeltGroundExpress { get; protected set; }
        public Building Inserter { get; protected set; }
        public Building LongInserter { get; protected set; }
        public Building FastInserter { get; protected set; }

        public SolutionGrader Grader { get; protected set; }

        public SolidRouter(Building belt, Building beltGroundNormal, Building beltGroundFast, Building beltGroundExpress,
            Building inserter, Building longInserter, Building fastInserter, SolutionGrader grader)
        {
            if (belt == null)
                throw new ArgumentNullException("belt");
            if (beltGroundNormal == null)
                throw new ArgumentNullException("beltGroundNormal");
            if (beltGroundFast == null)
                throw new ArgumentNullException("beltGroundFast");
            if (beltGroundExpress == null)
                throw new ArgumentNullException("beltGroundExpress");
            if (inserter == null)
                throw new ArgumentNullException("inserter");
            if (longInserter == null)
                throw new ArgumentNullException("longInserter");
            if (fastInserter == null)
                throw new ArgumentNullException("fastInserter");
            if (grader == null)
                throw new ArgumentNullException("grader");

            Belt = belt;
            BeltGroundNormal = beltGroundNormal;
            BeltGroundFast = beltGroundFast;
            BeltGroundExpress = beltGroundExpress;
            Inserter = inserter;
            LongInserter = longInserter;
            FastInserter = fastInserter;
            Grader = grader;
        }

        /// <summary>
        /// Attempts to route from the start states to the destination states. The given states are both lists, but only one state from each list will be selected for the final routing.
        /// </summary>
        /// <param name="item">How much to route.</param>
        /// <param name="space">The initialstate from which the routing starts.</param>
        /// <param name="startPositions">The list of starting states. Only one will be used.</param>
        /// <param name="destinations">The list of desired destination states. Only one will be reached.</param>
        /// <returns>The solution state found after routing</returns>
        public Searchspace Route(ItemAmount item, Searchspace space, IEnumerable<RoutingCoordinate> startPositions, IEnumerable<RoutingCoordinate> destinations)
        {
            if (startPositions == null)
                throw new ArgumentNullException("startPositions");
            if (destinations == null)
                throw new ArgumentNullException("destinations");

            AStar<SolidRouteState> star = new AStar<SolidRouteState>();
            star.StateGenerator = (s) => s.NextStates(Grader.CostForBuilding, Belt, BeltGroundNormal, BeltGroundFast, BeltGroundExpress, Inserter, LongInserter, FastInserter);
            star.EndStateValidator = ValidateEndState;

            foreach (var dest in destinations)
                star.AddDestination(dest);

            foreach (var position in startPositions)
            {
                switch (position.State)
                {
                    case RoutingCoordinate.CoordinateType.Belt:
                        var startBuilding = new FlowBuilding(item, Belt, position.Position, position.Rotation);
                        space = space.AddRoute(startBuilding);
                        var startState = new SolidRouteState(startBuilding, 0, position.Position, space, RoutingCoordinate.CoordinateType.Belt, Depth.None, position.Rotation);
                        star.AddState(startState);
                        break;
                    case RoutingCoordinate.CoordinateType.PlacedItem:
                    case RoutingCoordinate.CoordinateType.Inserter:
                        var startPlacedBuilding = new FlowBuilding(item, new Building("placed-item"), position.Position, BuildingRotation.North);
                        space = space.AddRoute(startPlacedBuilding);
                        var startPlacedState = new SolidRouteState(startPlacedBuilding, 0, position.Position, space, RoutingCoordinate.CoordinateType.PlacedItem);
                        star.AddState(startPlacedState);
                        break;
                }
            }

            while (!star.Step()) { }

            return star.EndState.Space;
        }

        private bool ValidateEndState(SolidRouteState state, HashSet<RoutingCoordinate> destinations)
        {
            if (!destinations.Where((d) => d.Position == state.Position).Any())
                return false;

            var destination = destinations.Where((d) => d.Position == state.Position).First();

            if (state.TransportState != destination.State)
                return false;

            return true;
        }
    }
}