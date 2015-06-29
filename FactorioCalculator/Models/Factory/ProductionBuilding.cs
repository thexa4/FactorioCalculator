using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class ProductionBuilding : ProductionStep, IPhysicalBuilding
    {
        public Vector2 Position { get; protected set; }
        public IEnumerable<FluidBox> FluidBoxes
        {
            get
            {
                IEnumerable<FluidBox> result = new FluidBox[]{};
                var inputs = Building.Fluidboxes.Where((b) => b.IsOutput == false);
                var outputs = Building.Fluidboxes.Where((b) => b.IsOutput == true);
                if (!Building.HidesFluidBox || Recipe.Ingredients.Where((i) => i.Item.ItemType == ItemType.Fluid).Any())
                    result = result.Concat(inputs);
                if (!Building.HidesFluidBox || Recipe.Results.Where((i) => i.Item.ItemType == ItemType.Fluid).Any())
                    result = result.Concat(outputs);
                return result;
            }
        }

        public ProductionBuilding(Recipe recipe, double amount, Building building, Vector2 position)
            : base(recipe, amount, building)
        {
            Position = position;
        }
    }
}
