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

        public ItemAmount(Item item, double amount) : this(item.Name, amount)
        {
            Initialize(item._library);
        }

        public void Initialize(Library library)
        {
            _library = library;
        }

        #region Operators
        public static ItemAmount operator *(double multiplier, ItemAmount amount)
        {
            return new ItemAmount(amount.Item, amount.Amount * multiplier);
        }
        public static ItemAmount operator *(ItemAmount amount, double multiplier)
        {
            return new ItemAmount(amount.Item, amount.Amount * multiplier);
        }

        public static ItemAmount operator /(double multiplier, ItemAmount amount)
        {
            return new ItemAmount(amount.Item, amount.Amount / multiplier);
        }
        public static ItemAmount operator /(ItemAmount amount, double multiplier)
        {
            return new ItemAmount(amount.Item, amount.Amount / multiplier);
        }

        public static ItemAmount operator +(ItemAmount item1, ItemAmount item2)
        {
            if (item1.Item != item2.Item)
                throw new Exception("Items don't match");
            return new ItemAmount(item1.Item, item1.Amount + item2.Amount);
        }

        public static ItemAmount operator -(ItemAmount item1, ItemAmount item2)
        {
            if (item1.Item != item2.Item)
                throw new Exception("Items don't match");
            return new ItemAmount(item1.Item, item1.Amount - item2.Amount);
        }

        public static ItemAmount operator +(double addition, ItemAmount amount)
        {
            return new ItemAmount(amount.Item, amount.Amount + addition);
        }
        public static ItemAmount operator +(ItemAmount amount, double addition)
        {
            return new ItemAmount(amount.Item, amount.Amount + addition);
        }

        public static ItemAmount operator -(double addition, ItemAmount amount)
        {
            return new ItemAmount(amount.Item, amount.Amount - addition);
        }
        public static ItemAmount operator -(ItemAmount amount, double addition)
        {
            return new ItemAmount(amount.Item, amount.Amount - addition);
        }

        public static ItemAmount operator -(ItemAmount amount)
        {
            return new ItemAmount(amount.Item, -amount.Amount);
        }
        #endregion

        public override string ToString()
        {
            return string.Format("ItemAmount<{0}, {1}>", Item, Amount);
        }
    }
}
