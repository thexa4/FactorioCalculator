using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class GroundToUnderground : PhysicalFlowBuilding
    {
        public Depth FlowDepth { get; set; }

        public GroundToUnderground(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation, Depth depth)
            : base(item, building, position, rotation)
        {
            FlowDepth = depth;
        }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            var collisions = space.CalculateCollisions(Position, Size).OfType<UndergroundFlow>();
            var sameDepth = collisions.Where((b) => b.FlowDepth == FlowDepth);
            var sameDir = sameDepth.Where((f) => f.Rotation == Rotation || f.Rotation == Rotation.Invert());
            return base.CalculateCost(space, grader) + grader.CollisionCost * sameDir.Count();
        }
    }
}
