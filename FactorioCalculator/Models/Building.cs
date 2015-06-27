using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    [Serializable]
    class Building
    {
        public string Name { get; set; }
        public double ProductionSpeed { get; set; }
        public List<string> CraftingCategories { get; protected set; }
        public IEnumerable<Recipe> Recipes { get { return _library.Recipes.Where((p) => p.Buildings.Contains(this)); } }
        public double Energy { get; set; }
        public int IngredientCount { get; set; }
        public EnergySource EnergySource { get; set; }
        
        [NonSerialized]
        private Library _library;

        public Building(string name)
        {
            Name = name;
            CraftingCategories = new List<string>();
            ProductionSpeed = 0;
            Energy = 0;
            EnergySource = Models.EnergySource.None;
            IngredientCount = 0;
        }

        public void Initialize(Library library)
        {
            _library = library;
        }

        /// <summary>
        /// Calculates the maximum amount of times the recipe can be run in one second.
        /// </summary>
        /// <param name="recipe">The recipe to calculate the production speed for</param>
        /// <returns>The amount of time the recipe can be executed in one second</returns>
        public double MaxProductionFor(Recipe recipe)
        {
            if (ProductionSpeed <= 0)
                return 0;
            if (!CraftingCategories.Contains(recipe.CraftingCategory))
                return 0;
            if (recipe.Ingredients.Count() > IngredientCount)
                return 0;

            var duration = recipe.Time / ProductionSpeed;
            return 1 / duration;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
