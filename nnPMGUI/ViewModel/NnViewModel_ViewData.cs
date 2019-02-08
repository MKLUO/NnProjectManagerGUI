using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;

using NnManager;

namespace NnManagerGUI.ViewModel
{
    partial class ProjectViewModel
    {
        void ResetSelections()
        {
            SelectedTask = null;
            SelectedTemplateId = null;
        }

        public List<string> TemplateCollection {
            get {
                if (project != null)
                    return project.GetTemplates();
                else
                    return null;
            }
        }

        string selectedTemplateId;
        public string SelectedTemplateId {
            get { return selectedTemplateId; }
            set {
                if (value != selectedTemplateId) {
                    selectedTemplateId = value;
                    OnPropertyChange("SelectedTemplateId");

                    SetNewParamCollection(
                        project.GetTemplateInfo(selectedTemplateId)
                    );
                }
            }
        }


        public class Param
        {
            public Param(string name, string value, string defaultValue)
            {
                this.Name = name;
                this.Value = value;
                this.DefaultValue = defaultValue;
            }

            public string Name {
                get;
                set;
            }
            public string Value {
                get;
                set;
            }
            public string DefaultValue {
                get;
                set;
            }
        }

        List<Param> paramCollection;
        public List<Param> ParamCollection {
            get {
                return paramCollection;
            }
            set {
                if (!paramCollection.SequenceEqual(value))
                    paramCollection = value;
            }
        }


        void SetNewParamCollection(Dictionary<string, (string, string)> param)
        {
            paramCollection = new List<Param>();
            foreach (var info in param)
                paramCollection.Add(
                    new Param(info.Key, info.Value.Item1, info.Value.Item2)
                );

            OnPropertyChange("ParamCollection");
        }

        public void UpdateParamCollection(Param param)
        {
            var obj = paramCollection.FirstOrDefault(x => x.Name == param.Name);
            obj.Value = param.Value;

            OnPropertyChange("ParamCollection");
        }

        public class Task : INotifyPropertyChanged
        {
            public Task(
                string name, 
                string runningModule, 
                string queuedModule, 
                string status)
            {
                this.Name = name;
                this.RunningModule = runningModule;
                this.QueuedModule = queuedModule;
                this.Status = status;
            }

            string name;
            public string Name {
                get {
                    return name;
                }
                set {
                    if (value != name) {
                        name = value;
                        OnPropertyChange("Name");
                    }
                }
            }
            string runningModule;
            public string RunningModule {
                get {
                    return runningModule;
                }
                set {
                    if (value != runningModule) {
                        runningModule = value;
                        OnPropertyChange("RunningModule");
                    }
                }
            }
            string queuedModule;
            public string QueuedModule {
                get {
                    return queuedModule;
                }
                set {
                    if (value != queuedModule) {
                        queuedModule = value;
                        OnPropertyChange("QueuedModule");
                    }
                }
            }

            string status;
            public string Status {
                get {
                    return status;
                }
                set {
                    if (value != status) {
                        status = value;
                        OnPropertyChange("Status");
                    }
                }
            }

            public void Update((string, string, string) updateData)
            {
                RunningModule = updateData.Item1;
                QueuedModule = updateData.Item2;
                Status = updateData.Item3;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChange(string arg)
            {
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(arg));
            }
        }

        ObservableCollection<Task> taskCollection;
        public ObservableCollection<Task> TaskCollection {
            get {
                if (project == null)
                    return null;

                if (taskCollection == null)
                    taskCollection = new ObservableCollection<Task>();

                var newtaskStatus = project.GetTasks();

                // 3-step update
                // 1. Update
                foreach (Task task in taskCollection)
                    if (newtaskStatus.ContainsKey(task.Name))
                        task.Update(newtaskStatus[task.Name]);

                // 2. Remove
                List<Task> removing =
                    taskCollection
                        .Where(x => !newtaskStatus.ContainsKey(x.Name)).ToList();
                foreach (Task task in removing)
                    taskCollection.Remove(task);

                // 3. New
                List<KeyValuePair<string, (string, string, string)>> adding =
                    newtaskStatus
                        .Where(
                            x => taskCollection
                                .FirstOrDefault(
                                    y => x.Key == y.Name
                                ) == null
                        ).ToList();

                foreach (var task in adding)
                    taskCollection.Add(
                        new Task(
                            task.Key,
                            task.Value.Item1,
                            task.Value.Item2,
                            task.Value.Item3
                        )
                    );
                
                return taskCollection;
            }
            private set {}
        }

        Task GetTaskByName(string id)
        {
            //foreach (var item in TaskCollection)
            //    if (item.Name == id)
            //        return item;
            //return null;

            return taskCollection.FirstOrDefault(x => x.Name == id);
        }

        Task selectedTask;
        public Task SelectedTask {
            get { return selectedTask; }
            set {
                if (value != selectedTask) {
                    selectedTask = value;
                    OnPropertyChange("SelectedTask");
                }
            }
        }

        public string SchedularStatus {
            get {
                if (project == null)
                    return "";

                if (project.IsSchedularRunning())
                    return "Active";
                else
                    return "Inactive";
            }
        }

        public string LogText {
            get {
                return project?.Log;
            }
        }
    }
}
