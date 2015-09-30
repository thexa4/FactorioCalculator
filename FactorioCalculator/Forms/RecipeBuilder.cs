using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
using FactorioCalculator.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FactorioCalculator.Models.PlaceRoute;

namespace FactorioCalculator.Forms
{
    public partial class RecipeBuilder : Form
    {
        private bool _exiting = true;
        private Library _library;
        private SolutionGenerator _generator;
        private RecipeGraph _recipe;

        public RecipeBuilder(Library library)
        {
            _library = library;

            InitializeComponent();
        }

        private void RecipeBuilder_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_exiting)
                Application.Exit();
        }

        private void RecipeBuilder_Load(object sender, EventArgs e)
        {
            var items = _library.Items.Select((i) => i.Name).OrderBy((i) => i).ToList();

            IngredientNames.DataSource = items;
            OutputNames.DataSource = items;

            IngredientNames.ValueType = typeof(string);
            OutputNames.ValueType = typeof(string);

            UpdateResultView();
        }

        private void ValidateDouble(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var view = sender as DataGridView;
            if (e.ColumnIndex != 1)
                return;

            double result;
            if (!double.TryParse(Convert.ToString(e.FormattedValue), out result))
                view.CurrentCell.ErrorText = "Unable to parse as number";
            else
                view.CurrentCell.ErrorText = null;
        }

        private IEnumerable<Tuple<Item, double>> ParseDataView(DataGridView source)
        {
            var rows = source.Rows.Cast<DataGridViewRow>()
                .Where((i) => !i.IsNewRow);

            if (rows.Where((r) => !string.IsNullOrEmpty(r.Cells[0].ErrorText) || !string.IsNullOrEmpty(r.Cells[1].ErrorText)).Any())
                return null;

            var inputs = rows.Select((r) => new Tuple<string, double>(r.Cells[0].FormattedValue as string, double.Parse(r.Cells[1].FormattedValue as string)));

            var mapped = inputs.Select((t) => new Tuple<Item, double>(_library.Items.Where((i) => i.Name == t.Item1).First(), t.Item2));

            return mapped;
        }

        private void UpdateResultView()
        {
            solveButton.Enabled = false;

            var inputs = ParseDataView(ingredientsView);
            var outputs = ParseDataView(outputView);

            if (inputs == null || outputs == null)
                return;

            var costs = new Dictionary<Item, double>();
            foreach (var item in inputs)
                costs.Add(item.Item1, item.Item2);

            var outputAmounts = outputs.Select((t) => new ItemAmount(t.Item1, t.Item2));

            try
            {

                var graph = RecipeGraph.FromLibrary(_library, inputs.Select((t) => t.Item1), outputAmounts, (i) => costs[i]);

                if (expandCheckbox.CheckState == CheckState.Checked)
                    graph = TrivialSolutionFactory.CreateFactory(graph);

                resultView.Text = string.Join("\r\n", graph.Children.AsDot());
                _recipe = graph;

                solveButton.Enabled = true;
            }
            catch (Exception e)
            {
                resultView.Text = e.Message;
            }
        }

        private void OnDataUpdate(object sender, DataGridViewCellEventArgs e)
        {
            UpdateResultView();
        }

        private void CheckboxChanged(object sender, EventArgs e)
        {
            UpdateResultView();
            solveButton.Enabled = expandCheckbox.CheckState == CheckState.Checked;
        }

        private async void solveButton_Click(object sender, EventArgs e)
        {
            ingredientsView.Enabled = false;
            outputView.Enabled = false;
            solveButton.Enabled = false;
            expandCheckbox.Enabled = false;
            try
            {
                var grader = new SolutionGrader();

                var pipe = _library.Buildings.Where((b) => b.Name == "pipe").First();
                var pipeToGround = _library.Buildings.Where((b) => b.Name == "pipe-to-ground").First();

                var router = new FluidRouter(pipeToGround, pipe, grader);

                var belt = _library.Buildings.Where((b) => b.Name == "basic-transport-belt").First();
                var beltGroundNormal = _library.Buildings.Where((b) => b.Name == "basic-transport-belt-to-ground").First();
                var beltGroundFast = _library.Buildings.Where((b) => b.Name == "fast-transport-belt-to-ground").First();
                var beltGroundExpress = _library.Buildings.Where((b) => b.Name == "express-transport-belt-to-ground").First();
                var fastInserter = _library.Buildings.Where((b) => b.Name == "fast-inserter").First();
                var inserter = _library.Buildings.Where((b) => b.Name == "basic-inserter").First();
                var longInserter = _library.Buildings.Where((b) => b.Name == "long-handed-inserter").First();
                var splitter = _library.Buildings.Where((b) => b.Name == "basic-splitter").First();

                var solid = new SolidRouter(belt, beltGroundNormal, beltGroundFast, beltGroundExpress, inserter, longInserter, fastInserter, splitter, grader);

                _generator = new SolutionGenerator(_recipe);
                _generator.SolidRouter = solid;
                _generator.FluidRouter = router;
            }
            catch (Exception ex)
            {
                resultView.Text = ex.Message;
            }

            ComputationLoop();
            
            while (true)
            {
                await Task.Delay(1000);
                bestCostLabel.Text = "Cost: " + _generator.LowestCost.ToString();
                if (_generator.BestState != null)
                    solutionImage.Image = _generator.BestState.Draw();
                if (_generator.Preview != null)
                    previewImage.Image = _generator.Preview.Draw();
            }
        }

        public Task ComputationLoop()
        {
            return Task.Factory.StartNew(() =>
            {
                _generator.Initialize();

                while (true)
                {
                    _generator.Step();
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
