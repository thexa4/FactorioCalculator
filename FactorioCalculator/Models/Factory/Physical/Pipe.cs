using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.PlaceRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    public class Pipe : FlowBuilding, IAccountableBuilding
    {
        public Pipe(ItemAmount item, Building building, Vector2 position, BuildingRotation rotation)
            : base(item, building, position, rotation) { }

        public double CalculateCost(Searchspace space, SolutionGrader grader)
        {
            return 0;
        }
    }
}
