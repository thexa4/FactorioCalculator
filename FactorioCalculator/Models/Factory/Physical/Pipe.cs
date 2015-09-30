using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.PlaceRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class Pipe : PhysicalFlowBuilding
    {
        public Pipe(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item, building, position, rotation) { }

        public double CalculateCost(Searchspace space, SolutionGrader grader)
        {
            var cost = base.CalculateCost(space, grader);

            var pipeBuilding = Building;

            BuildingRotation[] rotations = new BuildingRotation[] { BuildingRotation.North, BuildingRotation.East, BuildingRotation.South, BuildingRotation.West };
            foreach (var rotation in rotations)
            {
                var neighbors = space.CalculateCollisions(Position + rotation.ToVector());
                var misMatch = neighbors.Where((b) => b.Building == pipeBuilding && b is FlowBuilding).Cast<FlowBuilding>()
                    .Where((f) => f.Item.Item != Item.Item);


                cost += misMatch.Count() * grader.TouchLeakCost;
            }

            return cost;
        }
    }
}
