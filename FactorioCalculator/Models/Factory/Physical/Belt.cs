using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Helper;

namespace FactorioCalculator.Models.Factory.Physical
{
    class Belt : PhysicalFlowBuilding
    {
        public Belt(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item, building, position, rotation) { }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            var cost = base.CalculateCost(space, grader);

            var forward = space.CalculateCollisions(Position + Rotation.ToVector()).OfType<Belt>();
            if (forward.Where((b) => b.Item.Item == Item.Item && b.Rotation == Rotation.Invert()).Any())
                cost += grader.CollisionCost;

            if (this.SolidLeaks(space).Any())
                cost += grader.TouchLeakCost;

            for (int i = 0; i < 4; i++)
            {
                var dir = ((BuildingRotation)i);
                var buildings = space.CalculateCollisions(Position + dir.ToVector()).OfType<FlowBuilding>();

                foreach (var building in buildings)
                    if (building.SolidLeaks(space).Contains(this))
                        cost += grader.TouchLeakCost;
            }

            return cost;
        }
    }
}
