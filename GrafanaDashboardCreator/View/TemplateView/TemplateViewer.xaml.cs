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
using GrafanaDashboardCreator.Model;
using GrafanaDashboardCreator.View.PopUps;

namespace GrafanaDashboardCreator.View.TemplateView
{
    /// <summary>
    /// Interaktionslogik für TemplateViewer.xaml
    /// </summary>
    public partial class TemplateViewer : Window
    {
        ModelService modelService;

        //View for adding, deleting and editing templates
        public TemplateViewer(ModelService modelService)
        {
            InitializeComponent();
            this.modelService = modelService;
            TemplateListView.ItemsSource = modelService.GetTemplates();
        }

        private void DeleteTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (TemplateListView.SelectedItems.Count == 0)
            {
                return;
            }

            foreach (Template template in TemplateListView.SelectedItems)
            {
                modelService.RemoveTemplate(template);
            }

            TemplateListView.ItemsSource = modelService.GetTemplates();
            TemplateListView.Items.Refresh();
        }

        private void RenameTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Template template = TemplateListView.SelectedItem as Template;

                RenamePopUp popUp = new RenamePopUp();
                popUp.Owner = this;
                popUp.ShowDialog();

                if (!popUp.ButtonPressed)
                {
                    return;
                }

                modelService.ReCreateTemplate(template, popUp.EnteredText, template.JSONtext, template.ReplaceNodeID, template.ReplaceResourceID);
                TemplateListView.ItemsSource = modelService.GetTemplates();
                TemplateListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void ViewTemplateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Template selectedTemplate = TemplateListView.SelectedItem as Template;

                JSONViewer viewer = new JSONViewer(selectedTemplate.JSONtext);
                viewer.Owner = this;
                viewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void GetNewTemplateButton_OnCLick(object sender, RoutedEventArgs e)
        {
            try
            {
                GetNewTemplatePopUp popUp = new GetNewTemplatePopUp
                {
                    Owner = this
                };
                popUp.ShowDialog();

                if (!popUp.ButtonPressed)
                {
                    return;
                }

                string templateName = popUp.TemplateName;
                string templateTitle = popUp.JSONTitle;
                string pathToJSON = popUp.PathToJSON;
                bool replaceNodeID = popUp.ReplaceNodeID;
                bool replaceResourceID = popUp.ReplaceResourceID;

                modelService.CreateTemplate(templateName, templateTitle, pathToJSON, replaceNodeID, replaceResourceID);
                TemplateListView.ItemsSource = modelService.GetTemplates();
                TemplateListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
