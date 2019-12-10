using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Input;
using NNMCore;
using NNMCore.View;

namespace NnManagerGUI.ViewModel {

    partial class ProjectViewModel {
        #region common_logic

        bool IsSchedulerOff() => Manager.SchedulerStatus == SchedulerStatus.IDLE;
        bool IsProjectLoaded() => Manager.IsProjectLoaded();

        void ResetSelections() {
            SelectionMode = SelectionModes.None;
            ModuleSelectionMode = ModuleSelectionModes.None;
            ExecutionSelectionMode = ExecutionSelectionModes.None;

            SelectedTemplate = null;
            SelectedPlan = null;
            SelectedTask = null;
            SelectedModulePallete = null;
            SelectedModule = null;
        }

        #endregion

        //BackgroundWorker Worker { get; }
        //void InitializeWorker() {
        //    using var cts = new CancellationTokenSource();
        //    Worker.DoWork += (sender, e) => {
        //        var worker = sender as BackgroundWorker;
        //        Manager.WorkLoopAsync(cts.Token);
        //    };
        //    Worker.DoWork += (sender, e) => {
        //        while (true) 
        //            if (e.Cancel)
        //                cts.Cancel();
        //    };
        //}

        public ICommand CommandNewProject =>
            new RelayCommand(NewProjectExecute, () => true);
        void NewProjectExecute() {
            string? path = UtilGUI.OpenFileDialogToGetPath();
            if (path == null) return;

            if (Manager.IsBusy()) {
                if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
                    Manager.Terminate();
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

            //if (newTemp.IsNull()) {
            //    UtilGUI.Error("Template creation failed!");
            //} else {
            OnPropertyChanged(() => CollectionTemplate);
            SelectionMode = SelectionModes.Template;
            SelectedTemplate = newTemp;
            //}
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

        // TODO: Idea: Stand-alone plan import/export (w/ referred template).
        // TODO: Idea: Start a new plan from a task?

        public ICommand CommandAddPlan =>
            new RelayCommand(
                AddPlanExecute,
                () =>
                    IsSchedulerOff() &&
                    IsProjectLoaded() &&
                    (SelectedTemplate != null) &&
                    (!TemplateParamsForm.IsNull()));
        void AddPlanExecute() {
            if (SelectedTemplate == null) return;

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
                    IsSchedulerOff() &&
                    IsProjectLoaded() &&
                    (SelectedPlan != null) &&
                    (!TemplateParamsForm.IsNull()));
        void AddTaskExecute() {
            if (SelectedPlan == null) return;

            var newTask = Manager.AddTask(
                SelectedPlan, TemplateParamsForm);

            OnPropertyChanged(() => CollectionTask);
            SelectionMode = SelectionModes.Task;
            SelectedTask = newTask;
        }

        //public ICommand CommandAddTaskFromFile =>
        //    new RelayCommand(
        //        AddTaskFromFileExecute,
        //        () =>
        //            IsSchedulerOff() && IsProjectLoaded() &&
        //            (selectedPlan != null) &&
        //            (SelectionMode != SelectionModes.Template)
        //    );
        //void AddTaskFromFileExecute() {
        //    var (fileOpened, _, content) =
        //        UtilGUI.OpenFileDialogToGetNameAndContent(
        //            "NN++ template files (*.nnptmpl)|*.nnptmpl|All files (*.*)|*.*",
        //            "Choose a parameter file..."
        //        );

        //    if (fileOpened == false)
        //        return;

        //    selectedPlan?.AddTaskFromFile(content);
        //    SelectionMode = SelectionModes.Plan;
        //}

        public ICommand CommandDeleteTask =>
            new RelayCommand(
                () => {
                    if (SelectedTask == null) return;

                    Manager.DeleteTask(SelectedTask);
                    SelectedTask = null;
                },
                () =>
                IsSchedulerOff() &&
                IsProjectLoaded() &&
                (SelectedTask != null));

        //public ICommand StartQueue => 
        //    new RelayCommand(
        //        () => projectData?.StartScheduler(),
        //        () => IsProjectLoaded() && !IsQueueRunning());

        //public ICommand StopQueue =>
        //    new RelayCommand(
        //        () => projectData?.StopScheduler(),
        //        () => IsProjectLoaded() && IsQueueRunning());

        public ICommand CommandToggleQueue =>
            new RelayCommand(
                () => {
                    switch (Manager.SchedulerStatus) {
                        case SchedulerStatus.IDLE:
                            Manager.StartScheduler(); break;
                        case SchedulerStatus.Actice:
                            Manager.StopScheduler(); break;
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
                case ExecutionSelectionModes.Task:
                    if (SelectedTask == null) return;
                    Manager.Launch(SelectedTask); return;
                case ExecutionSelectionModes.Module:
                    if (SelectedModule == null) return;
                    Manager.Launch(SelectedModule); return;
            }
        }

        public ICommand CommandTerminate =>
            new RelayCommand(
                () => {
                    switch (SelectionMode) {
                        case SelectionModes.Plan:
                            //if (SelectedPlan != null) 
                            //Manager.Terminate(SelectedPlan); break;
                            break;
                        case SelectionModes.Task:
                            if (SelectedTasks == null) return;
                            foreach (var task in SelectedTasks)
                                Manager.Terminate(task); break;
                    }
                },
                () => IsSchedulerOff() && IsProjectLoaded());

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
            ) && (!ModuleParamsForm.IsNull());
        Action EnqueueModuleExecute(bool pad = false) =>
            () => {
                if (SelectedTask == null) return;

                var newModule = Manager.AddModule(
                    SelectedTask,
                    ModuleParamsForm);

                OnPropertyChanged(() => CollectionModule);
                ModuleSelectionMode = ModuleSelectionModes.Module;
                SelectedModule = newModule;

                return;
                //switch (SelectionMode) {
                //    //case SelectionModes.Plan:
                //    //    Manager.AddModule(SelectedPlan,);
                //    //    return;

                //    case SelectionModes.Task:
                //        var newModule = Manager.AddModule(
                //            SelectedTask, SelectedModulePallete,
                //            ModuleParamsForm.ToParams());
                //        if (newModule.IsNull()) {
                //            UtilGUI.Error("Module creation failed!");
                //        } else {
                //            OnPropertyChanged(() => CollectionModule);
                //            ModuleSelectionMode = ModuleSelectionModes.Module;
                //            SelectedModule = newModule;
                //        }
                //        return;
                //    //if (SelectedTasks == null) return;
                //    //foreach (var task in SelectedTasks)
                //    //    Manager.AddModule(
                //    //        task, SelectedModulePallete, 
                //    //        moduleParamsForm.ToParams());
                //    //return;

                //    default:
                //        return;
                //}
            };

        //public ICommand CommandClearModules =>
        //    new RelayCommand(
        //        ClearModulesExecute,
        //        () =>
        //            IsSchedulerOff() && IsProjectLoaded() &&
        //            (
        //                ((selectedPlan != null) && SelectionMode == SelectionModes.Plan) ||
        //                ((selectedTask != null) && SelectionMode == SelectionModes.Task)
        //            ));
        //void ClearModulesExecute() {
        //    switch (SelectionMode) {
        //        case SelectionModes.Plan:
        //            if (selectedPlan == null) return;
        //            selectedPlan.ClearModules();
        //            break;

        //        case SelectionModes.Task:
        //            if (selectedPlan == null) return;
        //            if (SelectedTasks == null) return;
        //            foreach (var task in SelectedTasks)
        //                task.ClearModules();
        //            break;

        //        default:
        //            return;
        //    }
        //}

        //FIXME: HACK!!!!
        //public ICommand CommandGenerateSomeReport =>
        //    new RelayCommand(
        //        GenerateSomeReport,
        //        () => IsSchedulerOff() && IsProjectLoaded() && (selectedPlan != null));
        //void GenerateSomeReport() {
        //    selectedPlan?.GenerateSomeReport();
        //}

        //public ICommand CommandGenerateBReport0323 =>
        //    new RelayCommand(
        //        GenerateBReport0323Execute,
        //        () => IsSchedulerOff() && IsProjectLoaded() && (selectedPlan != null));
        //void GenerateBReport0323Execute()
        //{
        //    selectedPlan?.GenerateBReport0323();
        //}

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
                    ModuleSelectionMode = ModuleSelectionModes.Module;
                    SelectedModule = module;
                }, () => true);
    }
}

