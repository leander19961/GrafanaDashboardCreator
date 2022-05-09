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
    /// Interaktionslogik für RenamePopUp.xaml
    /// </summary>
    public partial class RenamePopUp : Window
    {
        private bool _buttonpressed = false;
        
        public bool ButtonPressed { get { return _buttonpressed; } }

        public string EnteredText { get { return NewNameBox.Text; } }

        public RenamePopUp()
        {
            InitializeComponent();
        }

        private void CreateDashboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            _buttonpressed = true;
            Close();
        }

        private void NewNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateDashboardButton_OnClick(sender, e);
            }
        }
    }
}
