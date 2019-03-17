using System.Windows.Controls;

using ProjectViewModel = NnManagerGUI.ViewModel.ProjectViewModel;

namespace NnManagerGUI.View
{
    using Variable = NnManager.NnProjectData.Variable;
    using SelectionModes = ProjectViewModel.SelectionModes;

    /// <summary>
    /// Interaction logic for PrototypeView.xaml
    /// </summary>
    public partial class PrototypeView : UserControl
    {
        public PrototypeView()
        {
            InitializeComponent();
        }

        private void Params_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            (this.DataContext as ProjectViewModel).UpdateParamCollection(
                (sender as DataGrid).SelectedItem as Variable);
        }

        private void Module_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            (this.DataContext as ProjectViewModel).UpdateModule(
                (sender as DataGrid).SelectedItem as Variable);
        }

        private void Templates_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            (this.DataContext as ProjectViewModel).SelectionMode = SelectionModes.Template;

        private void Plans_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            (this.DataContext as ProjectViewModel).SelectionMode = SelectionModes.Plan;

        private void Tasks_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            (this.DataContext as ProjectViewModel).SelectionMode = SelectionModes.Task;


        //private void ParamDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    DataGrid dg = sender as DataGrid;
        //    ((ProjectViewModel)this.DataContext).UpdateParam((Param)dg.SelectedItem);
        //}
    }
}
