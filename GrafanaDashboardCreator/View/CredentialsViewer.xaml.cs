using GrafanaDashboardCreator.Model;
using HandlebarsDotNet.Collections;
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
    /// Interaktionslogik für CredentialsViewer.xaml
    /// </summary>
    public partial class CredentialsViewer : Window
    {
        private ModelService modelService;
        private bool _buttonPressed = false;

        public bool ButtonPressed { get { return _buttonPressed; } }

        public CredentialsViewer(ModelService modelService)
        {
            InitializeComponent();
            this.modelService = modelService;
            CredentialsListView.ItemsSource = modelService.GetCredentials();
        }

        private void EditCredentialsButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CredentialsListView.SelectedItem == null)
                {
                    return;
                }

                Credentials selectedCredentials = CredentialsListView.SelectedItem as Credentials;

                EditCredentialsPopUp popUp = new EditCredentialsPopUp(selectedCredentials)
                {
                    Owner = this
                };
                popUp.ShowDialog();

                if (!popUp.ButtonPressed)
                {
                    return;
                }

                modelService.EditCredentialProperties(selectedCredentials, popUp.Username, popUp.Password, popUp.Token, popUp.Url);
                CredentialsListView.ItemsSource = modelService.GetCredentials();
                CredentialsListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteCredentialsButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
