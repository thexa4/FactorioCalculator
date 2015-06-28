using System;
using System.Collections.Generic;
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
        public IEnumerable<Building> Buildings { get { return _library.Buildings.Where((b) => b.CraftingCategories.Contains(CraftingCategory)); } }

        public Recipe(string name)
            : base(name)
        {
            Time = 0.5;
            CraftingCategory = "crafting";
        }

        public override string ToString()
        {
            return string.Format("Recipe<{0}>", Name);
        }
    }
}
