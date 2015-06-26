using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FactorioCalculator.Models
{
    [Serializable]
    class ItemAmount
    {
        public double Amount { get; protected set; }
        public Item Item
        {
            get { return _library.Items.Where((i) => i.Name == _itemName).First(); }
        }

        private readonly string _itemName;

        [NonSerialized]
        private Library _library;

        public ItemAmount(string itemName, double amount)
        {
            _itemName = itemName;
            Amount = amount;
        }

        public ItemAmount(Item item, double amount) : this(item.Name, amount) { }

        public void Initialize(Library library)
        {
            _library = library;
        }

        public override string ToString()
        {
            return string.Format("ItemAmount<{0}, {1}>", Item, Amount);
        }
    }
}
