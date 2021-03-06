﻿using FactorioCalculator.Models.Factory;
using FactorioCalculator.Helper;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Services;
using System.Globalization;

namespace FactorioCalculator.Models.Factory
{
    public class RecipeGraph : IStep
    {
        public IStep Parent { get; set; }

        /// <summary>
        /// The step(s) that precede this one
        /// </summary>
        public HashSet<IStep> Previous { get; private set; }

        public IEnumerable<Item> Waste { get { return _waste.Select((s) => s.Item.Item);  } }
        public IEnumerable<SinkStep> WasteNodes { get { return _waste; } }
        private List<SinkStep> _waste = new List<SinkStep>();
        public IEnumerable<Item> Inputs { get { return _inputs.Select((s) => s.Item.Item); } }
        public IEnumerable<SourceStep> InputNodes { get { return _inputs; } }
        private List<SourceStep> _inputs = new List<SourceStep>();
        public IEnumerable<Item> Outputs { get { return _outputs.Select((s) => s.Item.Item); } }
        public IEnumerable<SinkStep> OutputNodes { get { return _outputs; } }
        private List<SinkStep> _outputs = new List<SinkStep>();
        public IEnumerable<FlowStep> Resources { get { return _resources; } }
        private List<FlowStep> _resources = new List<FlowStep>();
        public IEnumerable<TransformStep> Transformations { get { return _transformations; } }
        private List<TransformStep> _transformations = new List<TransformStep>();

        public IEnumerable<IStep> Children
        {
            get
            {
                return _waste.Cast<IStep>().Concat(_inputs).Concat(_outputs).Concat(_resources).Concat(_transformations);
            }
        }

        public RecipeGraph(IEnumerable<SinkStep> waste, IEnumerable<SourceStep> inputs, IEnumerable<SinkStep> outputs, IEnumerable<FlowStep> resources, IEnumerable<TransformStep> transformations)
        {
            _waste.AddRange(waste);
            _inputs.AddRange(inputs);
            _outputs.AddRange(outputs);
            _resources.AddRange(resources);
            _transformations.AddRange(transformations);
            foreach (var step in _waste.Cast<IStep>().Concat(_inputs).Concat(_outputs).Concat(_resources).Concat(_transformations))
                step.Parent = this;
        }
        
        public static RecipeGraph FromLibrary(Library library, IEnumerable<Item> inputs, IEnumerable<ItemAmount> outputs, Func<Item, double> costFunction)
        {
            if (library == null)
                throw new ArgumentNullException("library");
            if (inputs == null)
                throw new ArgumentNullException("inputs");
            if (outputs == null)
                throw new ArgumentNullException("outputs");
            if (costFunction == null)
                throw new ArgumentNullException("costFunction");

            var solver = new SimplexSolver();
            var itemRows = new Dictionary<Item, int>();
            var recipeVars = new Dictionary<Recipe, int>();
            var wasteGoals = new Dictionary<Item, ILinearGoal>();

            foreach (var item in library.Items)
            {
                int id;
                solver.AddRow(item, out id);
                itemRows.Add(item, id);
                solver.SetBounds(id, 0, Rational.PositiveInfinity);
                if(!inputs.Contains(item))
                    wasteGoals.Add(item, solver.AddGoal(id, 1, true));
            }

            // Bound output to requested values
            foreach (var itemAmount in outputs)
            {
                var item = itemAmount.Item;
                var amount = itemAmount.Amount;
                solver.SetBounds(itemRows[item], amount, amount);
            }

            foreach (var recipe in library.Recipes)
            {
                int id;
                solver.AddVariable(recipe, out id);
                recipeVars.Add(recipe, id);
                solver.SetBounds(id, 0, Rational.PositiveInfinity);

                foreach (var input in recipe.Ingredients)
                    solver.SetCoefficient(itemRows[input.Item], id, -input.Amount);
                foreach (var output in recipe.Results)
                    solver.SetCoefficient(itemRows[output.Item], id, output.Amount);
            }

            // Add input minimize goals
            foreach (var item in inputs)
            {
                var row = itemRows[item];
                solver.SetBounds(row, Rational.NegativeInfinity, 0);
                solver.AddGoal(row, (int)(10000 * costFunction(item)), false);
            }

            solver.Solve(new SimplexSolverParams());

            if (solver.SolutionQuality != Microsoft.SolverFoundation.Services.LinearSolutionQuality.Exact)
                throw new InvalidOperationException("Cannot solve problem");

            List<Tuple<Recipe, double>> usedRecipes = new List<Tuple<Recipe, double>>();

            foreach (var recipe in library.Recipes)
            {
                var value = (double)solver.GetValue(recipeVars[recipe]);
                if (value > 0)
                    usedRecipes.Add(new Tuple<Recipe, double>(recipe, value));
            }

            var sortedRecipes = usedRecipes.SortTopological((a, b) => a.Item1.Results.Select((i) => i.Item).Intersect(b.Item1.Ingredients.Select((i) => i.Item)).Any(), true);
            List<SourceStep> inputSteps = new List<SourceStep>();
            List<SinkStep> outputSteps = new List<SinkStep>();
            List<SinkStep> wasteSteps = new List<SinkStep>();
            Dictionary<Item, FlowStep> itemSteps = new Dictionary<Item, FlowStep>();
            List<FlowStep> flowSteps = new List<FlowStep>();
            List<TransformStep> transformSteps = new List<TransformStep>();

            foreach (var item in library.Items)
            {
                var value = (double)solver.GetValue(itemRows[item]);
                if (value > 0)
                {
                    var sink = new SinkStep(new ItemAmount(item, value / 2));
                    if (outputs.Select((o) => o.Item).Contains(item))
                        outputSteps.Add(sink);
                    else
                        wasteSteps.Add(sink);
                    itemSteps.Add(item, sink);
                }
                else if(value < 0)
                {
                    var source = new SourceStep(new ItemAmount(item, -value));
                    inputSteps.Add(source);
                    itemSteps.Add(item, source);
                }
            }

            foreach (var recipe in sortedRecipes)
            {
                foreach (var result in recipe.Item1.Results)
                {
                    var item = result.Item;
                    if (!itemSteps.ContainsKey(item))
                    {
                        var flowstep = new FlowStep(new ItemAmount(item, 0));
                        itemSteps.Add(item, flowstep);
                        flowSteps.Add(flowstep);
                    }
                }
            }

            foreach (var recipe in sortedRecipes)
            {
                var previous = recipe.Item1.Ingredients.Select((i) => i.Item);
                var step = new TransformStep(recipe.Item1, recipe.Item2);
                foreach (var item in previous)
                {
                    var prev = itemSteps[item];
                    step.Previous.Add(prev);
                }
                
                foreach (var amount in recipe.Item1.Results)
                {
                    var item = amount.Item;
                    
                    itemSteps[item].Previous.Add(step);
                    itemSteps[item].Item += amount * recipe.Item2;
                }
                transformSteps.Add(step);
            }

            return new RecipeGraph(wasteSteps, inputSteps, outputSteps, flowSteps, transformSteps);
        }
    }
}
