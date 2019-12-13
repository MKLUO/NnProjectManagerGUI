using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using NNMCore;

namespace NnManagerGUI.ViewModel {

    partial class ProjectViewModel {
        #region common_logic

        bool IsProjectLoaded() => Manager.IsProjectLoaded();
        bool IsSchedulerOff() => Manager.IsProjectLoaded() ? Manager.SchedulerStatus == SchedulerStatus.IDLE : true;

        void ResetSelections() {
            SelectionMode = SelectionModes.None;
            ModuleSelectionMode = ModuleSelectionModes.None;
            ExecutionSelectionMode = ExecutionSelectionModes.None;

            SelectedTemplate = null;
            SelectedPlan = null;
            SelectedTask = null;
            SelectedModulePallete = null;
            SelectedModule = null;
            //SelectedModuleVM = null;

            SelectedTasks = new List<INNTaskEntry> { };
            //SelectedModules = new List<INNModuleEntry> { };
            SelectedModuleVMs = new List<INNModuleEntryVM> { };
        }

        #endregion


        public ICommand CommandNewProject =>
            new RelayCommand(NewProjectExecute, () => true);
        void NewProjectExecute() {
            string? path = UtilGUI.OpenFileDialogToGetPath();
            if (path == null) return;

            if (Manager.IsBusy()) {
                if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
                    Manager.Reset();
                else return;
            }

            Manager.NewProject(path);

            if (!Manager.IsProjectLoaded()) {
                UtilGUI.Error("Project creation failed!");
                return;
            }

            ResetSelections();
        }

        //public ICommand CommandLoadProject =>
        //    new RelayCommand(LoadProjectExecute, () => true);
        //void LoadProjectExecute() {
        //    string? path = UtilGUI.OpenFileDialogToGetFolder();
        //    if (path == null) return;

        //    if (projectData?.IsBusy ?? false) {
        //        if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
        //            projectData?.Terminate();
        //        else
        //            return;
        //    }

        //    projectData = NnProjectData.Load(path);
        //    if (projectData == null) {
        //        UtilGUI.Error("Error!");
        //        return;
        //    }

        //    projectData.PropertyChanged += OnComponentPropertyChanged;

        //    ResetSelectionAndCollections();
        //    OnPropertyChanged("");
        //}

        //public ICommand CommandSaveProject =>
        //    new RelayCommand(() => projectData?.Save(), IsProjectLoaded);

        public ICommand CommandAddTemplate =>
            new RelayCommand(AddTemplateExecute, Manager.IsProjectLoaded);
        void AddTemplateExecute() {
            string? path = UtilGUI.OpenFileDialogToGetPath();
            if (path == null) return;

            var newTemp = Manager.AddTemplate(path);
            if (newTemp == null) return;

            OnPropertyChanged(() => CollectionTemplate);
            SelectionMode = SelectionModes.Template;
            SelectedTemplate = newTemp;
        }


        public ICommand CommandDeleteTemplate =>
            new RelayCommand(
                () => {
                    if (SelectedTemplate == null) return;

                    Manager.DeleteTemplate(SelectedTemplate);
                    OnPropertyChanged(() => CollectionTemplate);
                    SelectedTemplate = null;

                },
                () => IsProjectLoaded() && (SelectedTemplate != null));

        public ICommand CommandAddPlan =>
            new RelayCommand(
                AddPlanExecute,
                () =>
                    IsProjectLoaded() &&
                    (SelectedTemplate != null) &&
                    (TemplateParamsForm != null));
        void AddPlanExecute() {
            if (SelectedTemplate == null) return;
            if (TemplateParamsForm == null) return;

            string? name = UtilGUI.OpenInputDialogToGetText(
                "Name of the new plan:", SelectedTemplate.Title);
            if (name == null) return;

            var newPlan = Manager.AddPlan(
                SelectedTemplate, name, TemplateParamsForm);

            OnPropertyChanged(() => CollectionPlan);
            SelectionMode = SelectionModes.Plan;
            SelectedPlan = newPlan;
        }

        public ICommand CommandDeletePlan =>
            new RelayCommand(
                () => {
                    if (SelectedPlan == null) return;

                    Manager.DeletePlan(SelectedPlan);
                    OnPropertyChanged(() => CollectionPlan);
                    SelectedPlan = null;
                },
                () =>
                IsSchedulerOff() &&
                IsProjectLoaded() &&
                (SelectedPlan != null));

        public ICommand CommandAddTask =>
            new RelayCommand(
                AddTaskExecute,
                () =>
                    IsProjectLoaded() &&
                    (SelectedPlan != null) &&
                    (TemplateParamsForm != null));
        void AddTaskExecute() {
            if (SelectedPlan == null) return;
            if (TemplateParamsForm == null) return;

            var newTask = Manager.AddTask(
                SelectedPlan, TemplateParamsForm);

            OnPropertyChanged(() => CollectionTask);
            SelectionMode = SelectionModes.Task;
            SelectedTask = newTask;
        }

        public ICommand CommandDeleteTask =>
            new RelayCommand(
                () => {
                    if (SelectedTask == null) return;

                    Manager.DeleteTask(SelectedTask);
                    OnPropertyChanged(() => CollectionTask);
                    SelectedTask = null;
                },
                () =>
                IsSchedulerOff() &&
                IsProjectLoaded() &&
                (SelectedTask != null));

        // TODO: Module Set logic.
        public ICommand CommandEnqueueModule =>
            new RelayCommand(
                EnqueueModuleExecute(),
                CanEnqueueModuleExecute
            );
        //public ICommand CommandEnqueueModulePad =>
        //    new RelayCommand(
        //        EnqueueModuleExecute(pad: true),
        //        CanEnqueueModuleExecute
        //    );
        bool CanEnqueueModuleExecute() =>
            //IsSchedulerOff() && IsProjectLoaded() &&
            IsProjectLoaded() &&
            (
                ((SelectedPlan != null) && SelectionMode == SelectionModes.Plan) ||
                ((SelectedTask != null) && SelectionMode == SelectionModes.Task)
            ) && (ModuleParamsForm != null);
        Action EnqueueModuleExecute(bool pad = false) =>
            () => {
                if (SelectedTask == null) return;
                if (ModuleParamsForm == null) return;


                var collectionDep = CollectionModuleVM
                    .Where(vm => vm.Dep).Select(vm => vm.Entry).ToArray();

                var newModule = collectionDep.Any() ?
                    Manager.AddModule(SelectedTask, ModuleParamsForm, collectionDep) :
                    Manager.AddModule(SelectedTask, ModuleParamsForm);

                if (newModule == null) return;

                OnPropertyChanged(() => CollectionModuleVM);
                ModuleSelectionMode = ModuleSelectionModes.Module;
                SelectedModule = newModule;
                //SelectedModuleVM = ToModuleVM(newModule);
            };

        public ICommand CommandToggleQueue =>
            new RelayCommand(
                () => {
                    switch (Manager.SchedulerStatus) {
                        case SchedulerStatus.IDLE:
                            Manager.StartScheduler(); break;
                        case SchedulerStatus.Actice:
                            Manager.StopScheduler(true); break;
                        case SchedulerStatus.Stopping:
                            break;
                    }
                    OnPropertyChanged(() => TextSchedulerStatus);
                },
                () => IsProjectLoaded() &&
                Manager.SchedulerStatus != SchedulerStatus.Stopping);

        public ICommand CommandLaunch =>
            new RelayCommand(
                LaunchExecute,
                () =>
                IsProjectLoaded() &&
                CanLaunchExecute());
        bool CanLaunchExecute() {
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    return SelectedPlan != null;
                case ExecutionSelectionModes.Task:
                    return SelectedTask != null;
                case ExecutionSelectionModes.Module:
                    return SelectedModule != null;
                default:
                    return false;
            }
        }
        void LaunchExecute() {
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    if (SelectedPlan == null) return;
                    Manager.Launch(SelectedPlan); return;
                case ExecutionSelectionModes.Task:
                    if (SelectedTask == null) return;
                    Manager.Launch(SelectedTask); return;
                case ExecutionSelectionModes.Module:
                    if (SelectedModule == null) return;
                    Manager.Launch(SelectedModule); return;
            }
        }

        public ICommand CommandReset =>
            new RelayCommand(
                ResetExecute,
                () =>
                IsProjectLoaded() &&
                CanResetExecute());
        bool CanResetExecute() {
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    return SelectedPlan != null;
                case ExecutionSelectionModes.Task:
                    return SelectedTask != null;
                case ExecutionSelectionModes.Module:
                    return SelectedModule != null;
                default:
                    return false;
            }
        }
        void ResetExecute() {
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    if (SelectedPlan == null) return;
                    Manager.Reset(SelectedPlan); return;
                case ExecutionSelectionModes.Task:
                    if (SelectedTask == null) return;
                    Manager.Reset(SelectedTask); return;
                case ExecutionSelectionModes.Module:
                    if (SelectedModule == null) return;
                    Manager.Reset(SelectedModule); return;
            }
        }


        public ICommand CommandClearModules =>
            new RelayCommand(
                ClearModulesExecute,
                () =>
                    IsSchedulerOff() && IsProjectLoaded() &&
                    CanClearModulesExecute());
        bool CanClearModulesExecute() {
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    return SelectedPlan != null;
                case ExecutionSelectionModes.Task:
                    return SelectedTask != null;
                case ExecutionSelectionModes.Module:
                    return SelectedModule != null;
                default:
                    return false;
            }
        }
        void ClearModulesExecute() {
            // TODO:
            switch (ExecutionSelectionMode) {
                case ExecutionSelectionModes.Plan:
                    if (SelectedPlan == null) return;
                    Manager.Reset(SelectedPlan); return;
                case ExecutionSelectionModes.Task:
                    if (SelectedTask == null) return;
                    Manager.Reset(SelectedTask); return;
                case ExecutionSelectionModes.Module:
                    if (SelectedModule == null) return;
                    Manager.Reset(SelectedModule); return;
            }
        }

        public ICommand CommandMAGIC =>
            new RelayCommand(
                () => {
                    Manager.NewProject(
                        @"E:\research_materials\QDOTQBIT\DATAS\1207\magic");
                    var template = Manager.AddTemplate(
                        @"E:\research_materials\QDOTQBIT\NnTestFiles\SiGe_Si_HEMT_DQD_Petta.in");
                    var form = Manager.NewFormFromTemplate(template);
                    var plan = Manager.AddPlan(template, "PLAN", form);
                    var task = Manager.AddTask(plan, form);
                    var moduleInfo = Manager.GetModuleInfos(task);
                    var moduleForm = Manager.NewModuleFormFromTask(task, moduleInfo[0]);
                    var module = Manager.AddModule(task, moduleForm);

                    OnPropertyChanged("");
                    SelectionMode = SelectionModes.Task;
                    ExecutionSelectionMode = ExecutionSelectionModes.Task;
                    ModuleSelectionMode = ModuleSelectionModes.Module;
                    SelectedTemplate = template;
                    SelectedPlan = plan;
                    SelectedTask = task;
                    SelectedModulePallete = moduleInfo[0];
                    SelectedModule = module;
                    //SelectedModuleVM = ToModuleVM(module);
                }, () => true);
    }
}

