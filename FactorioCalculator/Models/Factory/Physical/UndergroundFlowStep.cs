using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class UndergroundFlow : FlowBuilding
    {
        public Depth FlowDepth { get; set; }

        public UndergroundFlow(ItemAmount item, Vector2 position, Depth depth, BuildingRotation rotation) : base(item, new Building("underground-flow"), position, rotation) {
            Position = position;
            FlowDepth = depth;
            Rotation = rotation;
            Size = Vector2.One;
        }
    }
}
