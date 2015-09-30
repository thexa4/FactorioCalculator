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
using FactorioCalculator.Models;

namespace FactorioCalculator
{
    public partial class Form1 : Form
    {
        private SolutionGenerator _generator;
        public Form1(SolutionGenerator generator)
        {
            InitializeComponent();
            _generator = generator;

            //_generator.Step();

            Poll();
            Display();
        }

        private async Task Display()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (_generator.BestState.Size != Vector2.Zero)
                    pictureBox1.Image = _generator.BestState.Draw();
                if (_generator.Preview.Size != Vector2.Zero)
                    pictureBox2.Image = _generator.Preview.Draw();
            }
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

                    Text = _generator.LowestCost.ToString();
                    Console.WriteLine(_generator.Temperature);
                }
                catch (Exception e)
                {
                    _generator.BestParameters = _generator.BestParameters.Modify(0.2);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
