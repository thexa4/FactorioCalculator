using System;
using FactorioCalculator.Forms;
using System.Windows.Forms;

namespace FactorioCalculator
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new LocationSelector().Show();
            Application.Run();
        }
    }
}
