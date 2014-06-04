using System;
using System.Collections.Generic;
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
        
        [NonSerialized]
        private Library _library;        

        public Item(string name)
        {
            Name = name;
            FuelValue = 0;
        }

        public void Initialize(Library library)
        {
            _library = library;
        }
    }
}
