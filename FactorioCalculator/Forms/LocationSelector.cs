using FactorioCalculator.Importer;
using FactorioCalculator.Models;
using FactorioCalculator.Properties;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactorioCalculator.Forms
{
    public partial class LocationSelector : Form
    {
        private Library _library;

        private static string FactorioPathPath
        {
            get { return Path.Combine(ModImporter.AppDataFolder.FullName, "factorio_path.txt"); }
        }

        public LocationSelector()
        {
            InitializeComponent();

            locationInput.Text = DefaultLocation();
        }

        private static string DefaultLocation()
        {
            if (File.Exists(FactorioPathPath))
            {
                var p = File.ReadAllText(FactorioPathPath);
                if (Directory.Exists(p))
                    return p;
            }

            return Settings.Default.FactorioLocation;
        }

        private async void browseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog {
                SelectedPath = locationInput.Text
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                locationInput.Text = dialog.SelectedPath;
                _library = await LoadLibrary(dialog.SelectedPath);

                File.WriteAllText(FactorioPathPath, dialog.SelectedPath);
            }
        }

        private async Task<Library> LoadLibrary(string path)
        {
            continueButton.Enabled = false;

            var versioninfo = new FileInfo(Path.Combine(path, "data", "changelog.txt"));
            if (!versioninfo.Exists)
            {
                statusLabel.Text = Resources.LocationSelector_LoadLibrary_FactorioNotFound;
                return null;
            }

            browseButton.Enabled = false;
            locationInput.Enabled = false;
            
            var file = versioninfo.OpenText();
            file.ReadLine();
            var version = file.ReadLine();

            statusLabel.Text = string.Format(Resources.LocationSelector_LoadLibrary_FactorioFound_Loading, version);
            var importer = new ModImporter(path);

            await Task.Run((Action)importer.Load);

            statusLabel.Text = string.Format(Resources.LocationSelector_LoadLibrary_FactorioFound_Loaded, version);
            continueButton.Enabled = true;
            browseButton.Enabled = true;
            locationInput.Enabled = true;
            continueButton.Focus();

            return importer.Library;
        }

        private async void continueButton_Click(object sender, EventArgs e)
        {
            if (_library == null)
                _library = await LoadLibrary(locationInput.Text);

            if (_library != null)
            {
                var next = new TreeViewer(_library);
                next.Show();
                Hide();
            }
        }

        private void LocationSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
