using GrafanaDashboardCreator.Model;
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

namespace GrafanaDashboardCreator.View.PopUps
{
    /// <summary>
    /// Interaktionslogik für AddSpecialDatasourcePopUp.xaml
    /// </summary>
    public partial class AddSpecialDatasourcePopUp : Window
    {
        ModelService modelService;

        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool _buttonPressed = false;

        public bool ButtonPressed { get { return _buttonPressed; } }

        public string EnteredText { get { return NameTextBox.Text; } }

        public string SelectedResourceIDString { get { return ResourceIDTextBox.Text; } }

        public Node SelectedNode { get { return SelectNodeBox.SelectedItem as Node; } }

        public AddSpecialDatasourcePopUp(ModelService modelService)
        {
            InitializeComponent();
            this.modelService = modelService;
            SelectNodeBox.ItemsSource = modelService.GetNodes();
        }

        private void AddSpecialDataSourceButton_OnCLick(object sender, RoutedEventArgs e)
        {
            _buttonPressed = true;
            Close();
        }

        private void NameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //For accepting "Return" as "confirmation"
            if (e.Key == Key.Return)
            {
                AddSpecialDataSourceButton_OnCLick(sender, e);
            }
        }
    }
}
