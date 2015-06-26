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
    public interface IStep
    {
        /// <summary>
        /// The parent step
        /// </summary>
        IStep Parent { get; }
        /// <summary>
        /// The step(s) that precede this one
        /// </summary>
        ImmutableHashSet<IStep> Previous { get; }
    }
}