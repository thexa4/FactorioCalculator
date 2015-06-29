using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents a non-transforming flow of items
    /// </summary>
    class FlowStep : Step
    {
        public ItemAmount Item { get; set; }
        public FlowStep(ItemAmount item)
        {
            Item = item;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Flow<{0}>", Item);
        }
    }
}
