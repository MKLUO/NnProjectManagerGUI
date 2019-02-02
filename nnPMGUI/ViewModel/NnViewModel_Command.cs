using System;
using System.Collections.Generic;
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
            return project.IsSchedulerRunning();
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
            string path = UtilGUI.OpenFileDialogToGetFolder();
            if (path == null) return;

            Project newProject = Project.NewProject(path);
            if (newProject == null) return;

            project = newProject;
            project.PropertyChanged += OnProjectPropertyChange;
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

            Project newProject = Project.LoadProject(path);
            if (newProject == null) return;

            project = newProject;
            project.PropertyChanged += OnProjectPropertyChange;
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

            project.AddTemplate(id, content);
            OnProjectPropertyChange();
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

            project.AddTask(
                SelectedTemplateId,
                paramDict);

            OnProjectPropertyChange();
        }

        public ICommand StartQueue {
            get {
                return new RelayCommand(
                    StartQueueExecute, 
                    () => !IsQueueRunning());
            }
        }

        void StartQueueExecute()
        {
            project.StartScheduler();
            OnProjectPropertyChange();
        }

        public ICommand StopQueue {
            get {
                return new RelayCommand(
                    StopQueueExecute, 
                    IsQueueRunning);
            }
        }

        void StopQueueExecute()
        {
            project.StopScheduler();
            OnProjectPropertyChange();
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
            project.EnqueueTask(SelectedTask.Item1);
            OnProjectPropertyChange();
        }

        public ICommand LoadParamFromTask {
            get {
                return new RelayCommand(
                    LoadParamFromTaskExecute,
                    () => IsProjectLoaded() && (SelectedTask != null));
            }
        }

        void LoadParamFromTaskExecute()
        {
            SelectedTemplateId = 
                (string)
                project.GetTaskInfo(selectedTask.Item1)["templateId"];

            SetNewParamCollection(
                (Dictionary<string, (string, string)>)
                project.GetTaskInfo(selectedTask.Item1)["param"]
            );

            OnProjectPropertyChange();
        }
    }
}
