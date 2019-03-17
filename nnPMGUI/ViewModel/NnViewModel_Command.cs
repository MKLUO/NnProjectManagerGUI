using System.Windows.Input;

using NnManager;

#nullable enable

namespace NnManagerGUI.ViewModel
{
    using NnPlanData = NnProjectData.NnPlanData;
    using NnTaskData = NnProjectData.NnTaskData;
    using NnTemplateData = NnProjectData.NnTemplateData;

    partial class ProjectViewModel
    {
        #region common_logic

        bool IsProjectLoaded() => projectData != null;

        //bool IsQueueRunning() => projectData?.IsSchedularRunning ?? false;

        void ResetSelectionAndCollections()
        {
            SelectedTemplate = null;
            SelectedModule = null;
            SelectedPlan = null;
            SelectedTask = null;
            SelectionMode = null;

            planCollection = null;
            taskCollection = null;
        }

        #endregion

        public ICommand CommandNewProject => 
            new RelayCommand(NewProjectExecute, () => true);
        void NewProjectExecute()
        {
            string? path = UtilGUI.OpenFileDialogToGetPath();
            if (path == null) return;

            if (projectData?.IsBusy ?? false) {
                if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
                    projectData?.Terminate();
                else
                    return;
            }

            projectData = NnProjectData.New(path);
            if (projectData == null) {
                UtilGUI.Error("Error!");
                return;
            }

            projectData.PropertyChanged += OnComponentPropertyChanged;

            ResetSelectionAndCollections();
            OnPropertyChanged("");
        }

        public ICommand CommandLoadProject =>
            new RelayCommand(LoadProjectExecute, () => true);
        void LoadProjectExecute()
        {
            string? path = UtilGUI.OpenFileDialogToGetFolder();
            if (path == null) return;

            if (projectData?.IsBusy ?? false) {
                if (UtilGUI.WarnAndDecide("Current project is busy.\nTerminate and continue?"))
                    projectData?.Terminate();
                else
                    return;
            }

            projectData = NnProjectData.Load(path);
            if (projectData == null) {
                UtilGUI.Error("Error!");
                return;
            }

            projectData.PropertyChanged += OnComponentPropertyChanged;

            ResetSelectionAndCollections();
            OnPropertyChanged("");
        }

        //public ICommand CommandSaveProject =>
        //    new RelayCommand(() => projectData?.Save(), IsProjectLoaded);

        public ICommand CommandAddTemplate => 
            new RelayCommand(AddTemplateExecute, IsProjectLoaded);
        void AddTemplateExecute()
        {
            var (fileOpened, id, content) =
                UtilGUI.OpenFileDialogToGetNameAndContent(
                    "NN++ template files (*.nnptmpl)|*.nnptmpl|All files (*.*)|*.*",
                    "Choose a template..."
                );

            if (fileOpened == false) return;

            NnTemplateData? temp;
            if ((temp = projectData?.AddTemplate(id, content)) != null) {
                SelectedTemplate = temp;
                SelectionMode = SelectionModes.Template;
            }
        }

        public ICommand CommandDeleteTemplate => 
            new RelayCommand(
                DeleteTemplateExecute, 
                () => IsProjectLoaded() && (selectedTemplate != null));
        void DeleteTemplateExecute()
        {
            if (selectedTemplate == null) return;
            if (projectData?.DeleteTemplate(selectedTemplate) ?? false) {
                //selectedTemplateId = null;
            }
        }

        //public ICommand CommandAddPlanFromFile =>
        //    new RelayCommand(
        //        AddPlanFromFileExecute,
        //        () => IsProjectLoaded() && (selectedTemplateId != null));
        //void AddPlanFromFileExecute()
        //{
        //    if (selectedTemplateId == null) return;

        //    var result =
        //    UtilGUI.OpenFileDialogToGetNameAndContent(
        //        "NN++ parameter files (*.nnpparam)|*.nnpparam|All files (*.*)|*.*",
        //        "Choose a parameter file..."
        //    );

        //    if (result.success == false) return;

        //    NnPlanData? plan;
        //    if ((plan = projectData?.AddPlan(
        //            $"{selectedTemplateId} - {result.name}",
        //            selectedTemplateId,
        //            result.content
        //        )) != null)
        //    {
        //        SelectedPlan = plan;
        //        SelectionMode = SelectionModes.Plan;
        //    }
        //}

        public ICommand CommandAddPlanEmpty =>
            new RelayCommand(
                AddPlanEmptyExecute,
                () => IsProjectLoaded() && (selectedTemplate != null));
        void AddPlanEmptyExecute()
        {
            if (selectedTemplate == null) return;

            NnPlanData? plan;
            if ((plan = projectData?.AddPlan(
                    $"{selectedTemplate.Id}",
                    selectedTemplate
                )) != null)
            {
                SelectedPlan = plan;
                SelectionMode = SelectionModes.Plan;
            }
        }

        public ICommand CommandDeletePlan =>
            new RelayCommand(
                DeletePlanExecute,
                () => IsProjectLoaded() && (selectedPlan != null));
        void DeletePlanExecute()
        {
            if (selectedPlan == null) return;

            if (projectData?.DeletePlan(selectedPlan) ?? false)
            {
                SelectedPlan = null;
            }
        }

        public ICommand CommandAddTask =>
            new RelayCommand(
                AddTaskExecute,
                () => 
                    IsProjectLoaded() && 
                    (selectedPlan != null) && 
                    (param != null) && 
                    (SelectionMode != SelectionModes.Template)
            );
        void AddTaskExecute()
        {
            if ((param == null)) return;
            NnTaskData? task;
            if ((task = selectedPlan?.AddTask(param)) != null) {
                SelectedTask = task;
                SelectionMode = SelectionModes.Task;
            }
        }

        public ICommand CommandAddTaskFromFile =>
            new RelayCommand(
                AddTaskFromFileExecute,
                () =>
                    IsProjectLoaded() &&
                    (selectedPlan != null) &&
                    (SelectionMode != SelectionModes.Template)
            );
        void AddTaskFromFileExecute()
        {
            var (fileOpened, _, content) =
                UtilGUI.OpenFileDialogToGetNameAndContent(
                    "NN++ template files (*.nnptmpl)|*.nnptmpl|All files (*.*)|*.*",
                    "Choose a parameter file..."
                );

            if (fileOpened == false)
                return;

            selectedPlan?.AddTaskFromFile(content);
            SelectionMode = SelectionModes.Plan;
        }

        public ICommand CommandDeleteTask => 
            new RelayCommand(
                DeleteTaskExecute,
                () => IsProjectLoaded() && (selectedPlan != null) && (selectedTask != null));
        void DeleteTaskExecute()
        {
            if ((selectedTask == null)) return;
            if (selectedPlan?.DeleteTask(selectedTask) ?? false) {
                SelectedTask = null;
            }
        }

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
                () => projectData?.ToggleScheduler(),
                () => IsProjectLoaded());

        public ICommand CommandEnqueueModule =>
            new RelayCommand(
                EnqueueModuleExecute,
                () =>
                    IsProjectLoaded() &&
                    (
                        ((selectedPlan != null) && SelectionMode == SelectionModes.Plan) ||
                        ((selectedTask != null) && SelectionMode == SelectionModes.Task)
                    ) &&
                    (module != null));
        void EnqueueModuleExecute()
        {
            if (module == null) return;

            switch (SelectionMode)
            {
                case SelectionModes.Plan:
                    if (selectedPlan == null) return;
                    selectedPlan.QueueModule(module);
                    break;

                case SelectionModes.Task:
                    if (selectedPlan == null) return;
                    if (selectedTask == null) return;
                    selectedTask.QueueModule(module);
                    break;

                default:
                    return;
            }
        }

        public ICommand CommandClearModules => 
            new RelayCommand(
                ClearModulesExecute,
                () =>
                    IsProjectLoaded() &&
                    (
                        ((selectedPlan != null) && SelectionMode == SelectionModes.Plan) ||
                        ((selectedTask != null) && SelectionMode == SelectionModes.Task)
                    ));
        void ClearModulesExecute()
        {
            switch (SelectionMode)
            {
                case SelectionModes.Plan:
                    if (selectedPlan == null) return;
                    selectedPlan.ClearModules();
                    break;

                case SelectionModes.Task:
                    if (selectedPlan == null) return;
                    if (selectedTask == null) return;
                    selectedTask.ClearModules();
                    break;

                default:
                    return;
            }
        }
    }
}
