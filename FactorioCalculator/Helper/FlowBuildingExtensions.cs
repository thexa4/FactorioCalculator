using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.PlaceRoute;
using FactorioCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorioCalculator.Models.Factory.Physical;

namespace FactorioCalculator.Helper
{
    static class FlowBuildingExtensions
    {
        public static IEnumerable<FlowBuilding> SolidLeaks(this FlowBuilding building, Searchspace space)
        {
            if (building is Splitter)
            {
                var offset = new BuildingRotation[] {
                    BuildingRotation.East,
                    BuildingRotation.South,
                    BuildingRotation.East,
                    BuildingRotation.South,
                };

                return SolidPointLeaks(building, Vector2.Zero, space)
                    .Concat(SolidPointLeaks(building, offset[(int)building.Rotation].ToVector(), space));
            }
            else
            {
                return SolidPointLeaks(building, Vector2.Zero, space);
            }
        }

        public static IEnumerable<FlowBuilding> SolidPointLeaks(this FlowBuilding building, Vector2 offset, Searchspace space)
        {
            var forward = space.CalculateCollisions(building.Position + building.Rotation.ToVector()).ToList();
            var belts = forward.OfType<Belt>();
            var splitters = forward.OfType<Splitter>();
            var ground = forward.OfType<GroundToUnderground>().Where((g) => g.FlowDepth != Depth.Fluid);

            var filteredBelts = belts.Where((b) => b.Item.Item != building.Item.Item);
            var filteredSplitters = splitters.Where((s) => s.Item.Item != building.Item.Item && s.Rotation == building.Rotation);
            var filteredGround = ground.Where((g) => !g.IsUpward && g.Rotation == building.Rotation.Invert() && g.Item.Item != building.Item.Item);

            return filteredBelts.Cast<FlowBuilding>()
                .Concat(filteredSplitters)
                .Concat(filteredGround);
        }
    }
}
