using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    [Serializable]
    class Recipe
    {
        public string Name { get; set; }
        public double Time { get; set; }
        public string CraftingCategory { get; set; }
        public IEnumerable<Building> Buildings { get { return _library.Buildings.Where((b) => b.CraftingCategories.Contains(CraftingCategory)); } }
        public IEnumerable<ItemAmount> Ingredients { get { return _ingredients; } }
        public IEnumerable<ItemAmount> Results { get { return _results; } }

        [XmlIgnore]
        private Library _library;
        
        private List<ItemAmount> _ingredients = new List<ItemAmount>();
        private List<ItemAmount> _results = new List<ItemAmount>();

        public Recipe(string name)
        {
            Name = name;
            Time = 0.5;
            CraftingCategory = "crafting";
        }

        public void Initialize(Library library)
        {
            _library = library;

            foreach (var i in _ingredients)
                i.Initialize(library);
            foreach (var r in _results)
                r.Initialize(library);
        }

        public void AddIngredient(ItemAmount amount)
        {
            amount.Initialize(_library);
            _ingredients.Add(amount);
        }

        public void RemoveIngredient(ItemAmount amount)
        {
            _ingredients.Remove(amount);
        }

        public void AddResult(ItemAmount amount)
        {
            amount.Initialize(_library);
            _results.Add(amount);
        }

        public void RemoveResult(ItemAmount amount)
        {
            _results.Remove(amount);
        }

        public override string ToString()
        {
            return string.Format("Recipe<{0}>", Name);
        }
    }
}
