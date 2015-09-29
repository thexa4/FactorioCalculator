using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.PlaceRoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory.Physical
{
    interface IAccountableBuilding : IPhysicalBuilding
    {
        double CalculateCost(Searchspace space, SolutionGrader grader);
    }
}
