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
        private List<IStep> tempInput;

        public TrivialSolutionFactory(Library library, IEnumerable<IStep> inputEnumerable)
        {
            this.library = library;

            foreach (IStep step in inputEnumerable)
            {
                if (step is TransformStep) {
                    TransformStep transform = (TransformStep)step;

                    var amount = transform.Amount;
                    var recipe = transform.Recipe;
                    var mod = 0;
                    var building = recipe.Buildings;
                    
                    var modTime = recipe.Time * 0.5;
                    
                }
            }
        }

        //Results lowest factory to build
        private String checkBuilding(List<Building> buildings) {
            buildings.RemoveAll((building) => !building.Recipes.Any());
            
            buildings.OrderBy(building => building.IngredientCount);
            
            foreach (Building building in buildings) {

            }

            return null;
        }
    }
}
