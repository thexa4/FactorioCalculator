using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioCalculator.Forms
{
    public partial class LocationSelector : Form
    {
        private Library _library;
        private bool _exiting = true;
        private bool _starting = true;
        private object _lockObject = new object();

        public LocationSelector()
        {
            InitializeComponent();

            locationInput.Text = Settings.Default.FactorioLocation;
            _starting = false;

            UpdateLocation();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.ProgramFiles;
            dialog.SelectedPath = locationInput.Text;

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                locationInput.Text = dialog.SelectedPath;
                UpdateLocation();
            }
        }

        private async Task UpdateLocation()
        {
            if (_starting)
                return;

            continueButton.Enabled = false;

            var versioninfo = new FileInfo(Path.Combine(locationInput.Text, "data", "changelog.txt"));
            if (!versioninfo.Exists)
            {
                statusLabel.Text = "Factorio not found at specified location";
                return;
            }

            browseButton.Enabled = false;
            locationInput.Enabled = false;
            
            var file = versioninfo.OpenText();
            file.ReadLine();
            var version = file.ReadLine();

            statusLabel.Text = string.Format("Found Factorio: [{0}], loading...", version);
            ModImporter importer = new ModImporter(locationInput.Text);
            await Task.Run(() =>
            {
                importer.Load();
            });
            _library = importer.Library;

            statusLabel.Text = string.Format("Found Factorio: [{0}], loaded.", version);
            continueButton.Enabled = true;
            browseButton.Enabled = true;
            locationInput.Enabled = true;
            continueButton.Focus();
        }

        private void locationInput_TextChanged(object sender, EventArgs e)
        {
            UpdateLocation();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (_library != null)
            {
                if (electricityCheckbox.CheckState == CheckState.Checked)
                    _library.AddPowerPseudoItems();

                var next = new RecipeBuilder(_library);
                next.Show();
                _exiting = false;
                this.Close();
            }
        }

        private void LocationSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(_exiting)
                Application.Exit();
        }
    }
}
