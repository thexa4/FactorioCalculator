using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models
{
    class RecipeChain
    {
        public Recipe Current { get; set; }

        public List<RecipeChain> Ingredients { get; private set; }

        public IEnumerable<Item> RawIngredients
        {
            get
            {
                foreach (var item in Current.Ingredients.Select((i) => i.Item))
                    yield return item;

                foreach (var child in Ingredients)
                    foreach (var item in child.RawIngredients)
                        yield return item;
            }
        }

        public IEnumerable<Item> RawOutput
        {
            get
            {
                foreach (var item in Current.Results.Select((i) => i.Item))
                    yield return item;

                foreach (var child in Ingredients)
                    foreach (var item in child.RawOutput)
                        yield return item;
            }
        }

        public IEnumerable<Item> Waste
        {
            get
            {
                var input = RawIngredients.ToList();
                var output = RawOutput.ToList();

                var expected = Current.Results.Select((i) => i.Item);

                return output.Except(input).Except(expected);
            }
        }

        public IEnumerable<Item> Input
        {
            get
            {
                var input = RawIngredients.ToList();
                var output = RawOutput.ToList();
                return input.Except(output);
            }
        }

        public RecipeChain()
        {
            Ingredients = new List<RecipeChain>();
        }

        public RecipeChain Copy()
        {
            var result = new RecipeChain();
            result.Current = Current;
            result.Ingredients = Ingredients.ToList();
            return result;
        }

        public static Dictionary<Item, List<RecipeChain>> InitializeChainDictionary(Library library)
        {
            var dict = new Dictionary<Item, List<RecipeChain>>();

            // Add empty trees for resources and uncraftable entities (wood for example)
            foreach(var item in library.Items)
                if(item.IsResource || !item.Recipes.Any())
                    dict.Add(item, new List<RecipeChain>());

            var todo = library.Recipes.ToList();
            while(todo.Count > 0)
            {
                Recipe recipe = null;

                //Select recipe that has only known subtrees
                foreach(var item in library.Items)
                    foreach(var candidate in item.Recipes)
                    {
                        if (!todo.Contains(candidate))
                            continue;

                        bool match = true;
                        foreach(var ingredient in candidate.Ingredients.Select((i) => i.Item))
                        {
                            if (dict.ContainsKey(ingredient))
                                continue;

                            match = false;
                            break;
                        }

                        if (!match)
                            continue;

                        recipe = candidate;
                        break;
                    }

                if (recipe == null)
                    throw new InvalidOperationException("Unreachable recipes found!");

                todo.Remove(recipe);

                List<List<RecipeChain>> ingredients = new List<List<RecipeChain>>();
                foreach(var ingredient in recipe.Ingredients)
                    ingredients.Add(dict[ingredient.Item]);

                List<RecipeChain> itemAdd = new List<RecipeChain>();

                foreach(var perm in FindPermutations(ingredients))
                {
                    var res = new RecipeChain();
                    res.Current = recipe;
                    foreach (var chain in perm)
                        res.Ingredients.Add(chain);

                    itemAdd.Add(res);
                }

                foreach(var result in recipe.Results.Select((r) => r.Item))
                {
                    if(!dict.ContainsKey(result))
                        dict.Add(result, new List<RecipeChain>());

                    dict[result].AddRange(itemAdd);
                }
            }

            return dict;
        }

        private static IEnumerable<List<T>> FindPermutations<T>(List<List<T>> sets)
        {
            sets = sets.Where((s) => s.Count > 0).ToList();
            if (sets.Count == 0)
            {
                yield return new List<T>();
                yield break;
            }

            int[] positions = new int[sets.Count];

            while(true)
            {
                var result = new List<T>();

                for (int i = 0; i < positions.Length; i++)
                    result.Add(sets[i][positions[i]]);

                yield return result;

                var done = true;
                for(int i = 0; i < positions.Length; i++)
                {
                    positions[i]++;
                    if (positions[i] >= sets[i].Count)
                    {
                        positions[i] = 0;
                        continue;
                    }

                    done = false;
                }

                if (done)
                    yield break;
            }
        }
    }
}
