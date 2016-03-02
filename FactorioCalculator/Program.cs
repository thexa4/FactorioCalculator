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

            using (var s = new LocationSelector())
            {
                s.Show();
                Application.Run();
            }
        }
    }
}
