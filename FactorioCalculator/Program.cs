using FactorioCalculator.Helper;
using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using FactorioCalculator.Models.Factory.Physical;
using FactorioCalculator.Models.PlaceRoute;
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
            var science1 = a.Library.Items.Where((i) => i.Name == "science-pack-1").First();
            var science2 = a.Library.Items.Where((i) => i.Name == "science-pack-2").First();
            var science3 = a.Library.Items.Where((i) => i.Name == "science-pack-3").First();
            var speed3 = a.Library.Items.Where((i) => i.Name == "speed-module-3").First();
            var productivity3 = a.Library.Items.Where((i) => i.Name == "productivity-module-3").First();
            var efficiency3 = a.Library.Items.Where((i) => i.Name == "effectivity-module-3").First();
            var advancedCircuit = a.Library.Items.Where((i) => i.Name == "advanced-circuit").First();
            var alienArtifact = a.Library.Items.Where((i) => i.Name == "alien-artifact").First();
            var stone = a.Library.Items.Where((i) => i.Name == "stone").First();

            var results = new Dictionary<String, double> {
                {"science-pack-1", 0.3}, 
                {"science-pack-2", 0.3}, 
                {"science-pack-3", 0.3}, 
                {"alien-science-pack", 0.3}, 
                {"advanced-circuit", 1}, 
                {"lab", 1.0 / 60 / 15}, 
                {"assembling-machine-3", 1.0 / 60 / 15}, 
                {"electric-furnace", 1.0 / 60 / 15}, 
                {"basic-transport-belt-to-ground", 1.0 / 60 / 15}, 
                {"basic-splitter", 1.0 / 60 / 15}, 
                {"pipe", 1.0 / 60 / 15}, 
                {"pipe-to-ground", 1.0 / 60 / 15}, 
                {"chemical-plant", 1.0 / 60 / 15}, 
                {"oil-refinery", 1.0 / 60 / 15}, 
                {"long-handed-inserter", 1.0 / 60 / 15}, 
                {"basic-transport-belt", 0.25}, 
                {"fast-inserter", 1.0 / 60 / 15}, 
                {"basic-inserter", 1.0 / 60 / 15}, 
                {"medium-electric-pole", 1.0 / 60 / 15}, 
                {"steel-chest", 1.0 / 60 / 15}, 
                {"basic-mining-drill", 1.0 / 60 / 15},
                //{"advanced-circuit", 1.16388888888889},
            };

            
            /*var graph = RecipeGraph.FromLibrary(a.Library,
                new Item[] { copperPlate, ironPlate, coal, oil, alienArtifact, stone, water },
                results.Select((s) => new ItemAmount(a.Library.Items.Where((i) => i.Name == s.Key.ToLowerInvariant()).First(), s.Value)),
                (r) => 1);

             */
            //graph.Children.PrintDot();

            //var result = TrivialSolutionFactory.CreateFactory(graph);
            //result.Children.PrintDot();

            //TrivialSolutionFactory.GenerateProductionLayer(a.Library, item, 2).PrintDot();

            //Console.WriteLine(a.Library.RecipeChains.Last().Value.First().Waste);


            //var solution = new TrivialSolutionFactory(a.Library, graph.Children);

            var refinery = a.Library.Buildings.Where((b) => b.Name == "oil-refinery").First();
            var chemical = a.Library.Buildings.Where((b) => b.Name == "chemical-plant").First();
            var pipe = a.Library.Buildings.Where((b) => b.Name == "pipe").First();
            var pipeToGround = a.Library.Buildings.Where((b) => b.Name == "pipe-to-ground").First();

            var space = new SearchSpace(new Vector2(16, 16));
            var refBuilding = new ProductionBuilding(refinery.Recipes.First(), 0.1, refinery, Vector2.One, BuildingRotation.East);
            var chemBuilding = new ProductionBuilding(chemical.Recipes.First(), 0.1, chemical, new Vector2(6, 11), BuildingRotation.North);
            space = space.AddComponent(refBuilding);
            space = space.AddComponent(chemBuilding);

            var grader = new SolutionGrader();

            var router = new FluidRouter();
            router.Grader = grader;
            router.Pipe = pipe;
            router.PipeToGround = pipeToGround;

            var solid = new SolidRouter();
            solid.Grader = grader;
            solid.Belt = a.Library.Buildings.Where((b) => b.Name == "basic-transport-belt").First();
            solid.BeltGroundNormal = a.Library.Buildings.Where((b) => b.Name == "basic-transport-belt-to-ground").First();
            solid.BeltGroundFast = a.Library.Buildings.Where((b) => b.Name == "fast-transport-belt-to-ground").First();
            solid.BeltGroundExpress = a.Library.Buildings.Where((b) => b.Name == "express-transport-belt-to-ground").First();
            solid.FastInserter = a.Library.Buildings.Where((b) => b.Name == "fast-inserter").First();
            solid.Inserter = a.Library.Buildings.Where((b) => b.Name == "basic-inserter").First();
            solid.LongInserter = a.Library.Buildings.Where((b) => b.Name == "long-handed-inserter").First();

            space = router.Route(new ItemAmount(water, 1), space, new Vector2(2, 3), BuildingRotation.West, new List<Vector2>() { new Vector2(8, 12) });
            space = router.Route(new ItemAmount(oil, 1), space, new Vector2(4, 3), BuildingRotation.East, new List<Vector2>() { new Vector2(7, 13) });
            List<Vector2> startPoints = new List<Vector2>();
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    startPoints.Add(refBuilding.Position + new Vector2(x, y));

            List<Vector2> destinations = new List<Vector2>();
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    destinations.Add(chemBuilding.Position + new Vector2(x, y));

            space = solid.Route(new ItemAmount(coal, 0.1), space, startPoints, destinations);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new Form1(space.Draw()));
        }
    }
}
