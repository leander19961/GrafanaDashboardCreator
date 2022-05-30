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
    /// Interaktionslogik für SetTemplatePopUp.xaml
    /// </summary>
    public partial class SetTemplatePopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public Template SelectedTemplate { get { return TemplateSelectBox.SelectedItem as Template; } }

        public SetTemplatePopUp(ObservableCollection<Template> templates)
        {
            InitializeComponent();
            TemplateSelectBox.ItemsSource = templates;
        }

        private void RemoveDashboard_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }
    }
}
