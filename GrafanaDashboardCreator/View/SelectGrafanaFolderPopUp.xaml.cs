using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.Net;
using GrafanaDashboardCreator.Parser;
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

namespace GrafanaDashboardCreator.View
{
    /// <summary>
    /// Interaktionslogik für SelectGrafanaFolderPopUp.xaml
    /// </summary>
    public partial class SelectGrafanaFolderPopUp : Window
    {
        private bool _buttonPressed = false;

        public bool ButtonPressed { get { return _buttonPressed; } }

        public Folder SelectedFolder { get { return FolderSelectBox.SelectedItem as Folder; } }

        public SelectGrafanaFolderPopUp()
        {
            InitializeComponent();
            FolderSelectBox.ItemsSource = JSONParser.GetFolders(RESTAPI.GETFoldersFromGrafana());
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            _buttonPressed = true;
            Close();
        }
    }
}
