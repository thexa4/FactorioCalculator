using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class ProductionBuilding : ProductionStep, IPhysicalBuilding
    {
        public Point Position { get; protected set; }

        public ProductionBuilding(Recipe recipe, double amount, Building building, Point position)
            : base(recipe, amount, building)
        {
            Position = position;
        }
    }
}
