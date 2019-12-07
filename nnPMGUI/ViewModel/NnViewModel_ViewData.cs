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
            set => SetField(ref selectionMode, value);
        }


        public enum ModuleSelectionModes {
            None,
            Module,
            ModuleQueue
        }
        ModuleSelectionModes moduleSelectionMode = ModuleSelectionModes.None;
        public ModuleSelectionModes ModuleSelectionMode {
            get => moduleSelectionMode;
            set => SetField(ref moduleSelectionMode, value);
        }

        IEntry selectedTemplate = new NullEntry();
        public IEntry SelectedTemplate {
            get => FindData(selectedTemplate, collectionTemplate);
            set {
                SetField(ref selectedTemplate, FindData(value, collectionTemplate));
                if (!selectedTemplate.IsNull())
                    SelectionMode = SelectionModes.Template;
            }
        }

        IEntry selectedPlan = new NullEntry();
        public IEntry SelectedPlan {
            get => FindData(selectedPlan, collectionPlan);
            set {
                SetField(ref selectedPlan, FindData(value, collectionPlan));
                if (!selectedPlan.IsNull())
                    SelectionMode = SelectionModes.Plan;
            }
        }

        IEntry selectedTask = new NullEntry();
        public IEntry SelectedTask {
            get => FindData(selectedTask, collectionTask);
            set {
                SetField(ref selectedTask, FindData(value, collectionTask));
                if (!selectedTask.IsNull())
                    SelectionMode = SelectionModes.Task;
            }
        }
        public IList<IEntry>? SelectedTasks; // TODO: ?


        IEntry selectedModuleInfo = new NullEntry();
        public IEntry SelectedModuleInfo {
            get => FindData(selectedModuleInfo, collectionModuleInfo);
            set => SetField(ref selectedModuleInfo, FindData(value, collectionModuleInfo));
        }

        IEntry selectedModule = new NullEntry();
        public IEntry SelectedModule {
            get => FindData(selectedModule, collectionModule);
            set => SetField(ref selectedModule, FindData(value, collectionModule));
        }

        #endregion

        #region Collection

        readonly ObservableCollection<IEntry> collectionTemplate = default;
        public ObservableCollection<IEntry> CollectionTemplate {
            get {
                Update(collectionTemplate, Manager.GetTemplates());
                return collectionTemplate;
            }
        }

        readonly ObservableCollection<IEntry> collectionPlan = default;
        public ObservableCollection<IEntry> CollectionPlan {
            get {
                if (SelectedTemplate.IsNull())
                    Update(collectionPlan, Manager.GetPlans());
                else
                    Update(collectionPlan, Manager.GetPlans(SelectedTemplate));
                return collectionPlan;
            }
        }

        readonly ObservableCollection<IEntry> collectionTask = default;
        public ObservableCollection<IEntry> CollectionTask {
            get {
                if (SelectedPlan.IsNull())
                    collectionTask.Clear();
                else
                    Update(collectionTask, Manager.GetTasks(SelectedPlan));
                return collectionTask;
            }
        }

        readonly ObservableCollection<IEntry> collectionModuleInfo = default;
        public ObservableCollection<IEntry> CollectionModuleInfo {
            get {
                if (SelectedTask.IsNull())
                    collectionModuleInfo.Clear();
                else
                    Update(collectionModuleInfo, Manager.GetModuleInfos(SelectedTask));
                // TODO: Stay at same module
                return collectionModuleInfo;
            }
        }

        readonly ObservableCollection<IEntry> collectionModule = default;
        public ObservableCollection<IEntry> CollectionModule {
            get {
                if (SelectedTask.IsNull())
                    collectionModule.Clear();
                else
                    Update(collectionModule, Manager.GetModuleInfos(SelectedTask));
                // TODO: Stay at same module type
                return collectionModule;
            }
        }

        #endregion

        #region ParamsForm

        IParamsForm templateParamsForm = new NullForm();
        public IParamsForm TemplateParamsForm {
            get {
                switch (SelectionMode) {
                    case SelectionModes.Template:
                        if (!SelectedTemplate.IsNull())
                            return (templateParamsForm = Manager.NewFormFromTemplate(SelectedTemplate));
                        break;

                    case SelectionModes.Plan:
                        if (!SelectedPlan.IsNull())
                            return (templateParamsForm = Manager.NewFormFromPlan(SelectedPlan));
                        break;

                    case SelectionModes.Task:
                        if (!SelectedTask.IsNull())
                            return (templateParamsForm = Manager.NewFormFromTask(SelectedTask));
                        break;
                }
                return (templateParamsForm = new NullForm());
            }
        }

        public void UpdateTemplateParamsForm<T>(string key, IParamForm<T> newParamForm) {
            if (templateParamsForm.IsNull())
                return;

            templateParamsForm
                .GetParamForm<T>(key)
                .SetValue(newParamForm.Value);

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
            get {
                switch (ModuleSelectionMode) {
                    case ModuleSelectionModes.Module:
                        if (!SelectedModuleInfo.IsNull())
                            return (moduleParamsForm = Manager.NewModuleFormFromTask(
                                SelectedTask, SelectedModuleInfo));
                        break;

                    case ModuleSelectionModes.ModuleQueue:
                        if (!SelectedModule.IsNull())
                            return (moduleParamsForm = Manager.NewModuleForm(SelectedModule));
                        break;
                }
                return (moduleParamsForm = new NullForm());
            }
        }

        public void UpdateModuleParamsForm<T>(string key, IParamForm<T> newParamForm) {
            if (moduleParamsForm.IsNull())
                return;

            moduleParamsForm
                .GetParamForm<T>(key)
                .SetValue(newParamForm.Value);
        }

        #endregion

        #region Text

        public string? TextSchedulerStatus {
            get {
                if (!Manager.IsProjectLoaded())
                    return "Scheduler: -";
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
    }
}
