using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    [Serializable]
    public class Library
    {
        public IEnumerable<Item> Items { get { return _items; } }
        public IEnumerable<Building> Buildings { get { return _buildings; } }
        public IEnumerable<Recipe> Recipes { get { return _recipes; } }

        private List<Item> _items = new List<Item>();
        private List<Building> _buildings = new List<Building>();
        private List<Recipe> _recipes = new List<Recipe>();

        public Dictionary<Item, RecipeGraph> BestRecipe { get; private set; }
        
        public void Initialize()
        {
            foreach (var i in _items)
                i.Initialize(this);
            foreach (var b in _buildings)
                b.Initialize(this);
            foreach (var r in _recipes)
                r.Initialize(this);

            BestRecipe = new Dictionary<Item, RecipeGraph>();
        }

        public void AddPowerPseudoItems()
        {
            Item joule = new Item("joule") { IsVirtual = true };
            AddItem(joule);
            Item water = _items.Where((i) => i.Name == "water").First();
            Item warmWater = new Item("water-warm") { IsVirtual = true };
            AddItem(warmWater);

            Building boiler = _buildings.Where((b) => b.Name == "boiler").First();
            boiler.AddCraftingCategory("boiling");
            
            double boilerPower = 390 * 1000;
            double boilerEffectivity = 0.5;
            double waterHeatCapacity = 1000;
            double waterNormalTemp = 15;
            double waterMaxTemp = 100;
            double waterTempDiff = waterMaxTemp - waterNormalTemp;
            double warmWaterEnergy = waterHeatCapacity * waterTempDiff;

            boiler.ProductionSpeed = boilerEffectivity;

            foreach (var item in _items.Where((i) => i.FuelValue > 0))
            {
                double expectedEnergy = item.FuelValue * boilerEffectivity;
                double burnTime = expectedEnergy / boilerPower;
                Recipe burnRecipe = new Recipe("burn-" + item.Name)
                {
                    CraftingCategory = "boiling",
                    Time = burnTime / boilerEffectivity,
                };
                double waterAmount = expectedEnergy / warmWaterEnergy;
                burnRecipe.AddIngredient(new ItemAmount(water, waterAmount));
                burnRecipe.AddIngredient(new ItemAmount(item, 1));
                burnRecipe.AddResult(new ItemAmount(warmWater, waterAmount));
                AddRecipe(burnRecipe);
            }

            Building steamEngine = _buildings.Where((b) => b.Name == "steam-engine").First();
            steamEngine.AddCraftingCategory("generator");
            steamEngine.ProductionSpeed = 1;
            steamEngine.IngredientCount = 1;

            Recipe generatorRecipe = new Recipe("generator-water")
            {
                CraftingCategory = "generator",
            };
            double generatorPower = 510 * 1000;
            generatorRecipe.AddIngredient(new ItemAmount(warmWater, generatorPower / warmWaterEnergy));
            generatorRecipe.AddResult(new ItemAmount(joule, generatorPower));
            generatorRecipe.Time = 1 / (0.1 * 60); // Generators use 0.1 water per tick.
            AddRecipe(generatorRecipe);
            var energyConversion = new Dictionary<string, double>() {
                {"crafting", 200 * 1000}, // Rough estimate of power use of all assembling machines divided by efficiency
                {"oil-processing", 420 * 1000},
                {"chemistry", 210 / 1.25 * 1000},
            };

            foreach(var conversion in energyConversion)
                foreach (var recipe in _recipes.Where((r) => r.CraftingCategory == conversion.Key))
                {
                    recipe.AddIngredient(new ItemAmount(joule, conversion.Value * recipe.Time)); 
                }
        }

        public void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            item.Initialize(this);
            _items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            _items.Remove(item);
        }

        public void AddBuilding(Building building)
        {
            if (building == null)
                throw new ArgumentNullException("building");

            building.Initialize(this);
            _buildings.Add(building);
        }

        public void RemoveBuilding(Building building)
        {
            _buildings.Remove(building);
        }

        public void AddRecipe(Recipe process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            process.Initialize(this);
            _recipes.Add(process);
        }

        public void RemoveRecipe(Recipe process)
        {
            _recipes.Remove(process);
        }
    }
}
