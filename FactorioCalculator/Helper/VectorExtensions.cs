using FactorioCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    static class VectorExtensions
    {
        public static Vector2 Rotate(this Vector2 vector, BuildingRotation rotation)
        {
            switch (rotation)
            {
                case BuildingRotation.North:
                    return vector;
                case BuildingRotation.East:
                    return new Vector2(vector.Y, vector.X);
                case BuildingRotation.South:
                    return new Vector2(vector.X, -vector.Y);
                case BuildingRotation.West:
                    return new Vector2(-vector.X, vector.Y);
            }
            return vector;
        }

        public static Vector2 RotateAbsolute(this Vector2 vector, BuildingRotation rotation)
        {
            var result = Rotate(vector, rotation);
            return new Vector2(Math.Abs(result.X), Math.Abs(result.Y));
        }
    }
}
