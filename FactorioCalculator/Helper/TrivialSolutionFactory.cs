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
    public static class TrivialSolutionFactory
    {
        public static RecipeGraph CreateFactory(RecipeGraph recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException("recipe");

            var inputs = new List<SourceStep>();
            var outputs = new List<SinkStep>();
            var wastes = new List<SinkStep>();
            var resources = new List<FlowStep>();
            var transforms = new List<TransformStep>();

            foreach (var resource in recipe.Resources)
                resources.Add(new FlowStep(resource.Item) { Parent = resource });

            foreach(var input in recipe.InputNodes) {
                var r = new FlowStep(input.Item) { Parent = input };
                var s = new SourceStep(input.Item) { Parent = input };

                r.Previous.Add(s);

                resources.Add(r);
                inputs.Add(s);
            }

            foreach (var output in recipe.OutputNodes)
            {
                var r = new FlowStep(output.Item) { Parent = output };
                var o = new SinkStep(output.Item) { Parent = output };

                o.Previous.Add(r);

                resources.Add(r);
                outputs.Add(o);
            }

            foreach (var waste in recipe.WasteNodes)
            {
                var w = new SinkStep(waste.Item) { Parent = waste };
                wastes.Add(w);
                w.Previous.Add(resources.Where((r) => r.Item.Item == waste.Item.Item).First());
            }

            foreach (var transform in recipe.Transformations)
            {
                var amount = transform.Amount;
                var transformRecipe = transform.Recipe;
                var building = FirstMatchingBuilding(transformRecipe.Buildings);
                var modTime = building.MaxProductionFor(transformRecipe);
                var nrOfFactories = Math.Ceiling(amount / modTime);

                for (int i = 0; i < nrOfFactories; i++)
                {
                    var p = new ProductionStep(transform.Recipe, transform.Amount / nrOfFactories, building) { Parent = transform };
                    foreach (var input in transformRecipe.Ingredients)
                        p.Previous.Add(resources.Where((r) => r.Item.Item == input.Item).First());

                    foreach (var output in transformRecipe.Results)
                        resources.Where((r) => r.Item.Item == output.Item).First().Previous.Add(p);

                    transforms.Add(p);
                }
            }

            return new RecipeGraph(wastes, inputs, outputs, resources, transforms);
        }

        /// <summary>
        /// Results lowest factory to build
        /// </summary>
        /// <param name="buildings"></param>
        /// <returns></returns>
        private static Building FirstMatchingBuilding(IEnumerable<Building> buildings) {
            var filtered = buildings.Where((building) => building.Recipes.Any())
                .Where((b) => b.Name != "player");
            
            return filtered.OrderBy(building => building.IngredientCount).First();
        }
    }
}
