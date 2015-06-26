using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents an abstract step in the production process
    /// </summary>
    class Step
    {
        /// <summary>
        /// The parent step
        /// </summary>
        public IStep Parent { get; private set; }
        /// <summary>
        /// The step(s) that precede this one
        /// </summary>
        public ImmutableHashSet<IStep> Previous { get; private set; }

        public Step(IStep parent, IEnumerable<IStep> previous)
        {
            Parent = parent;
            Previous = previous.ToImmutableHashSet();
        }
    }
}
