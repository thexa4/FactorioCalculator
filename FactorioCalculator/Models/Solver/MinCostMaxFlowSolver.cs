using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Solver
{
    class MinCostMaxFlowSolver
    {
        public static double Solve(IEnumerable<Node> nodes, IEnumerable<Arc> arcs, bool debug = false)
        {
            var solver = new SimplexSolver();
            //Add variable for every edge
            foreach (var arc in arcs)
            {
                if (arc.Variable >= 0)
                    continue;

                int id;
                solver.AddVariable(arc, out id);
                arc.Variable = id;
                solver.SetBounds(id, 0, Rational.PositiveInfinity);
                if (arc.ConstantGroup != null)
                    foreach (var equal in arc.ConstantGroup.Keys)
                        equal.Variable = id;
            }

            //Add cost row to solve for
            int costRow;
            solver.AddRow("cost-row", out costRow);
            foreach (var arc in arcs)
                solver.SetCoefficient(costRow, arc.Variable, arc.Cost);

            //Add row per node (require to be 0)
            foreach (var node in nodes)
            {
                int row;
                solver.AddRow(node, out row);
                node.Row = row;

                foreach (var arc in arcs.Where((a) => a.To == node))
                {
                    var current = (double)solver.GetCoefficient(row, arc.Variable);
                    if(arc.ConstantGroup == null)
                        solver.SetCoefficient(row, arc.Variable, current - 1.0 );
                    else
                        solver.SetCoefficient(row, arc.Variable, current - arc.ConstantGroup[arc]);
                }
                foreach (var arc in arcs.Where((a) => a.From == node))
                {
                    var current = (double)solver.GetCoefficient(row, arc.Variable);
                    if (arc.ConstantGroup == null)
                        solver.SetCoefficient(row, arc.Variable, current + 1.0 * arc.Multiplier);
                    else
                        solver.SetCoefficient(row, arc.Variable, current + arc.ConstantGroup[arc] * arc.Multiplier);
                }
                var upper = Math.Max(node.IsVariableFlow ? 0 : node.Flow, node.Flow);
                var lower = Math.Min(node.IsVariableFlow ? 0 : node.Flow, node.Flow);
                solver.SetBounds(row, lower, upper);
            }

            var goal = solver.AddGoal(costRow, 1, true);
            solver.Solve(new SimplexSolverParams());

            if(debug)
                PrintMatrix(solver, nodes, arcs, costRow);

            if (solver.SolutionQuality != Microsoft.SolverFoundation.Services.LinearSolutionQuality.Exact)
                throw new InvalidOperationException("Cannot solve problem");

            foreach (var arc in arcs)
                arc.Flow = (double)solver.GetValue(arc.Variable);

            return (double)solver.GetValue(goal.Index);
        }

        static private void PrintMatrix(SimplexSolver solution, IEnumerable<Node> nodes, IEnumerable<Arc> arcs, int goal)
        {
            foreach (var arc in arcs)
            {
                if (solution.SolutionQuality != LinearSolutionQuality.Exact)
                {
                    Rational upper;
                    Rational lower;
                    solution.GetBounds(arc.Variable, out lower, out upper);
                    if (upper != lower)
                        Console.Write("?");
                    else
                        Console.Write(upper);
                }
                else
                {
                    Console.Write(solution.GetValue(arc.Variable));
                }
                Console.Write("\t");
            }
            Console.WriteLine();
            Console.WriteLine();
            PrintRow(solution, goal, arcs);
            foreach (var node in nodes)
                PrintRow(solution, node.Row, arcs);
        }

        static private void PrintRow(SimplexSolver solution, int vid, IEnumerable<Arc> arcs)
        {
            foreach(var arc in arcs)
                Console.Write(solution.GetCoefficient(vid, arc.Variable) + "\t");
            Console.Write("\t|\t");
            if (solution.SolutionQuality != LinearSolutionQuality.Exact)
            {
                Rational lower;
                Rational upper;
                solution.GetBounds(vid, out lower, out upper);
                if (lower != upper)
                    Console.Write("?");
                else
                    Console.Write(lower);
            }
            else
            {
                Console.Write(solution.GetValue(vid));
            }
            Console.WriteLine();
        }
    }

    public class Node
    {
        public double Flow { get; set; }
        public bool IsVariableFlow { get; set; }
        public int Row { get; set; }
        public Object Tag { get; set; }
        public override string ToString()
        {
            return Tag.ToString();
        }
    }
    public class Arc
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public double Cost { get; set; }
        public int Variable { get; set; }
        public double Multiplier { get; set; }
        public Dictionary<Arc, double> ConstantGroup { get; set; }
        public double Flow { get; set; }
        public Arc(Node from, Node to)
        {
            From = from;
            To = to;
            Variable = -1;
            Multiplier = 1;
        }
    }
}
