using FactorioCalculator.Helper;
using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioCalculator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ModImporter a = new ModImporter(@"D:\Program Files\Factorio", "base");
            a.Load();

            //var assembler = a.Library.Buildings.Where((b) => b.Name == "assembling-machine-1").First();
            //var recipe = a.Library.Recipes.Where((r) => r.Name == "light-oil").First();
            //var step = new ProductionStep(null, new IStep[] { }, recipe, 1, assembler);

            var item = a.Library.Items.Where((i) => i.Name == "speed-module-3").First();
            RecipeGraph.FromLibrary(a.Library, new Item[] { a.Library.Items.Where((i) => i.Name == "copper-plate").First() }, new Item[] { a.Library.Items.Where((i) => i.Name == "copper-cable").First() }, (r) => r.Ingredients.Select((i) => i.Amount).Sum() / r.Results.Select((i) => i.Amount).Sum(), 10000);
            //TrivialSolutionFactory.GenerateProductionLayer(a.Library, item, 2).PrintDot();

            //Console.WriteLine(a.Library.RecipeChains.Last().Value.First().Waste);

            List<IStep> tempInput = new List<IStep>() { new TransformStep(null, new IStep[]{}, a.Library.Recipes.Where((i) => i.Name == "copper-cable").First(), 20.0) };
            var solution = new TrivialSolutionFactory(a.Library, tempInput);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
