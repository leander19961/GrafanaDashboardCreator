using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Net;
using GrafanaDashboardCreator.Parser;

namespace GrafanaDashboardCreator.View.GrafanaPOSTView
{
    /// <summary>
    /// Interaktionslogik für SelectGrafanaFolderPopUp.xaml
    /// </summary>
    public partial class SelectGrafanaFolderPopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool _buttonPressed = false;

        public bool ButtonPressed { get { return _buttonPressed; } }

        public Folder SelectedFolder { get { return FolderSelectBox.SelectedItem as Folder; } }

        public SelectGrafanaFolderPopUp()
        {
            InitializeComponent();

            try
            {
                string foldersJSON = RESTAPI.GETFoldersFromGrafana();

                if (foldersJSON == null || foldersJSON == "")
                {
                    MessageBox.Show("GET request to Grafana failed!", "Error!");
                    return;
                }

                FolderSelectBox.ItemsSource = JSONParser.GetFolders(foldersJSON);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _buttonPressed = true;
            Close();
        }
    }
}
