using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    class TrivialSolutionFactory
    {
        private Library library;

        public TrivialSolutionFactory(Library library, IEnumerable<IStep> inputEnumerable)
        {
            this.library = library;

            foreach (IStep step in inputEnumerable)
            {
                var transform = step as TransformStep;
                var flow = step as FlowStep;

                if (transform != null) {
                    var amount = transform.Amount;
                    var recipe = transform.Recipe;
                    var building = FirstMatchingBuilding(recipe.Buildings);
                    var modTime = building.MaxProductionFor(recipe);
                    var nrOfFactories = Math.Ceiling(amount / modTime);

                    for (int i = 0; i < nrOfFactories; i++)
                    {
                        //place factory
                    }
                }
            }
        }

        /// <summary>
        /// Results lowest factory to build
        /// </summary>
        /// <param name="buildings"></param>
        /// <returns></returns>
        private static Building FirstMatchingBuilding(IEnumerable<Building> buildings) {
            var filtered = buildings.Where((building) => building.Recipes.Any());
            
            return filtered.OrderBy(building => building.IngredientCount).First();
        }
    }
}
