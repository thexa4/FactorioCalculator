using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class PlacedItem : PhysicalFlowBuilding
    {
        private static Building _building = new Building("placed-item");

        public PlacedItem(ItemAmount item, Vector2 position) : base(item, _building, position, BuildingRotation.North)
        {

        }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            double cost = 0;

            var above = space.CalculateCollisions(Position).Where((b) => !(b is UndergroundFlow))
                .Where((b) => b != this);

            foreach (var contender in above)
            {
                var productionContender = contender as ProductionBuilding;
                var flowContender = contender as FlowBuilding;
                if (productionContender != null)
                {
                    var recipe = productionContender.Recipe;
                    if (!recipe.Ingredients.Where((i) => i.Item == Item.Item).Any() &&
                        !recipe.Results.Where((i) => i.Item == Item.Item).Any())
                        cost += grader.CollisionCost;
                }
                else if (flowContender != null)
                {
                    if (flowContender.Item.Item != Item.Item)
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
