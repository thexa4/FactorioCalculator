using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class GroundToUnderground : FlowBuilding
    {
        public Depth FlowDepth { get; set; }

        public GroundToUnderground(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation, Depth depth)
            : base(item, building, position, rotation)
        {
            FlowDepth = depth;
        }
    }
}
