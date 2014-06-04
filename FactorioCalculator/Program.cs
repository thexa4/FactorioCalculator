using FactorioCalculator.Importer;
using FactorioCalculator.Models;
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
            ModImporter a = new ModImporter(@"C:\Users\Max\Documents\Factorio_0.9.8.9400", "base");
            a.Load();
            

            Console.WriteLine(a.Library);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
