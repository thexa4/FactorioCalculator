using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
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

        public override string ToString()
        {
            return string.Format("Collision<{0}>", _step);
        }
    }
}
