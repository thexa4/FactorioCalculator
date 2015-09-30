using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class Splitter : PhysicalFlowBuilding
    {
        public Splitter(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation) : base(item, building, position, rotation)
        {

        }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
#warning No leak cost taken into account yet
            var cost = 0.0;
            var collisions = space.CalculateCollisions(Position, Size).Where((b) => b != this && !(b is UndergroundFlow));
            foreach (var collision in collisions)
            {
                var belt = collision as Belt;
                if (belt != null)
                {
                    if (belt.Item.Item != Item.Item || belt.Rotation != Rotation)
                        cost += grader.CollisionCost;
                }
                else
                {
                    cost += grader.CollisionCost;
                }
                
            }
            return cost;
        }
    }
}
