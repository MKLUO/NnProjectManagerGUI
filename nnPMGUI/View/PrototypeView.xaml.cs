using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ProjectViewModel = NnManagerGUI.ViewModel.ProjectViewModel;

namespace NnManagerGUI.View {
    //using Variable = NnManager.NnProjectData.Variable;
    using SelectionModes = ProjectViewModel.SelectionModes;
    using ModuleSelectionModes = ProjectViewModel.ModuleSelectionModes;

    /// <summary>
    /// Interaction logic for PrototypeView.xaml
    /// </summary>
    public partial class PrototypeView : UserControl {
        public PrototypeView() {
            InitializeComponent();
        }

        ProjectViewModel VM => this.DataContext as ProjectViewModel ??
            throw new InvalideViewModelException();

        public class InvalideViewModelException : System.Exception { }

        private void Params_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            //VM.UpdateTemplateParamsForm((sender as DataGrid).SelectedItem as Variable);
        }

        private void Module_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            //VM.UpdateModule(
            //    (sender as DataGrid).SelectedItem as Variable);
        }

        private void Templates_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Template;

        private void Plans_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Plan;

        private void Tasks_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Task;

        private void Modules_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.ModuleSelectionMode = ModuleSelectionModes.Module;

        private void ModuleQueue_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.ModuleSelectionMode = ModuleSelectionModes.ModuleQueue;

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //VM.SelectedTasks = (sender as DataGrid).SelectedItems.Cast<NnTaskData>().ToList();
        }
    }
}
