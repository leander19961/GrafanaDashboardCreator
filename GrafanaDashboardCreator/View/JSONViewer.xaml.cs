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
    /// Interaktionslogik für JSONViewer.xaml
    /// </summary>
    public partial class JSONViewer : Window
    {
        //Just for viewing text
        public JSONViewer(string jsonText)
        {
            InitializeComponent();
            JSONViewerTextBox.AppendText(jsonText);
        }
    }
}
