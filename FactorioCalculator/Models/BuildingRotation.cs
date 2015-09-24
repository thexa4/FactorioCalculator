using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models
{
    enum BuildingRotation
    {
        North,
        West,
        South,
        East,
    }

    static class BuildingDirectionMethods
    {
        public static Vector2 ToVector(this BuildingRotation direction)
        {
            switch (direction)
            {
                case BuildingRotation.North:
                    return new Vector2(0, -1);
                case BuildingRotation.East:
                    return new Vector2(1, 0);
                case BuildingRotation.South:
                    return new Vector2(0, 1);
                case BuildingRotation.West:
                    return new Vector2(-1, 0);
                default:
                    return Vector2.Zero;
            }
        }

        public static BuildingRotation Invert(this BuildingRotation rotation)
        {
            switch (rotation)
            {
                case BuildingRotation.North:
                    return BuildingRotation.South;
                case BuildingRotation.East:
                    return BuildingRotation.West;
                case BuildingRotation.South:
                    return BuildingRotation.North;
                case BuildingRotation.West:
                    return BuildingRotation.East;
                default:
                    return BuildingRotation.North;
            }
        }
    }
}
