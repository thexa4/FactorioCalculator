using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class Belt : PhysicalFlowBuilding
    {
        public Belt(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item, building, position, rotation) { }
    }
}
