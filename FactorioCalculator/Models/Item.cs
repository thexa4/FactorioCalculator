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
    class Item
    {
        public string Name { get; set; }
        public double FuelValue { get; set; }
        public string SubGroup { get; set; }
        public string PlaceResultName { get; set; }

        public bool IsResource { get; set; }

        public ItemType ItemType { get; set; }
        public Building PlaceResult
        {
            get
            {
                if (String.IsNullOrWhiteSpace(PlaceResultName))
                    return null;
                return _library.Buildings.Where((b) => b.Name == PlaceResultName).First();
            }
            set
            {
                PlaceResultName = value != null ? value.Name : null;
            }
        }

        public IEnumerable<Recipe> Recipes
        {
            get
            {
                return _library.Recipes.Where((r) => r.Results.Where((res) => res.Item == this).Any());
            }
        }
        
        [NonSerialized]
        internal Library _library;        

        public Item(string name)
        {
            Name = name;
            FuelValue = 0;
        }

        public void Initialize(Library library)
        {
            _library = library;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Item<{0}>", Name);
        }
    }
}
