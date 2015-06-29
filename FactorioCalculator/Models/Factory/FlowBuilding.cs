using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class FlowBuilding : FlowStep, IPhysicalBuilding
    {
        public Vector2 Position { get; protected set; }
        public Building Building { get; protected set; }

        public FlowBuilding(ItemAmount item, Building building, Vector2 position)
            : base(item)
        {
            Item = item;
            Building = building;
            Position = position;
        }
    }
}
