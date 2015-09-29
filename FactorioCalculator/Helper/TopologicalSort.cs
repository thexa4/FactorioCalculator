using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    public static class TopologicalSort
    {
        /// <summary>
        /// Does a topological sort in O(n^2)
        /// </summary>
        /// <typeparam name="T">The type of enumerator</typeparam>
        /// <param name="inputs">The inputs to sort</param>
        /// <param name="linksTo">A predicate wether element 1 links to element 2</param>
        /// <returns>A topologically sorted representation</returns>
        public static IEnumerable<T> SortTopological<T>(this IEnumerable<T> inputs, Func<T, T, bool> linksTo, bool allowCycles)
        {
            Dictionary<T, int> links = new Dictionary<T,int>();
            Dictionary<Tuple<T, T>, bool> linkCache = new Dictionary<Tuple<T, T>, bool>();

            foreach (var input in inputs)
            {
                links.Add(input, 0);
                foreach (var target in inputs)
                {
                    var link = linksTo(target, input);
                    linkCache.Add(new Tuple<T, T>(target, input), link);
                    if (link)
                        links[input]++;
                }
            }

            while (links.Count != 0)
            {
                bool progress = false;

                List<T> removals = new List<T>();
                foreach (var key in links.Keys)
                    if (links[key] == 0)
                    {
                        progress = true;
                        removals.Add(key);
                        yield return key;
                    }
                foreach (var key in removals)
                {
                    links.Remove(key);
                    foreach (var target in inputs)
                        if (linkCache[new Tuple<T,T>(key, target)])
                            if (links.ContainsKey(target))
                                links[target]--;
                }

                if (!progress)
                {
                    if (allowCycles)
                    {
                        var lowestKey = links.Keys.OrderBy((k) => links[k]).First();
                        links[lowestKey] = 0;
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException("Malformed input, graph contains cycles.");
                    }
                }
            }
        }
    }
}
