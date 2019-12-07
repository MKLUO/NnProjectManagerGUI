using System;
using System.Windows.Input;
using NNMCore;
using NNMCore.View;

namespace NnManagerGUI.ViewModel {

    partial class ProjectViewModel {
        #region common_logic

        bool IsSchedulerOff() => !Manager.IsSchedulerOn();
        bool IsProjectLoaded() => Manager.IsProjectLoaded();

        void ResetSelections() {
            SelectionMode = SelectionModes.None;
            ModuleSelectionMode = ModuleSelectionModes.None;

            SelectedTemplate = new NullEntry();
            SelectedPlan = new NullEntry();
            SelectedTask = new NullEntry();
            SelectedModuleInfo = new NullEntry();
            SelectedModule = new NullEntry();
        }

        #endregion

        public ICommand CommandNewProject =>
            new RelayCommand(NewProjectExecute, () => true);
        void NewProjectExecute() {

            string? path = UtilGUI.OpenFileDialogToGetPath();
            if (path == null) return;

            if (Manager.IsBusy()) {
                if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
                    Manager.Terminate();
                else
                    return;
            }

            Manager.NewProject(path);

            if (!Manager.IsProjectLoaded()) {
                UtilGUI.Error("Project creation failed!");
                return;
            }

            ResetSelections();

            // TODO: ?
            //OnPropertyChanged("");
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

            if (newTemp.IsNull()) {
                UtilGUI.Error("Template creation failed!");
            } else {
                SelectedTemplate = newTemp;
            }
        }


        public ICommand CommandDeleteTemplate =>
            new RelayCommand(
                () => {
                    Manager.DeleteTemplate(SelectedTemplate);
                    SelectedTemplate = new NullEntry();
                },
                () => IsProjectLoaded() && (!SelectedTemplate.IsNull()));

        // TODO: Idea: Stand-alone plan import/export (w/ referred template).
        // TODO: Idea: Start a new plan from a task?

        public ICommand CommandAddPlan =>
            new RelayCommand(
                AddPlanEmptyExecute,
                () =>
                    IsSchedulerOff() &&
                    IsProjectLoaded() &&
                    (!SelectedTemplate.IsNull()) &&
                    (!templateParamsForm.IsNull()));
        void AddPlanEmptyExecute() {
            var newPlan = Manager.AddPlan(
                SelectedTemplate, "", templateParamsForm.ToParams());

            if (newPlan.IsNull()) {
                UtilGUI.Error("Plan creation failed!");
            } else {
                SelectedPlan = newPlan;
            }
        }

        public ICommand CommandDeletePlan =>
            new RelayCommand(
                () => {
                    Manager.DeletePlan(SelectedPlan);
                    SelectedPlan = new NullEntry();
                },
                () =>
                IsSchedulerOff() &&
                IsProjectLoaded() &&
                (!SelectedPlan.IsNull()));

        public ICommand CommandAddTask =>
            new RelayCommand(
                AddTaskExecute,
                () =>
                    IsSchedulerOff() &&
                    IsProjectLoaded() &&
                    (!SelectedPlan.IsNull()) &&
                    (!templateParamsForm.IsNull()) &&
                    (SelectionMode != SelectionModes.Template)
            );
        void AddTaskExecute() {
            var newTask = Manager.AddTask(
                SelectedPlan, templateParamsForm.ToParams());

            if (newTask.IsNull()) {
                UtilGUI.Error("Task creation failed!");
            } else {
                SelectedTask = newTask;
            }
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
                    Manager.DeleteTask(SelectedTask);
                    SelectedTask = new NullEntry();
                },
                () =>
                IsSchedulerOff() &&
                IsProjectLoaded() &&
                (!SelectedTask.IsNull()));

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
                    if (IsSchedulerOff())
                        Manager.StartScheduler();
                    else
                        Manager.StopScheduler();
                },
                () => IsProjectLoaded());

        //public ICommand CommandLaunch =>
        //    new RelayCommand(
        //        () => {
        //            if (SelectedTasks != null)
        //                foreach (var task in SelectedTasks)
        //                    task.Launch();
        //        },
        //        () => IsSchedulerOff() && IsProjectLoaded() && (SelectedTasks?.Count > 0));

        public ICommand CommandTerminate =>
            new RelayCommand(
                () => {
                    switch (SelectionMode) {
                        case SelectionModes.Plan:
                            if (!SelectedPlan.IsNull())
                                Manager.Terminate(SelectedPlan); break;
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
            IsSchedulerOff() && IsProjectLoaded() &&
            (
                ((!SelectedPlan.IsNull()) && SelectionMode == SelectionModes.Plan) ||
                ((!SelectedTask.IsNull()) && SelectionMode == SelectionModes.Task)
            ) && (!moduleParamsForm.IsNull());
        Action EnqueueModuleExecute(bool pad = false) =>
            () => {
                switch (SelectionMode) {
                    //case SelectionModes.Plan:
                    //    Manager.AddModule(SelectedPlan,);
                    //    return;

                    case SelectionModes.Task:
                        if (SelectedTasks == null) return;
                        foreach (var task in SelectedTasks)
                            Manager.AddModule(
                                task, SelectedModuleInfo, 
                                moduleParamsForm.ToParams());
                        return;

                    default:
                        return;
                }
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
    }
}
