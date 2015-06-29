using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class SinkStep : FlowStep
    {
        public SinkStep(ItemAmount amount)
            : base(amount)
        {
        }

        public override string ToString()
        {
            return String.Format("Sink<{0}>", Item);
        }
    }
}
