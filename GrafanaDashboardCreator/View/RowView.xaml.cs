﻿using System;
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
    /// Interaktionslogik für RowView.xaml
    /// </summary>
    public partial class RowView : Page
    {
        //Thats no window! Its a page that bring all needed view elements to render a row with its
        //datasources in the dashboardview, gets added to the content of the tabitem of the dashboard
        public RowView()
        {
            InitializeComponent();
        }
    }
}
