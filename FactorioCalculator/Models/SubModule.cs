using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    public class SubModule
    {
        public string Name { get; set; }
        public IEnumerable<ItemAmount> Ingredients { get { return _ingredients; } }
        public IEnumerable<ItemAmount> Results { get { return _results; } }

        private List<ItemAmount> _ingredients = new List<ItemAmount>();
        private List<ItemAmount> _results = new List<ItemAmount>();

        public double Time { get; set; }

        [XmlIgnore]
        protected Library _library;

        public SubModule(string name)
        {
            Name = name;
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
            return string.Format(CultureInfo.InvariantCulture, "SubModule<{0}>", Name);
        }
    }
}
