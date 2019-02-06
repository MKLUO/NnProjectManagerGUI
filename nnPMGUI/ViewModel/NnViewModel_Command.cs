using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using NnManager;

namespace NnManagerGUI.ViewModel
{
    partial class ProjectViewModel
    {
        #region common_logic

        bool IsProjectLoaded()
        {
            return project != null;
        }

        bool IsQueueRunning()
        {
            if (!IsProjectLoaded()) return false;
            return project.IsSchedularRunning();
        }

        #endregion

        public ICommand NewProject {
            get {
                return new RelayCommand(
                    NewProjectExecute,
                    //() => !IsProjectLoaded()
                    () => true
                );
            }
        }

        void NewProjectExecute()
        {
            //string path = UtilGUI.OpenFileDialogToGetFolder();
            //if (path == null) return;

            string path = UtilGUI.OpenFileDialogToGetPath();

            Project newProject = Project.NewProject(path);
            if (newProject == null) return;

            project = newProject;
            project.PropertyChanged += OnProjectPropertyChange;

            ResetSelections();
            OnProjectPropertyChange();
        }

        public ICommand LoadProject {
            get {
                return new RelayCommand(
                    LoadProjectExecute,
                    //() => !IsProjectLoaded()
                    () => true
                );
            }
        }

        void LoadProjectExecute()
        {
            string path = UtilGUI.OpenFileDialogToGetFolder();
            if (path == null) return;

            //string path = UtilGUI.OpenFileDialogToGetPath(true);

            Project newProject = Project.LoadProject(path);
            if (newProject == null) return;

            project = newProject;
            project.PropertyChanged += OnProjectPropertyChange;

            ResetSelections();
            OnProjectPropertyChange();
        }

        public ICommand SaveProject {
            get {
                return new RelayCommand(
                    SaveProjectExecute,
                    IsProjectLoaded);
            }
        }

        void SaveProjectExecute()
        {
            project.Save();
        }

        public ICommand AddTemplate {
            get {
                return new RelayCommand(
                    AddTemplateExecute,
                    IsProjectLoaded);
            }
        }

        void AddTemplateExecute()
        {
            bool fileOpened;
            string id, content;
            (fileOpened, id, content) =
                UtilGUI.OpenFileDialogToGetNameAndContent(
                    "NN++ template files (*.nnptmpl)|*.nnptmpl|All files (*.*)|*.*"
                );

            if (fileOpened == false) return;

            if (project.AddTemplate(id, content)) {
                OnPropertyChange("TemplateCollection");
                SelectedTemplateId = id;
            }
        }

        public ICommand AddTask {
            get {
                return new RelayCommand(
                    AddTaskExecute,
                    () => IsProjectLoaded() && (SelectedTemplateId != null));
            }
        }

        void AddTaskExecute()
        {
            Dictionary<string, (string, string)> paramDict =
                new Dictionary<string, (string, string)>();

            foreach (var param in paramCollection) {
                if ((param.Value != null) || (param.DefaultValue != null))
                    paramDict[param.Name] = (param.Value, param.DefaultValue);
                else
                    return;
            }

            string id = project.AddTask(
                SelectedTemplateId,
                paramDict);
            
            OnPropertyChange("TaskCollection");
            SelectedTask = GetTaskByName(id);
        }

        public ICommand StartQueue {
            get {
                return new RelayCommand(
                    StartQueueExecute,
                    () => IsProjectLoaded() && !IsQueueRunning());
            }
        }

        void StartQueueExecute()
        {
            project.StartScheduler();
            OnPropertyChange("SchedularStatus");
        }

        public ICommand StopQueue {
            get {
                return new RelayCommand(
                    StopQueueExecute,
                    () => IsProjectLoaded() && IsQueueRunning());
            }
        }

        void StopQueueExecute()
        {
            project.StopScheduler();
            OnPropertyChange("SchedularStatus");
        }

        public ICommand EnqueueTask {
            get {
                return new RelayCommand(
                    EnqueueTaskExecute,
                    () => IsProjectLoaded() && (SelectedTask != null));
            }
        }

        void EnqueueTaskExecute()
        {
            project.EnqueueTaskWithModule(SelectedTask.Name, "Test");
            OnPropertyChange("TaskCollection");
        }

        public ICommand LoadParamFromTask {
            get {
                return new RelayCommand(
                    LoadParamFromTaskExecute,
                    () => {
                        if (SelectedTask == null)
                            return false;
                        else
                            return IsProjectLoaded();
                    }
                );
            }
        }

        void LoadParamFromTaskExecute()
        {
            SelectedTemplateId =
                (string)
                project.GetTaskInfo(selectedTask.Name)["templateId"];

            SetNewParamCollection(
                (Dictionary<string, (string, string)>)
                project.GetTaskInfo(selectedTask.Name)["param"]
            );

            OnPropertyChange("ParamCollection");
        }
    }
}
