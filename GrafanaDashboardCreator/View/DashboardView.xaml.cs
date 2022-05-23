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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GrafanaDashboardCreator.View
{
    /// <summary>
    /// Interaktionslogik für DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Page
    {
        //Thats no window! Its a page that bring all neede view elements to render a dashboard with its
        //rows and datasources in the mainview, gets added to the content of the tabitem of the mainview
        internal DashboardView()
        {
            InitializeComponent();
        }
    }
}
