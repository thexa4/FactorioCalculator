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
        public Building Splitter { get; protected set; }

        public SolutionGrader Grader { get; protected set; }

        public SolidRouter(Building belt, Building beltGroundNormal, Building beltGroundFast, Building beltGroundExpress,
            Building inserter, Building longInserter, Building fastInserter, Building splitter, SolutionGrader grader)
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
            if (splitter == null)
                throw new ArgumentNullException("splitter");
            if (grader == null)
                throw new ArgumentNullException("grader");

            Belt = belt;
            BeltGroundNormal = beltGroundNormal;
            BeltGroundFast = beltGroundFast;
            BeltGroundExpress = beltGroundExpress;
            Inserter = inserter;
            LongInserter = longInserter;
            FastInserter = fastInserter;
            Splitter = splitter;
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
            star.StateGenerator = (s) => s.NextStates(Grader.CostForBuilding, Belt, BeltGroundNormal, BeltGroundFast, BeltGroundExpress, Inserter, LongInserter, FastInserter, Splitter);
            star.EndStateValidator = ValidateEndState;

            foreach (var dest in destinations)
                star.AddDestination(dest);

            foreach (var position in startPositions)
            {
                switch (position.State)
                {
                    case RoutingCoordinate.CoordinateType.Belt:
                        var startBuilding = new Belt(item, Belt, position.Position, position.Rotation);
                        var tmpSpace1 = space.AddRoute(startBuilding);
                        var startState = new SolidRouteState(startBuilding, 0, position.Position, tmpSpace1, RoutingCoordinate.CoordinateType.Belt, Depth.None, position.Rotation);
                        star.AddState(startState);
                        break;
                    case RoutingCoordinate.CoordinateType.PlacedItem:
                    case RoutingCoordinate.CoordinateType.Inserter:
                        var startPlacedBuilding = new PlacedItem(item, position.Position);
                        var tmpSpace2 = space.AddRoute(startPlacedBuilding);
                        var startPlacedState = new SolidRouteState(startPlacedBuilding, 0, position.Position, tmpSpace2, RoutingCoordinate.CoordinateType.PlacedItem);
                        star.AddState(startPlacedState);
                        break;
                    case RoutingCoordinate.CoordinateType.Splitter:
                        var offsets = new BuildingRotation[]{
                            BuildingRotation.West,
                            BuildingRotation.North,
                            BuildingRotation.West,
                            BuildingRotation.North,
                        };

                        var offsetDir = offsets[(int)position.Rotation];
                        var startSplitter1 = new Splitter(item, Splitter, position.Position, position.Rotation);
                        var startSplitter2 = new Splitter(item, Splitter, position.Position + offsetDir.ToVector(), position.Rotation);
                        var space1 = space.AddRoute(startSplitter1);
                        var space2 = space.AddRoute(startSplitter2);
                        var state1 = new SolidRouteState(startSplitter1, 0, position.Position - offsetDir.ToVector(), space1, RoutingCoordinate.CoordinateType.Belt, Depth.None, position.Rotation);
                        var state2 = new SolidRouteState(startSplitter2, 0, position.Position + offsetDir.ToVector(), space2, RoutingCoordinate.CoordinateType.Belt, Depth.None, position.Rotation);
                        var before = space.CalculateCollisions(position.Position - position.Rotation.ToVector()).OfType<Belt>()
                            .Where((b) => b.Item.Item == item.Item)
                            .Where((b) => b.Rotation == position.Rotation);
                        var after = space.CalculateCollisions(position.Position + position.Rotation.ToVector()).OfType<Belt>()
                            .Where((b) => b.Item.Item == item.Item)
                            .Where((b) => b.Rotation == position.Rotation);

                        if (before.Any() && after.Any())
                        {
                            star.AddState(state1);
                            star.AddState(state2);
                        }

                        var startPlacedBuilding2 = new PlacedItem(item, position.Position);
                        var tmpSpace3 = space.AddRoute(startPlacedBuilding2);
                        var startPlacedState2 = new SolidRouteState(startPlacedBuilding2, 0, position.Position, tmpSpace3, RoutingCoordinate.CoordinateType.PlacedItem);
                        star.AddState(startPlacedState2);
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

            if (destination.State == RoutingCoordinate.CoordinateType.Splitter &&
                state.TransportState == RoutingCoordinate.CoordinateType.Inserter)
                return true;

            if (state.TransportState != destination.State)
                return false;

            return true;
        }
    }
}