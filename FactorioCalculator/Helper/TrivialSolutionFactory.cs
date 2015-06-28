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
    static class TrivialSolutionFactory
    {
        public static List<IStep> Generate(Library library, Item item, double amount)
        {
            

            return null;
        }
        /*
        public static IEnumerable<IStep> GenerateProductionLayer(Library library, Item item, double amount)
        {
            List<IStep> result = new List<IStep>();
            var chain = FindBestChain(library, item);
            var top = new Step(null, new IStep[] { });

            Queue<Tuple<Item, RecipeGraph>> todo = new Queue<Tuple<Item, RecipeGraph>>();
            Dictionary<Item, ItemAmount> balance = new Dictionary<Item, ItemAmount>();
            Stack<Tuple<RecipeGraph, double>> nodes = new Stack<Tuple<RecipeGraph, double>>();
            todo.Enqueue(new Tuple<Item, RecipeGraph>(item, chain));
            foreach (var i in chain.RawIngredients)
                if(!balance.ContainsKey(i))
                    balance.Add(i, new ItemAmount(i, 0));
            foreach (var i in chain.Waste)
                if (!balance.ContainsKey(i))
                    balance.Add(i, new ItemAmount(i, 0));

            foreach (var i in chain.Current.Results)
                if (!balance.ContainsKey(i.Item))
                    balance.Add(i.Item, new ItemAmount(i.Item, 0));

            balance[item] -= amount;

            while (todo.Count > 0)
            {
                var queueItem = todo.Dequeue();
                var currentItem = queueItem.Item1;

                var currentChain = queueItem.Item2;
                var target = balance[currentItem];

                if (target.Amount > 0)
                    continue;

                foreach (var input in currentChain.Current.Ingredients)
                    if (currentChain.Ingredients.Count > 0)
                    {
                        var from = currentChain.Ingredients.Where((c) => c.Current.Results.Where((res) => res.Item == input.Item).Any()).Take(1).ToList();
                        if(from.Count > 0)
                            todo.Enqueue(new Tuple<Item, RecipeGraph>(input.Item, from[0]));
                    }

                var iterations = -target.Amount / currentChain.Current.Results.Where((r) => r.Item == currentItem).First().Amount;
                foreach (var output in currentChain.Current.Results)
                    balance[output.Item] += output * iterations;
                foreach (var input in currentChain.Current.Ingredients)
                    balance[input.Item] -= input * iterations;

                nodes.Push(new Tuple<RecipeGraph, double>(currentChain, iterations));
            }

            List<ItemAmount> waste = new List<ItemAmount>();
            List<ItemAmount> inputs = new List<ItemAmount>();

            foreach (var key in balance.Keys)
            {
                if (balance[key].Amount == 0)
                    continue;
                if (balance[key].Amount > 0)
                    waste.Add(balance[key]);
                else
                    inputs.Add(-balance[key]);
            }

            Dictionary<Item, List<IStep>> steps = new Dictionary<Item, List<IStep>>();

            foreach (var input in inputs)
            {
                var step = new SourceStep(top, new IStep[] { }, input);
                steps.Add(input.Item, new List<IStep>() { step });
                result.Add(step);
            }

            while (nodes.Count != 0)
            {
                var node = nodes.Pop();
                var nodeChain = node.Item1;
                var nodeAmount = node.Item2;
                var previous = nodeChain.Current.Ingredients.SelectMany((i) => steps[i.Item]);
                var step = new TransformStep(top, previous, nodeChain.Current, nodeAmount);
                foreach (var output in nodeChain.Current.Results)
                {
                    if (!steps.ContainsKey(output.Item))
                        steps.Add(output.Item, new List<IStep>());
                    steps[output.Item].Add(step);
                }
                result.Add(step);
            }

            foreach (var type in waste)
                result.Add(new SinkStep(top, steps[type.Item], type));

            result.Add(new SinkStep(top, steps[item], new ItemAmount(item, amount)));

            return result;
        }
        
        public static RecipeGraph FindBestChain(Library library, Item item)
        {
            return library.RecipeChains[item].OrderBy((c) => c.Waste.Count()).First();
        */
    }
}
