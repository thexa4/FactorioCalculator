using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models
{
    /// <summary>
    /// Represents a 2D coordinate
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The X value of the coordinate
        /// </summary>
        public double X { get { return _x; } }
        /// <summary>
        /// The Y value of the coordinate
        /// </summary>
        public double Y { get { return _y; } }
        private double _x;
        private double _y;

        public static readonly Vector2 One = new Vector2(1, 1);
        public static readonly Vector2 Zero = new Vector2(0, 0);

        /// <summary>
        /// Creates a new Vector with the specified coordinates
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Vector2(double x, double y)
        {
            _x = x;
            _y = y;
        }

        #region Convenience Methods
        public Vector2 Add(Vector2 other)
        {
            return this + other;
        }

        public Vector2 Subtract(Vector2 other)
        {
            return this - other;
        }

        public Vector2 Multiply(double other)
        {
            return this * other;
        }

        public Vector2 Divide(double other)
        {
            return this / other;
        }

        public Vector2 Transpose()
        {
            return new Vector2(Y, X);
        }
        #endregion
        #region Operators
        public static Vector2 operator *(double multiplier, Vector2 other)
        {
            return new Vector2(other.X * multiplier, other.Y * multiplier);
        }
        public static Vector2 operator *(Vector2 other, double multiplier)
        {
            return new Vector2(other.X * multiplier, other.Y * multiplier);
        }

        public static Vector2 operator /(double multiplier, Vector2 other)
        {
            return new Vector2(other.X / multiplier, other.Y / multiplier);
        }

        public static Vector2 operator /(Vector2 other, double multiplier)
        {
            return new Vector2(other.X / multiplier, other.Y / multiplier);
        }

        public static Vector2 operator +(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X + second.X, first.Y + second.Y);
        }

        public static Vector2 operator -(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }

        public static bool operator ==(Vector2 first, Vector2 second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Vector2 first, Vector2 second)
        {
            return !first.Equals(second);
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            {
                var cast = (Vector2)obj;
                return cast.X == X && cast.Y == Y;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new Tuple<double, double>(X, Y).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", X, Y);
        } 
    }
}
