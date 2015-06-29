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
    }
}
