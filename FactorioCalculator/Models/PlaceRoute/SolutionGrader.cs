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
            ProductionCollisionCost = 2000;
            TouchLeakCost = 10;
            CollisionCost = 20;
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

            return cost;
        }
    }
}
