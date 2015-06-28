using System;
using System.Collections.Generic;
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
        public ItemAmount Item { get; protected set; }
        public FlowStep(IStep parent, IEnumerable<IStep> previous, ItemAmount item)
        {
            Item = item;
        }
    }
}
