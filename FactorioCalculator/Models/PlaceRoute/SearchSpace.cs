﻿using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.PlaceRoute
{
    struct SearchSpace
    {
        public ImmutableList<CollisionBox<ProductionBuilding>> Components { get { return _components; } }
        public ImmutableList<CollisionBox<FlowBuilding>> Routes { get { return _routes; } }

        private ImmutableList<CollisionBox<ProductionBuilding>> _components;
        private ImmutableList<CollisionBox<FlowBuilding>> _routes;

        public IEnumerable<IPhysicalBuilding> Buildings { get { return Components.Select((c) => c.Step as IPhysicalBuilding).Concat(Routes.Select((c) => c.Step as IPhysicalBuilding)); } }

        public Vector2 Size { get { return _size; } }
        private Vector2 _size;

        public SearchSpace(Vector2 size) : this(size, ImmutableList<CollisionBox<ProductionBuilding>>.Empty, ImmutableList<CollisionBox<FlowBuilding>>.Empty) { }

        public SearchSpace(Vector2 size, ImmutableList<CollisionBox<ProductionBuilding>> components, ImmutableList<CollisionBox<FlowBuilding>> routes)
        {
            _size = size;
            _components = components;
            _routes = routes;
        }

        public SearchSpace AddComponent(ProductionBuilding building)
        {
            return new SearchSpace(Size, _components.Add(new CollisionBox<ProductionBuilding>(building)), _routes);
        }

        public SearchSpace AddRoute(FlowBuilding building)
        {
            return new SearchSpace(Size, _components, _routes.Add(new CollisionBox<FlowBuilding>(building)));
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 pos)
        {
            foreach (var building in Buildings)
            {
                var translated = building.Position - pos;
                if (translated.X > 0 || translated.Y > 0)
                    continue;
                if (translated.X < building.Size.X || translated.Y < building.Size.Y)
                    continue;
                yield return building;
            }
        }

        public IEnumerable<IPhysicalBuilding> CalculateCollisions(Vector2 pos, Vector2 size) {
            foreach (var building in Buildings)
            {
                var translated = building.Position - pos;
                if (translated.X > size.X || translated.Y > size.Y)
                    continue;
                if (translated.X < building.Size.X || translated.Y < building.Size.Y)
                    continue;
                yield return building;
            }
        }

        public bool IsValidSinkSourcePosition(Vector2 pos)
        {
            Vector2 offset = pos - Vector2.One - _size;
            return offset.X == 0 || offset.Y == 0 || pos.X == 0 | pos.Y == 0;
        }
    }
}