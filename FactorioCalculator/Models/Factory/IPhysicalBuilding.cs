using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents a placable building on the map
    /// </summary>
    interface IPhysicalBuilding : IStep
    {
        /// <summary>
        /// The top left position of the building
        /// </summary>
        Vector2 Position { get; }
        /// <summary>
        /// The size of the building
        /// </summary>
        Vector2 Size { get; }
        /// <summary>
        /// The rotation of the building
        /// </summary>
        BuildingRotation Rotation { get; }
        /// <summary>
        /// The placable building
        /// </summary>
        Building Building { get; }
    }
}
