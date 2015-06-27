using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    public static class StepExtensions
    {
        public static void PrintDot(this IEnumerable<IStep> list)
        {
            Console.WriteLine("digraph production {");
            foreach (var step in list)
                foreach (var prev in step.Previous)
                    Console.WriteLine(String.Format("\"{0}\" -> \"{1}\";", prev, step));
            Console.WriteLine("}");
        }
    }
}
