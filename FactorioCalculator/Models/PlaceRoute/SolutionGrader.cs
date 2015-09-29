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
        private Dictionary<string, double> CostLookup = new Dictionary<string, double>(){
            {"long-handed-inserter", 18},
            {"fast-inserter", 20},
            {"basic-inserter", 8},
            {"basic-belt-to-ground", 20},
            {"pipe-to-ground", 20},
            //{"underground-flow", -1},
        };

        public double ProductionCollisionCost { get; set; }
        public double TouchLeakCost { get; set; }
        public double CollisionCost { get; set; }
        public double LandUseCost { get; set; }
        public double EdgeUseCost { get; set; }
        public double AreaCost { get; set; }

        public SolutionGrader()
        {
            ProductionCollisionCost = 1000;
            TouchLeakCost = 10;
            CollisionCost = 30;
            LandUseCost = 1;
            EdgeUseCost = 5;
            AreaCost = 1;
        }

        public double CostForSolution(Searchspace state)
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

        public double CostForBuilding(Searchspace state, IPhysicalBuilding building)
        {
            if (building == null)
                throw new ArgumentNullException("building");

            double cost = building.Size.X * building.Size.Y * LandUseCost;

            if (building.Position.X <= 0 && building.Rotation != BuildingRotation.West && building.Rotation != BuildingRotation.East)
                cost += EdgeUseCost;
            if (building.Position.Y <= 0 && building.Rotation != BuildingRotation.North && building.Rotation != BuildingRotation.South)
                cost += EdgeUseCost;
            if (building.Position.X >= state.Size.X - building.Size.X && building.Rotation != BuildingRotation.East && building.Rotation != BuildingRotation.West)
                cost += EdgeUseCost;
            if (building.Position.Y >= state.Size.Y - building.Size.Y && building.Rotation != BuildingRotation.South && building.Rotation != BuildingRotation.North)
                cost += EdgeUseCost;

            var accountable = building as IAccountableBuilding;
            if (accountable != null)
                cost += accountable.CalculateCost(state, this);

            if (CostLookup.ContainsKey(building.Building.Name))
                cost += CostLookup[building.Building.Name];

            var collisions = state.CalculateCollisions(building.Position, building.Size).Where((b) => b != building);

            if (building is UndergroundFlow)
            {
                
            }
            else if (building is GroundToUnderground)
            {
                var cur = building as GroundToUnderground;

                foreach (var collision in collisions)
                {
                    var flow = collision as UndergroundFlow;
                    if (flow != null)
                    {
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
                    var productionContender = contender as ProductionBuilding;
                    var flowContender = contender as FlowBuilding;
                    if (productionContender != null)
                    {
                        var recipe = productionContender.Recipe;
                        if (!recipe.Ingredients.Where((i) => i.Item == converted.Item.Item).Any() &&
                            !recipe.Results.Where((i) => i.Item == converted.Item.Item).Any())
                            cost += CollisionCost;
                    }
                    else if (flowContender != null)
                    {
                        if (flowContender.Item.Item != converted.Item.Item)
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
