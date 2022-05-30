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

namespace GrafanaDashboardCreator.View.CredentialsView
{
    /// <summary>
    /// Interaktionslogik für EditCredentialsPopUp.xaml
    /// </summary>
    public partial class EditCredentialsPopUp : Window
    {
        //Button pressed is for checking if the window was closed without pressing the "Confirm" button
        private bool _buttonPressed = false;

        public bool ButtonPressed { get { return _buttonPressed; } }

        public string Username { get { return UsernameTextBox.Text; } }
        public string Password { get { return PasswordTextBox.Text; } }
        public string Token { get { return TokenTextBox.Text; } }
        public string Url { get { return UrlTextBox.Text; } }

        public EditCredentialsPopUp(Credentials credentials)
        {
            InitializeComponent();
            UsernameTextBox.Text = credentials.Username;
            PasswordTextBox.Text = credentials.Password;
            TokenTextBox.Text = credentials.Token;
            UrlTextBox.Text = credentials.Url;
        }

        private void EditPropertiesButton_OnClick(object sender, RoutedEventArgs e)
        {
            _buttonPressed = true;
            Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //For accepting "Return" as "confirmation"
            if (e.Key == Key.Return)
            {
                EditPropertiesButton_OnClick(sender, e);
            }
        }
    }
}
