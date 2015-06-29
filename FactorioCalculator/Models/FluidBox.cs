using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models
{
    class FluidBox
    {
        public ProductionType ProductionType { get; protected set; }
        public Point Position { get; protected set; }

        public FluidBox(ProductionType productionType, Point position)
        {
            ProductionType = productionType;
            Position = position;
        }
    }
}
