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
    /// Interaktionslogik für GetNewTemplatePopUp.xaml
    /// </summary>
    public partial class GetNewTemplatePopUp : Window
    {
        public string TemplateName { get { return TemplateNameTextBox.Text; } }
        public string JSONTitle { get { return JSONTitleTextBox.Text; } }
        public string PathToJSON { get { return PathToJSONTextBox.Text; } }
        public bool ReplaceNodeID { get { return (bool)ReplaceNodeIDCheckBox.IsChecked; } }
        public bool ReplaceResourceID { get { return (bool)ReplaceResourceIDCheckBox.IsChecked; } }

        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public GetNewTemplatePopUp()
        {
            InitializeComponent();
        }

        private void CreateDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //For accepting "Return" as "confirmation"
            if (e.Key == Key.Return)
            {
                CreateDashboardButton_OnClick(sender, e);
            }
        }
    }
}
