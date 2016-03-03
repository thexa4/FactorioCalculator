using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Helper;

namespace FactorioCalculator.Models.Factory
{
    public class ProductionBuilding : ProductionStep, IPhysicalBuilding
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Size { get; protected set; }
        public BuildingRotation Rotation { get; protected set; }
        public IEnumerable<FluidBox> FluidBoxes
        {
            get
            {
                IEnumerable<FluidBox> filtered = new FluidBox[]{};
                var inputs = Building.FluidBoxes.Where((b) => b.IsOutput == false);
                var outputs = Building.FluidBoxes.Where((b) => b.IsOutput == true);
                if (!Building.HidesFluidBox || Recipe.Ingredients.Where((i) => i.Item.ItemType == ItemType.Fluid).Any())
                    filtered = filtered.Concat(inputs);
                if (!Building.HidesFluidBox || Recipe.Results.Where((i) => i.Item.ItemType == ItemType.Fluid).Any())
                    filtered = filtered.Concat(outputs);
                var result = filtered.Select((b) => new FluidBox(b.IsOutput, (b.Position - (Size - Vector2.One) / 2).Rotate(Rotation) + (Size - Vector2.One) / 2));
                return result;
            }
        }

        public ProductionBuilding(Recipe recipe, double amount, Building building, Vector2 position, BuildingRotation rotation)
            : base(recipe, amount, building)
        {
            if (recipe == null)
                throw new ArgumentNullException("recipe");
            if (building == null)
                throw new ArgumentNullException("building");

            Position = position;
            Size = building.Size.RotateAbsolute(rotation);
            Rotation = rotation;
        }
    }
}
