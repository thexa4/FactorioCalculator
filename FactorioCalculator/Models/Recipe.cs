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
    class Recipe : SubModule
    {
        public string CraftingCategory { get; set; }
        public IEnumerable<Building> Buildings
        {
            get
            {
                var sameCategory = _library.Buildings.Where((b) => b.CraftingCategories.Contains(CraftingCategory));
                var enoughCapacity = sameCategory.Where((b) => b.IngredientCount >= Ingredients.Count());
                var result = enoughCapacity;
                if (Ingredients.Any((i) => i.Item.ItemType == ItemType.Fluid))
                    result = result.Where((b) => b.Fluidboxes.Any((z) => z.IsOutput == false));
                if (Results.Any((i) => i.Item.ItemType == ItemType.Fluid))
                    result = result.Where((b) => b.Fluidboxes.Any((z) => z.IsOutput == true));
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
