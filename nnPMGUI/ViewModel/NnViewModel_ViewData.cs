using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;

using System;

using NNMCore.View;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using NNMCore;

namespace NnManagerGUI.ViewModel {
    //using ModuleType = NNMCore.NNModuleType;

    partial class ProjectViewModel {

        #region Selection

        public enum SelectionModes {
            None,
            Template,
            Plan,
            Task
        }
        SelectionModes selectionMode = SelectionModes.None;
        public SelectionModes SelectionMode {
            get => selectionMode;
            set {
                SetField(ref selectionMode, value);
                ModuleSelectionMode = ModuleSelectionModes.None;
                switch (selectionMode) {
                    case SelectionModes.Template:
                        SelectedTemplate = SelectedTemplate; return;
                    case SelectionModes.Plan:
                        SelectedPlan = SelectedPlan; return;
                    case SelectionModes.Task:
                        SelectedTask = SelectedTask; return;
                }
            }
        }

        public enum ModuleSelectionModes {
            None,
            ModulePalette,
            Module
        }
        ModuleSelectionModes moduleSelectionMode = ModuleSelectionModes.None;
        public ModuleSelectionModes ModuleSelectionMode {
            get => moduleSelectionMode;
            set {
                SetField(ref moduleSelectionMode, value);
                switch (moduleSelectionMode) {
                    case ModuleSelectionModes.ModulePalette:
                        SelectedModulePallete = SelectedModulePallete; return;
                    case ModuleSelectionModes.Module:
                        SelectedModule = SelectedModule; return;
                }
            }
        }

        public enum ExecutionSelectionModes {
            None,
            Task,
            ModulePalette,
            Module
        }
        ExecutionSelectionModes executionSelectionMode = ExecutionSelectionModes.None;
        public ExecutionSelectionModes ExecutionSelectionMode {
            get => executionSelectionMode;
            set {
                SetField(ref executionSelectionMode, value);
                //switch (executionSelectionMode) {
                //    case ExecutionSelectionModes.Task:
                //        SelectedModulePallete = SelectedModulePallete; return;
                //    case ExecutionSelectionModes.Module:
                //        SelectedModule = SelectedModule; return;
                //}
            }
        }


        INNTemplateEntry? selectedTemplate = null;
        public INNTemplateEntry? SelectedTemplate {
            get => FindData(selectedTemplate, collectionTemplate);
            set {
                SetField(ref selectedTemplate, FindData(value, collectionTemplate));
                NewTemplateParamsForm();
            }
        }

        INNPlanEntry? selectedPlan = null;
        public INNPlanEntry? SelectedPlan {
            get => FindData(selectedPlan, collectionPlan);
            set {
                SetField(ref selectedPlan, FindData(value, collectionPlan));
                NewTemplateParamsForm();
            }
        }

        INNTaskEntry? selectedTask = null;
        public INNTaskEntry? SelectedTask {
            get => FindData(selectedTask, collectionTask);
            set {
                SetField(ref selectedTask, FindData(value, collectionTask));
                NewTemplateParamsForm();
            }
        }
        public IList<INNTaskEntry>? SelectedTasks; // TODO: ?


        INNModuleInfoEntry? selectedModulePallete = null;
        public INNModuleInfoEntry? SelectedModulePallete {
            get => FindData(selectedModulePallete, collectionModulePallete);
            set {
                SetField(ref selectedModulePallete, FindData(value, collectionModulePallete));
                NewModuleParamsForm();
            }
        }

        INNModuleEntry? selectedModule = null;
        public INNModuleEntry? SelectedModule {
            get => FindData(selectedModule, collectionModule);
            set {
                SetField(ref selectedModule, FindData(value, collectionModule));
                NewModuleParamsForm();
            }
        }

        #endregion

        #region Collection

        static ObservableCollection<TIEntry> Empty<TIEntry>() => new ObservableCollection<TIEntry>();

        ObservableCollection<INNTemplateEntry> collectionTemplate = Empty<INNTemplateEntry>();
        public ObservableCollection<INNTemplateEntry> CollectionTemplate {
            get {
                if (!IsProjectLoaded())
                    collectionTemplate = Empty<INNTemplateEntry>();
                else
                    Update(ref collectionTemplate, Manager.GetTemplates());
                return collectionTemplate;
            }
        }

        ObservableCollection<INNPlanEntry> collectionPlan = Empty<INNPlanEntry>();
        public ObservableCollection<INNPlanEntry> CollectionPlan {
            get {
                if (!IsProjectLoaded())
                    collectionPlan = Empty<INNPlanEntry>();
                else
                    Update(ref collectionPlan, Manager.GetPlans());
                return collectionPlan;
            }
        }

        ObservableCollection<INNTaskEntry> collectionTask = Empty<INNTaskEntry>();
        public ObservableCollection<INNTaskEntry> CollectionTask {
            get {
                if (!IsProjectLoaded() || SelectedPlan == null)
                    collectionTask = Empty<INNTaskEntry>();
                else
                    Update(ref collectionTask, Manager.GetTasks(SelectedPlan));
                return collectionTask;
            }
        }

        ObservableCollection<INNModuleInfoEntry> collectionModulePallete = Empty<INNModuleInfoEntry>();
        public ObservableCollection<INNModuleInfoEntry> CollectionModulePallete {
            get {
                if (!IsProjectLoaded() || SelectedTask == null)
                    collectionModulePallete = Empty<INNModuleInfoEntry>();
                else
                    Update(ref collectionModulePallete, Manager.GetModuleInfos(SelectedTask));
                // TODO: Stay at same module?
                return collectionModulePallete;
            }
        }

        ObservableCollection<INNModuleEntry> collectionModule = Empty<INNModuleEntry>();
        public ObservableCollection<INNModuleEntry> CollectionModule {
            get {
                if (!IsProjectLoaded() || SelectedTask == null)
                    collectionModule = Empty<INNModuleEntry>();
                else
                    Update(ref collectionModule, Manager.GetModules(SelectedTask));
                return collectionModule;
            }
        }

        #endregion

        #region ParamsForm

        IParamsForm templateParamsForm = new NullForm();
        public IParamsForm TemplateParamsForm {
            get => ParamDiffOnly ? templateParamsForm : templateParamsForm;
            set => SetField(ref templateParamsForm, value);
        }
        public void NewTemplateParamsForm() {
            switch (SelectionMode) {
                case SelectionModes.Template:
                    if (SelectedTemplate == null) break;
                    TemplateParamsForm = Manager.NewFormFromTemplate(SelectedTemplate);
                    return;

                case SelectionModes.Plan:
                    if (SelectedPlan == null) break;
                    TemplateParamsForm = Manager.NewFormFromPlan(SelectedPlan);
                    return;

                case SelectionModes.Task:
                    if (SelectedTask == null || SelectedPlan == null) break;
                    TemplateParamsForm = Manager.NewFormFromTask(
                            SelectedTask, SelectedPlan);
                    return;
            }
            TemplateParamsForm = new NullForm();
        }
        public void UpdateTemplateParamsForm<T>(INamedForm<T> newParamForm) {
            if (TemplateParamsForm.IsNull())
                return;

            TemplateParamsForm
                .GetParamForm<T>(newParamForm.Key).Value =
                newParamForm.Value ?? newParamForm.DefaultValue;

            // TODO: Common Var logic.

            //foreach (var oldVar in param.CommonVariables)
            //    if (oldVar.Name == newVar.Name) {
            //        oldVar.Value = newVar.Value;
            //        return;
            //    }
        }

        // TODO: Multiple Task logic
        IParamsForm moduleParamsForm = new NullForm();
        public IParamsForm ModuleParamsForm {
            get => moduleParamsForm;
            private set => SetField(ref moduleParamsForm, value);
        }
        public void NewModuleParamsForm() {
            switch (ModuleSelectionMode) {
                case ModuleSelectionModes.ModulePalette:
                    if (SelectedModulePallete == null || SelectedTask == null) break;
                    ModuleParamsForm = Manager.NewModuleFormFromTask(
                            SelectedTask, SelectedModulePallete);
                    return;

                case ModuleSelectionModes.Module:
                    if (SelectedModule == null || SelectedTask == null) break;
                    ModuleParamsForm = Manager.NewModuleForm(
                            SelectedModule, SelectedTask);
                    return;
            }
            ModuleParamsForm = new NullForm();
        }
        public void UpdateModuleParamsForm<T>(INamedForm<T> newParamForm) {
            if (ModuleParamsForm.IsNull())
                return;

            ModuleParamsForm
                .GetParamForm<T>(newParamForm.Key).Value =
                newParamForm.Value ?? newParamForm.DefaultValue;
        }

        #endregion

        #region Text

        public string? TextSchedulerStatus {
            get {
                if (!Manager.IsProjectLoaded())
                    return "Scheduler";
                else
                    return Manager.IsSchedulerOn() ? "Scheduler: ON" : "Scheduler: OFF";
            }
        }

        public string TextEnqueueModuleButton {
            get {
                //switch (SelectionMode) {
                //    case SelectionModes.Plan:
                //        return "Enqueue (Plan)";

                //    case SelectionModes.Task:
                //        return "Enqueue (Task)";

                //    default:
                //        return "Enqueue ...";
                //}

                return "Enqueue";
            }
        }

        public string TextEnqueueModulePadButton => "Pad";

        public string TextDequeueModuleButton {
            get {
                switch (SelectionMode) {
                    case SelectionModes.Plan:
                        return "Clear (Plan)";

                    case SelectionModes.Task:
                        return "Clear (Task)";

                    default:
                        return "Clear";
                }
            }
        }

        // TODO: Logging

        //public string TextLog =>
        //    projectData?.Log ?? "";

        #endregion

        #region CheckBox

        bool paramDiffOnly = false;
        public bool ParamDiffOnly {
            get => paramDiffOnly;
            set => SetField(ref paramDiffOnly, value);
        }

        #endregion
    }
}
