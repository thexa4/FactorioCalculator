using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public class SolutionGrader
    {
        public Dictionary<string, double> CostLookup = new Dictionary<string, double>(){
            {"long-handed-inserter", 18},
            {"fast-inserter", 20},
            {"basic-inserter", 8},
            {"basic-belt-to-ground", 20},
            {"pipe-to-ground", 20},
            //{"underground-flow", -1},
        };

        public double ProductionCollisionCost = 1000;
        public double TouchLeakCost = 50;
        public double CollisionCost = 200;
        public double LandUseCost = 1;
        public double EdgeUseCost = 200;
        public double AreaCost = 1;

        public double CostForSolution(SearchSpace state)
        {
            double cost = 0;
            cost += state.Size.X * state.Size.Y * AreaCost;

            foreach(var production in state.Buildings)
                foreach(var collision in state.CalculateCollisions(production.Position, production.Size))
                {
                    if (collision == production)
                        continue;
                    if (collision is ProductionBuilding)
                        cost += ProductionCollisionCost;
                }

            foreach (var route in state.Routes)
                cost += CostForBuilding(state, route.Step);
            return cost;
        }

        public double CostForBuilding(SearchSpace state, IPhysicalBuilding building)
        {
            double cost = building.Size.X * building.Size.Y * LandUseCost;

            if (building.Position.X <= 0 && building.Rotation != BuildingRotation.West && building.Rotation != BuildingRotation.East)
                cost += EdgeUseCost;
            if (building.Position.Y <= 0 && building.Rotation != BuildingRotation.North && building.Rotation != BuildingRotation.South)
                cost += EdgeUseCost;
            if (building.Position.X >= state.Size.X - building.Size.X && building.Rotation != BuildingRotation.East && building.Rotation != BuildingRotation.West)
                cost += EdgeUseCost;
            if (building.Position.Y >= state.Size.Y - building.Size.Y && building.Rotation != BuildingRotation.South && building.Rotation != BuildingRotation.North)
                cost += EdgeUseCost; 

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
            else if (building.Building.Name == "placed-item")
            {
                var converted = building as FlowBuilding;
                var above = collisions.Where((b) => !(b is UndergroundFlow));
                foreach (var contender in above)
                {
                    if (contender is ProductionBuilding)
                    {
                        var recipe = ((ProductionBuilding)contender).Recipe;
                        if (!recipe.Ingredients.Where((i) => i.Item == converted.Item.Item).Any() &&
                            !recipe.Results.Where((i) => i.Item == converted.Item.Item).Any())
                            cost += CollisionCost;
                    }
                    else if (contender is FlowBuilding)
                    {
                        if (((FlowBuilding)contender).Item.Item != converted.Item.Item)
                            cost += CollisionCost;
                    }
                    else
                    {
                        cost += CollisionCost;
                    }
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
