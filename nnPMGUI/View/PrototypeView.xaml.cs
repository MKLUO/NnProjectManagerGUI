
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ProjectViewModel = NnManagerGUI.ViewModel.ProjectViewModel;

namespace NnManagerGUI.View {

    using NNMCore.View;
    using NNMCore.NN.View;

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


        private void Templates_GotFocus(object sender, System.Windows.RoutedEventArgs e) =>
            VM.SelectionMode = SelectionModes.Template;

        private void Plans_GotFocus(object sender, System.Windows.RoutedEventArgs e) {
            VM.SelectionMode = SelectionModes.Plan;
            VM.ExecutionSelectionMode = ExecutionSelectionModes.Plan;
        }

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



        private void Tasks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            VM.SelectedTasks = 
                (sender as DataGrid)?.SelectedItems.Cast<INNTaskEntry>()
                .ToList() ?? new List<INNTaskEntry> { }; 
        }
        private void Modules_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //VM.SelectedModules =
            //    (sender as DataGrid)?.SelectedItems.Cast<INNModuleEntry>()
            //    .ToList() ?? new List<INNModuleEntry> { };
            VM.SelectedModuleVMs =
                (sender as DataGrid)?.SelectedItems.Cast<ViewModel.INNModuleEntryVM>()
                .ToList() ?? new List<ViewModel.INNModuleEntryVM> { };
        }



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

        private void ModuleComboBox_DropDownClosed(object sender, System.EventArgs e) {
            if (sender is ComboBox cBox)
                if (cBox.SelectedItem is INamedForm<string> newForm)
                    VM.UpdateModuleParamsForm(newForm);
        }
    }
}