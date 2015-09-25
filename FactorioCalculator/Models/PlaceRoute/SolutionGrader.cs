using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    class SolutionGrader
    {
        public Dictionary<string, double> CostLookup = new Dictionary<string, double>(){
            {"long-handed-inserter", 4},
            {"fast-inserter", 10},
            {"basic-belt-to-ground", 4},
            {"pipe-to-ground", 5},
        };

        public double TouchLeakCost = 50;
        public double CollisionCost = 200;
        public double LandUseCost = 1;

        public double CostForBuilding(SearchSpace state, IPhysicalBuilding building)
        {
            double cost = building.Size.X * building.Size.Y * LandUseCost;

            if (CostLookup.ContainsKey(building.Building.Name))
                cost += CostLookup[building.Building.Name];

            var collisions = state.CalculateCollisions(building.Position, building.Size).Where((b) => b != building);

            if (building is UndergroundFlow)
            {
                var cur = building as UndergroundFlow;
                var flows = collisions.Where((c) => c is UndergroundFlow).Cast<UndergroundFlow>().Where((f) => f.FlowDepth == cur.FlowDepth);
                var wells = collisions.Where((c) => c is GroundToUnderground).Cast<GroundToUnderground>().Where((w) => w.FlowDepth == cur.FlowDepth);

                cost += flows.Where((f) => f.Rotation == cur.Rotation || f.Rotation == cur.Rotation.Invert()).Count() * CollisionCost;
                cost += wells.Where((f) => f.Rotation == cur.Rotation || f.Rotation == cur.Rotation.Invert()).Count() * CollisionCost;
            }
            else if (building is GroundToUnderground)
            {
                var cur = building as GroundToUnderground;

                foreach (var collision in collisions)
                {
                    if (collision is UndergroundFlow)
                    {
                        var flow = collision as UndergroundFlow;
                        if (flow.FlowDepth != cur.FlowDepth)
                            continue;
                        if (flow.Rotation != cur.Rotation && flow.Rotation != cur.Rotation.Invert())
                            continue;
                        cost += CollisionCost;
                    }
                    else
                    {
                        cost += CollisionCost;
                    }
                }
            }
            else if (building.Building.Name == "pipe")
            {
                var above = collisions.Where((b) => !(b is UndergroundFlow));
                cost += above.Count() * CollisionCost;

                var pipeBuilding = (FlowBuilding)building;

                BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
                foreach (var rotation in rotations)
                {
                    var neighbors = state.CalculateCollisions(building.Position + rotation.ToVector());
                    var misMatch = neighbors.Where((b) => b.Building.Name == "pipe" && b is FlowBuilding).Cast<FlowBuilding>()
                        .Where((f) => f.Item.Item != pipeBuilding.Item.Item);


                    cost += misMatch.Count() * TouchLeakCost;
                }
            }
            else
            {
                var above = collisions.Where((b) => !(b is UndergroundFlow));
                cost += above.Count() * CollisionCost;
            }

            return cost;
        }
    }
}
