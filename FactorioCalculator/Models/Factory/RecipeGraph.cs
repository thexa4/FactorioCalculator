using FactorioCalculator.Models.Factory;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    class RecipeGraph : IStep
    {
        public IStep Parent { get; set; }

        /// <summary>
        /// The step(s) that precede this one
        /// </summary>
        public HashSet<IStep> Previous { get; set; }

        public IEnumerable<Item> Waste { get { return _waste.Select((s) => s.Amount.Item);  } }
        private List<SinkStep> _waste = new List<SinkStep>();
        public IEnumerable<Item> Inputs { get { return _inputs.Select((s) => s.Amount.Item); } }
        private List<SourceStep> _inputs = new List<SourceStep>();
        public IEnumerable<Item> Outputs { get { return _outputs.Select((s) => s.Amount.Item); } }
        private List<SinkStep> _outputs = new List<SinkStep>();
        public IEnumerable<FlowStep> Resources { get { return _resources; } }
        private List<FlowStep> _resources = new List<FlowStep>();
        public IEnumerable<TransformStep> Transformations { get { return _transformations; } }
        private List<TransformStep> _transformations = new List<TransformStep>();

        public RecipeGraph(IEnumerable<SinkStep> waste, IEnumerable<SourceStep> inputs, IEnumerable<SinkStep> outputs, IEnumerable<FlowStep> resources, IEnumerable<TransformStep> transformations)
        {
            _waste.AddRange(waste);
            _inputs.AddRange(inputs);
            _outputs.AddRange(outputs);
            _resources.AddRange(resources);
            _transformations.AddRange(transformations);            
        }

        private class Node
        {
            public double Flow { get; set; }
        }
        private class Arc
        {
            public Node From { get; set; }
            public Node To { get; set; }
            public double Cost { get; set; }
            public int Variable { get; set; }
            public Arc(Node from, Node to)
            {
                From = from;
                To = to;
            }
        }
        public static RecipeGraph FromLibrary(Library library, IEnumerable<Item> inputs, IEnumerable<Item> outputs, Func<Recipe, double> costFunction, double wasteCost)
        {
            var solver = new SimplexSolver();
            var nodes = new List<Node>();
            var arcs = new List<Arc>();
            var itemNodes = new Dictionary<Item, Node>();
            var productionIn = new Dictionary<Item, Node>();
            var productionOut = new Dictionary<Item, Node>();
            var waste = new Node();

            //Todo: add input discard cost and flow

            foreach (var item in library.Items)
            {
                var node = new Node();
                nodes.Add(node);
                var wasteArc = new Arc(node, waste) { Cost = wasteCost };
                arcs.Add(wasteArc);
                itemNodes.Add(item, node);
            }
            
            foreach(var recipe in library.Recipes)
            {
                var inNode = new Node();
                nodes.Add(inNode);
                var outNode = new Node();
                nodes.Add(outNode);
                var cost = costFunction(recipe);
                var productionArc = new Arc(inNode, outNode) { Cost = cost };
                arcs.Add(productionArc);

                foreach (var input in recipe.Ingredients)
                {
                    var arc = new Arc(itemNodes[input.Item], inNode);
                    arcs.Add(arc);
                }
                foreach (var output in recipe.Results)
                {
                    var arc = new Arc(itemNodes[output.Item], outNode);
                    arcs.Add(arc);
                }
            }

            var outFlow = inputs.Count(); // (inputsize * outputsize) / outputsize
            var inFlow = outputs.Count();

            foreach (var input in inputs)
                itemNodes[input].Flow = inFlow;
            foreach (var output in outputs)
            {
                var node = itemNodes[output];
                node.Flow = -outFlow;
                var wasteArc = new Arc(waste, node);
                arcs.Add(wasteArc);
            }

            foreach (var arc in arcs)
            {
                int id;
                solver.AddVariable(arc, out id);
                arc.Variable = id;
                solver.SetBounds(id, 0, Rational.PositiveInfinity);
            }
            int costRow;
            solver.AddRow(new Object(), out costRow);
            foreach (var arc in arcs)
                solver.SetCoefficient(costRow, arc.Variable, arc.Cost);
            
            return null;
        }
    }
}
