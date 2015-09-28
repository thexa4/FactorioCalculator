using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    public class SinkStep : FlowStep
    {
        public SinkStep(ItemAmount amount)
            : base(amount)
        {
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Sink<{0}>", Item);
        }
    }
}
