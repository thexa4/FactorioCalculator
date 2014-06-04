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
    class Library
    {
        public IEnumerable<Item> Items { get { return _items; } }
        public IEnumerable<Building> Buildings { get { return _buildings; } }
        public IEnumerable<Recipe> Recipes { get { return _recipes; } }

        private List<Item> _items = new List<Item>();
        private List<Building> _buildings = new List<Building>();
        private List<Recipe> _recipes = new List<Recipe>();

        public Dictionary<Item, List<RecipeChain>> RecipeChains { get; private set; }

        public void Initialize()
        {
            foreach (var i in _items)
                i.Initialize(this);
            foreach (var b in _buildings)
                b.Initialize(this);
            foreach (var r in _recipes)
                r.Initialize(this);

            RecipeChains = RecipeChain.InitializeChainDictionary(this);
        }

        public void AddItem(Item item)
        {
            item.Initialize(this);
            _items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            _items.Remove(item);
        }

        public void AddBuilding(Building building)
        {
            building.Initialize(this);
            _buildings.Add(building);
        }

        public void RemoveBuilding(Building building)
        {
            _buildings.Remove(building);
        }

        public void AddRecipe(Recipe process)
        {
            process.Initialize(this);
            _recipes.Add(process);
        }

        public void RemoveRecipe(Recipe process)
        {
            _recipes.Remove(process);
        }
    }
}
