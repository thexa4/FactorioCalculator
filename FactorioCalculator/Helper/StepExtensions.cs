using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    public static class StepExtensions
    {
        public static IEnumerable<string> AsDot(this IEnumerable<IStep> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            yield return "digraph production {";
            foreach (var step in list)
                foreach (var prev in step.Previous)
                    yield return String.Format(CultureInfo.InvariantCulture, "\"{0}\" -> \"{1}\";", prev, step);
            yield return "}";
        }

        public static void PrintDot(this IEnumerable<IStep> list)
        {
            foreach (var line in list.AsDot())
                Console.WriteLine(line);
        }
    }
}
