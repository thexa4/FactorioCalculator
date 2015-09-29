using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    public struct CollisionBox<T> where T : IPhysicalBuilding
    {
        public Vector2 Position { get { return _position; } }
        public Vector2 Size { get { return _size; } }

        public T Step { get { return _step; } }

        private Vector2 _position;
        private Vector2 _size;
        private T _step;

        public CollisionBox(T step)
        {
            _position = step.Position;
            _size = step.Size;
            _step = step;
        }

        public override int GetHashCode()
        {
            return new Tuple<Vector2, Vector2, T>(_position, _size, _step).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CollisionBox<T>))
                return false;

            return Equals((CollisionBox<T>)obj);
        }

        public bool Equals(CollisionBox<T> other)
        {
            return other._position == _position &&
                other._size == _size &&
                other._step.Equals(_step);
        }

        public static bool operator ==(CollisionBox<T> box1, CollisionBox<T> box2)
        {
            return box1.Equals(box2);
        }

        public static bool operator !=(CollisionBox<T> box1, CollisionBox<T> box2)
        {
            return !box1.Equals(box2);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Collision<{0}>", _step);
        }
    }
}
