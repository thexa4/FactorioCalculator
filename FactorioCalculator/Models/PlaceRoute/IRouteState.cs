using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    interface IRouteState
    {
        double Cost { get; }
        Vector2 Position { get; }
        IPhysicalBuilding Building { get; }
        Searchspace Space { get; }
        RoutingCoordinate RoutingCoord { get; }
    }
}
