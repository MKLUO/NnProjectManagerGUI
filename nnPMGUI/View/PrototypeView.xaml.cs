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

using NnManagerGUI;

using ProjectViewModel = NnManagerGUI.ViewModel.ProjectViewModel;
using Param = NnManagerGUI.ViewModel.ProjectViewModel.Param;

namespace NnManagerGUI.View
{
    /// <summary>
    /// Interaction logic for PrototypeView.xaml
    /// </summary>
    public partial class PrototypeView : UserControl
    {
        public PrototypeView()
        {
            InitializeComponent();
        }

        private void ParamDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            ((ProjectViewModel)this.DataContext).UpdateParamCollection((Param)dg.SelectedItem);
        }

        private void TaskDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
