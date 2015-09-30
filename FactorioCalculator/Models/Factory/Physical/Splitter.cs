using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    class Splitter : PhysicalFlowBuilding
    {
        public Splitter(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation) : base(item, building, position, rotation)
        {

        }

        public override double CalculateCost(PlaceRoute.Searchspace space, PlaceRoute.SolutionGrader grader)
        {
            return 0;
        }
    }
}
