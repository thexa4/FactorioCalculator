using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class PhysicalFlowBuilding : FlowBuilding, IAccountableBuilding
    {
        public PhysicalFlowBuilding(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item, building, position, rotation) { }

        public virtual double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            var collisions = space.CalculateCollisions(Position, Size).Where((b) => b != this && !(b is UndergroundFlow));
            return collisions.Count() * grader.CollisionCost;
        }
    }
}
