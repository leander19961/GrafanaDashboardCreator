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
    /// Interaktionslogik für AreYouShurePopUp.xaml
    /// </summary>
    public partial class AreYouShurePopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool buttonPressed = false;

        public bool ButtonPressed { get { return buttonPressed; } }

        public AreYouShurePopUp(string labelText)
        {
            InitializeComponent();
            MessageLabel.Content = labelText;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
