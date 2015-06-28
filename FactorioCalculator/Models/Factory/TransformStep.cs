using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents a transforming step in the process
    /// </summary>
    class TransformStep : Step
    {
        /// <summary>
        /// The recipe to use for the transform
        /// </summary>
        public Recipe Recipe { get; private set; }
        /// <summary>
        /// The amount times the recipe should run per second
        /// </summary>
        public double Amount { get; private set; }

        public TransformStep(IStep parent, IEnumerable<IStep> previous, Recipe recipe, double amount)
        {
            Recipe = recipe;
            Amount = amount;
        }

        public override string ToString()
        {
            return String.Format("Transform<{0} x {1}>", Recipe.Name, Amount);
        }
    }
}
