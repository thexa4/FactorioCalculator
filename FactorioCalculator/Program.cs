using FactorioCalculator.Helper;
using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Solver;
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
            ModImporter a = new ModImporter(@"C:\Program Files\Factorio", "base");
            a.Load();

            //var assembler = a.Library.Buildings.Where((b) => b.Name == "assembling-machine-1").First();
            //var recipe = a.Library.Recipes.Where((r) => r.Name == "light-oil").First();
            //var step = new ProductionStep(null, new IStep[] { }, recipe, 1, assembler);

            var item = a.Library.Items.Where((i) => i.Name == "speed-module-3").First();
            var copperPlate = a.Library.Items.Where((i) => i.Name == "copper-plate").First();
            var ironPlate = a.Library.Items.Where((i) => i.Name == "iron-plate").First();
            var coal = a.Library.Items.Where((i) => i.Name == "coal").First();
            var oil = a.Library.Items.Where((i) => i.Name == "crude-oil").First();
            var water = a.Library.Items.Where((i) => i.Name == "water").First();
            var electronicCircuit = a.Library.Items.Where((i) => i.Name == "electronic-circuit").First();
            var science2 = a.Library.Items.Where((i) => i.Name == "science-pack-2").First();
            var science3 = a.Library.Items.Where((i) => i.Name == "science-pack-3").First();
            var advancedCircuit = a.Library.Items.Where((i) => i.Name == "advanced-circuit").First();
            var alienArtifact = a.Library.Items.Where((i) => i.Name == "alien-artifact").First();
            var stone = a.Library.Items.Where((i) => i.Name == "stone").First();
            
            var graph = RecipeGraph.FromLibrary(a.Library,
                new Item[] { copperPlate, ironPlate, coal, oil, alienArtifact, stone, water },
                new ItemAmount[] { new ItemAmount(science3, 1) },
                (r) => 1, 10000);

            graph.PrintDotFormat();
            //TrivialSolutionFactory.GenerateProductionLayer(a.Library, item, 2).PrintDot();
            
            //Console.WriteLine(a.Library.RecipeChains.Last().Value.First().Waste);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
