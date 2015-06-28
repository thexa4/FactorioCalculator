using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class SinkStep : Step
    {
        public ItemAmount Amount { get; private set; }

        public SinkStep(IStep parent, IEnumerable<IStep> previous, ItemAmount amount)
        {
            Amount = amount;
        }

        public override string ToString()
        {
            return String.Format("Sink<{0}>", Amount);
        }
    }
}
