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
    public class ItemAmount
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
            if (itemName == null)
                throw new ArgumentNullException("itemName");

            _itemName = itemName;
            Amount = amount;
        }

        public ItemAmount(Item item, double amount)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _itemName = item.Name;
            Amount = amount;
            Initialize(item._library);
        }

        public void Initialize(Library library)
        {
            _library = library;
        }

        #region Operators
        public static ItemAmount operator *(double multiplier, ItemAmount amount)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount * multiplier);
        }
        public static ItemAmount operator *(ItemAmount amount, double multiplier)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount * multiplier);
        }

        public static ItemAmount operator /(double multiplier, ItemAmount amount)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount / multiplier);
        }
        public static ItemAmount operator /(ItemAmount amount, double multiplier)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount / multiplier);
        }

        public static ItemAmount operator +(ItemAmount item1, ItemAmount item2)
        {
            if (item1 == null)
                throw new ArgumentNullException("item1");
            if (item2 == null)
                throw new ArgumentNullException("item2");

            if (item1.Item != item2.Item)
                throw new InvalidOperationException("Items don't match");
            return new ItemAmount(item1.Item, item1.Amount + item2.Amount);
        }

        public static ItemAmount operator -(ItemAmount item1, ItemAmount item2)
        {
            if (item1 == null)
                throw new ArgumentNullException("item1");
            if (item2 == null)
                throw new ArgumentNullException("item2");

            if (item1.Item != item2.Item)
                throw new InvalidOperationException("Items don't match");
            return new ItemAmount(item1.Item, item1.Amount - item2.Amount);
        }

        public static ItemAmount operator +(double addition, ItemAmount amount)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount + addition);
        }
        public static ItemAmount operator +(ItemAmount amount, double addition)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount + addition);
        }

        public static ItemAmount operator -(double addition, ItemAmount amount)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount - addition);
        }
        public static ItemAmount operator -(ItemAmount amount, double addition)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, amount.Amount - addition);
        }

        public static ItemAmount operator -(ItemAmount amount)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");

            return new ItemAmount(amount.Item, -amount.Amount);
        }
        #endregion

        #region Convenience methods
        public ItemAmount Multiply(double multiplier)
        {
            return this * multiplier;
        }

        public ItemAmount Divide(double multiplier)
        {
            return this / multiplier;
        }

        public ItemAmount Add(double addition)
        {
            return this + addition;
        }

        public ItemAmount Subtract(double subtraction)
        {
            return this - subtraction;
        }

        public ItemAmount Negate()
        {
            return -this;
        }
        #endregion

        public override int GetHashCode()
        {
            return new Tuple<string, double>(_itemName, Amount).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var amount = obj as ItemAmount;
            if (object.ReferenceEquals(amount, null))
                return false;

            return Equals(amount);
        }

        public bool Equals(ItemAmount other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            return other._itemName == _itemName &&
                other.Amount == Amount;
        }

        public static bool operator ==(ItemAmount box1, ItemAmount box2)
        {
            if (object.ReferenceEquals(box1, null))
                return object.ReferenceEquals(box2, null);

            return box1.Equals(box2);
        }

        public static bool operator !=(ItemAmount box1, ItemAmount box2)
        {
            if (object.ReferenceEquals(box1, null))
                return !object.ReferenceEquals(box2, null);

            return !box1.Equals(box2);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "ItemAmount<{0}, {1}>", Item, Amount);
        }
    }
}
