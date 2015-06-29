using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class ProductionStep : TransformStep
    {
        public Building Building { get; private set; }

        public double MaxAmount { get { return Building.MaxProductionFor(Recipe); } }

        public ProductionStep(Recipe recipe, double amount, Building building)
            : base(recipe, amount)
        {
            Building = building;

            if (MaxAmount == 0)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Building {0} can't craft {1}!", building.Name, recipe.Name));
            if (amount > MaxAmount)
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Production amount of {0} exceeds maximum capacity of {1}!", amount, MaxAmount));
        }
    }
}
