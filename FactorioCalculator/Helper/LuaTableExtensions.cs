using FactorioCalculator.Models;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    public static class LuaTableExtensions
    {
        public static bool ContainsKey(this LuaTable table, string key)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            return table.Keys.OfType<string>().Contains(key);
        }
        /// <summary>
        /// Tries to return a Vector2 of the data in the LuaTable.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this LuaTable table) {
            if (table == null)
                throw new ArgumentNullException("table");
            if (table.Keys.Count == 2)
                return new Vector2((double)table[1.0], (double)table[2.0]);
            else
                throw new InvalidOperationException();
        }
    }
}
