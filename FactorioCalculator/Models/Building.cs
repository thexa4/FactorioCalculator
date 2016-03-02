using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FactorioCalculator.Models
{
    [Serializable]
    public class Building
    {
        public string Name { get; set; }
        public double ProductionSpeed { get; set; }
        public IEnumerable<string> CraftingCategories { get { return InternalCraftingCategories; } }
        protected Collection<string> InternalCraftingCategories { get; private set; }
        public IEnumerable<Recipe> Recipes { get { return _library.Recipes.Where((p) => p.Buildings.Contains(this)); } }
        public string IconPath { get; set; }
        public double Energy { get; set; }
        public int IngredientCount { get; set; }
        public EnergySource EnergySource { get; set; }

        public IEnumerable<FluidBox> FluidBoxes { get { return OriginalFluidBoxes; } }
        protected Collection<FluidBox> OriginalFluidBoxes { get; private set; }
        public Vector2 Size { get; set; }
        public bool HidesFluidBox{ get; set; }
        
        [NonSerialized]
        private Library _library;

        public Building(string name)
        {
            Name = name;
            InternalCraftingCategories = new Collection<string>();
            ProductionSpeed = 0;
            Energy = 0;
            EnergySource = Models.EnergySource.None;
            IngredientCount = 0;
            OriginalFluidBoxes = new Collection<FluidBox>();
            Size = Vector2.One;
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
            if (recipe == null)
                throw new ArgumentNullException("recipe");
            if (ProductionSpeed <= 0)
                return 0;
            if (!InternalCraftingCategories.Contains(recipe.CraftingCategory))
                return 0;
            if (recipe.Ingredients.Where((i) => !i.Item.IsVirtual).Count() > IngredientCount)
                return 0;

            var duration = recipe.Time / ProductionSpeed;
            return 1 / duration;
        }

        public void AddCraftingCategory(string category) {
            InternalCraftingCategories.Add(category);
        }

        public void AddFluidBox(FluidBox box)
        {
            OriginalFluidBoxes.Add(box);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
