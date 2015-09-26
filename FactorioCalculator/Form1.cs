using FactorioCalculator.Models.PlaceRoute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FactorioCalculator.Helper;

namespace FactorioCalculator
{
    public partial class Form1 : Form
    {
        private SolutionGenerator _generator;
        public Form1(SolutionGenerator generator)
        {
            InitializeComponent();
            _generator = generator;

            Poll();
        }

        private async Task Poll()
        {
            while (true)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        _generator.Step();
                    });

                    pictureBox1.Image = _generator.BestState.Draw();
                    Text = _generator.LowestCost.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
