using GrafanaDashboardCreator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GrafanaDashboardCreator.View.PopUps
{
    /// <summary>
    /// Interaktionslogik für SelectNodePopUp.xaml
    /// </summary>
    public partial class SelectNodePopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public Node SelectedNode { get { return NodeSlectBox.SelectedItem as Node; } }

        public SelectNodePopUp(ObservableCollection<Node> nodes)
        {
            InitializeComponent();
            NodeSlectBox.ItemsSource = nodes;
        }

        private void RemoveDashboard_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }
    }
}
