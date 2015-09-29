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
        protected HashSet<RoutingCoordinate> _destinations = new HashSet<RoutingCoordinate>();
        public T EndState { get; private set; }

        public Func<T, IEnumerable<T>> StateGenerator { get; set; }
        public Func<T, HashSet<RoutingCoordinate>, bool> EndStateValidator { get; set; }

        public AStar(double distanceCost = 5)
        {
            DistanceCost = distanceCost;
            _queue = new HeapPriorityQueue<AStarState>(300 * 1000);
        }

        public void AddDestination(RoutingCoordinate position)
        {
            _destinations.Add(position);
        }

        public void AddState(T state)
        {
            if (_destinations.Contains(state.RoutingCoord))
            {
                EndState = state;
                return;
            }

            foreach (var newState in StateGenerator(state))
            {
                var guessedCost = newState.Cost + CostHeuristic(newState.Position);
                _queue.Enqueue(new AStarState(newState), guessedCost);
            }
        }

        public bool Step()
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("No route found!");

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
                var offset = origin - destination.Position;
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
