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

namespace GrafanaDashboardCreator.View
{
    /// <summary>
    /// Interaktionslogik für TemplateViewer.xaml
    /// </summary>
    public partial class TemplateViewer : Window
    {
        ModelService modelService;

        public TemplateViewer(ModelService modelService)
        {
            InitializeComponent();
            this.modelService = modelService;
            TemplateListView.ItemsSource = modelService.GetTemplates();
        }

        private void DeleteTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Template template in TemplateListView.SelectedItems)
            {
                modelService.RemoveTemplate(template);
            }

            TemplateListView.ItemsSource = modelService.GetTemplates();
            TemplateListView.Items.Refresh();
        }

        private void RenameTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Template template = TemplateListView.SelectedItem as Template;

            RenamePopUp popUp = new RenamePopUp();
            popUp.Owner = this;
            popUp.ShowDialog();

            if (!popUp.ButtonPressed)
            {
                return;
            }

            modelService.ReCreateTemplate(popUp.EnteredText, template.JSONtext, template.ReplaceNodeID, template.ReplaceResourceID);
            modelService.RemoveTemplate(template);
            TemplateListView.ItemsSource = modelService.GetTemplates();
            TemplateListView.Items.Refresh();
        }

        private void ViewTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Template selectedTemplate = TemplateListView.SelectedItem as Template;

            JSONViewer viewer = new JSONViewer(selectedTemplate.JSONtext);
            viewer.Owner = this;
            viewer.Show();
        }
    }
}
