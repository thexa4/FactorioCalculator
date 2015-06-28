using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    interface IPhysicalBuilding : IStep
    {
        /// <summary>
        /// The top left position of the building
        /// </summary>
        Point Position { get; }
        Building Building { get; }
    }
}
