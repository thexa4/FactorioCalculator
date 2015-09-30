using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Helper;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class GroundToUnderground : PhysicalFlowBuilding
    {
        public Depth FlowDepth { get; set; }
        public bool IsUpward { get; set; }

        public GroundToUnderground(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation, Depth depth, bool isUpward)
            : base(item, building, position, rotation)
        {
            FlowDepth = depth;
            IsUpward = isUpward;
        }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            var collisions = space.CalculateCollisions(Position, Size).OfType<UndergroundFlow>();
            var sameDepth = collisions.Where((b) => b.FlowDepth == FlowDepth);
            var sameDir = sameDepth.Where((f) => f.Rotation == Rotation || f.Rotation == Rotation.Invert());

            double cost = base.CalculateCost(space, grader) + grader.CollisionCost * sameDir.Count();

            if (IsUpward)
            {
                if (this.SolidLeaks(space).Any())
                    cost += grader.TouchLeakCost;
            }

            return cost;
        }
    }
}
