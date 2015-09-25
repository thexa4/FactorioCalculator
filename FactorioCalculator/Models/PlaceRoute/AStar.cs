using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    class AStar<T> where T : IRouteState
    {
        public double DistanceCost { get; protected set; }
        protected HeapPriorityQueue<AStarState> _queue;
        protected HashSet<Vector2> _destinations = new HashSet<Vector2>();
        public T EndState { get; private set; }

        public Func<T, IEnumerable<T>> StateGenerator { get; set; }
        public Func<T, HashSet<Vector2>, bool> EndStateValidator { get; set; }

        public AStar(double distanceCost = 10)
        {
            DistanceCost = distanceCost;
            _queue = new HeapPriorityQueue<AStarState>(1000 * 1000);
        }

        public void AddDestination(Vector2 position)
        {
            _destinations.Add(position);
        }

        public void AddState(T state)
        {
            if (_destinations.Contains(state.Position))
            {
                EndState = state;
                return;
            }

            foreach (var newState in StateGenerator(state))
            {
                var guessedCost = newState.Cost + CostHeuristic(newState.Position);
                if (newState.Position.X < 0 || newState.Position.Y < 0
                    || newState.Position.X >= newState.Space.Size.X || newState.Position.Y >= newState.Space.Size.Y)
                    continue;
                _queue.Enqueue(new AStarState(newState), guessedCost);
            }
        }

        public bool Step()
        {
            var state = _queue.Dequeue().State;
            if (EndStateValidator(state, _destinations))
            {
                EndState = state;
                return true;
            }

            AddState(state);
            return false;
        }

        public double CostHeuristic(Vector2 origin)
        {
            double min = double.MaxValue / DistanceCost;
            foreach(var destination in _destinations)
            {
                var offset = origin - destination;
                var dist = Math.Abs(offset.X) + Math.Abs(offset.Y);
                if (dist < min)
                    min = dist;
            }

            return min * DistanceCost;
        }

        protected class AStarState : PriorityQueueNode
        {
            public T State;

            public AStarState(T state)
            {
                State = state;
            }
        }
    }
}
