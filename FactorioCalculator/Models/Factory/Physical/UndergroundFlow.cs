using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class UndergroundFlow : FlowBuilding, IAccountableBuilding
    {
        public Depth FlowDepth { get; set; }

        public UndergroundFlow(ItemAmount item, Vector2 position, Depth depth, BuildingRotation rotation) : base(item, new Building("underground-flow"), position, rotation) {
            Position = position;
            FlowDepth = depth;
            Rotation = rotation;
            Size = Vector2.One;
        }

        public double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            var collisions = space.CalculateCollisions(Position, Size).Where((b) => b != this);
            double cost = 0;
            var flows = collisions.Where((c) => c is UndergroundFlow).Cast<UndergroundFlow>().Where((f) => f.FlowDepth == FlowDepth);
            var wells = collisions.Where((c) => c is GroundToUnderground).Cast<GroundToUnderground>().Where((w) => w.FlowDepth == FlowDepth);

            cost += flows.Where((f) => f.Rotation == Rotation || f.Rotation == Rotation.Invert()).Count() * grader.CollisionCost;
            cost += wells.Where((f) => f.Rotation == Rotation || f.Rotation == Rotation.Invert()).Count() * grader.CollisionCost;

            return cost;
        }
    }
}
