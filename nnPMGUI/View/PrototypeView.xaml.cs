using NNMCore.View;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ProjectViewModel = NnManagerGUI.ViewModel.ProjectViewModel;

namespace NnManagerGUI.View {
    //using Variable = NnManager.NnProjectData.Variable;
    using SelectionModes = ProjectViewModel.SelectionModes;
    using ModuleSelectionModes = ProjectViewModel.ModuleSelectionModes;
    using ExecutionSelectionModes = ProjectViewModel.ExecutionSelectionModes;

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
            if (sender is DataGrid grid)
                if (grid.SelectedItem is INamedForm<string> newForm)
                    VM.UpdateTemplateParamsForm(newForm);
        }

        private void Module_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            if (sender is DataGrid grid)
                if (grid.SelectedItem is INamedForm<string> newForm)
                    VM.UpdateModuleParamsForm(newForm);
        }

        private void Templates_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Template;

        private void Plans_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Plan;

        private void Tasks_GotFocus(object sender, System.Windows.RoutedEventArgs e) {
            VM.SelectionMode = SelectionModes.Task;
            VM.ExecutionSelectionMode = ExecutionSelectionModes.Task;
        }

        private void ModulePalette_GotFocus(object sender, System.Windows.RoutedEventArgs e) {
            VM.ModuleSelectionMode = ModuleSelectionModes.ModulePalette;
            VM.ExecutionSelectionMode = ExecutionSelectionModes.ModulePalette;
        }

        private void Modules_GotFocus(object sender, System.Windows.RoutedEventArgs e) {
            VM.ModuleSelectionMode = ModuleSelectionModes.Module;
            VM.ExecutionSelectionMode = ExecutionSelectionModes.Module;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // TODO:
            //VM.SelectedTasks = (sender as DataGrid).SelectedItems.Cast<NnTaskData>().ToList();
        }
    }
}
