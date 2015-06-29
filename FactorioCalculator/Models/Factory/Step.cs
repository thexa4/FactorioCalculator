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
    class Step : IStep
    {
        /// <summary>
        /// The parent step
        /// </summary>
        public IStep Parent { get; set; }
        
        /// <summary>
        /// The step(s) that precede this one
        /// </summary>
        public HashSet<IStep> Previous { get; set; }

        public Step()
        {
            Previous = new HashSet<IStep>();
        }
    }
}
