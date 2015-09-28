using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents a spawn point of a certain resource
    /// </summary>
    public class SourceStep : FlowStep
    {
        public SourceStep(ItemAmount amount)
            : base(amount)
        {

        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Source<{0}>", Item);
        }
    }
}
