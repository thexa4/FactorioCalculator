using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    [Serializable]
    public class Recipe : SubModule
    {
        public string CraftingCategory { get; set; }
        public IEnumerable<Building> Buildings
        {
            get
            {
                var sameCategory = Library.Buildings.Where((b) => b.CraftingCategories.Contains(CraftingCategory));
                var enoughCapacity = sameCategory.Where((b) => b.IngredientCount >= Ingredients.Where((i) => !i.Item.IsVirtual).Count());
                var result = enoughCapacity;
                if (Ingredients.Any((i) => i.Item.ItemType == ItemType.Fluid))
                    result = result.Where((b) => b.FluidBoxes.Any((z) => z.IsOutput == false));
                if (Results.Any((i) => i.Item.ItemType == ItemType.Fluid))
                    result = result.Where((b) => b.FluidBoxes.Any((z) => z.IsOutput == true));
                return result;
            }
        }

        public Recipe(string name)
            : base(name)
        {
            Time = 0.5;
            CraftingCategory = "crafting";
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Recipe<{0}>", Name);
        }
    }
}
