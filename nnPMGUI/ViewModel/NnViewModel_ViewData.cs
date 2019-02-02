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
        public List<string> TemplateCollection
        {
            get {
                if (project != null)
                    return project.GetTemplates();
                else
                    return null;
            }
        }

        string selectedTemplateId;
        public string SelectedTemplateId
        {
            get { return selectedTemplateId; }
            set {
                if (value != selectedTemplateId) {
                    selectedTemplateId = value;
                    PropertyChanged?.Invoke(this, 
                        new PropertyChangedEventArgs("SelectedTemplateId"));
                    PropertyChanged?.Invoke(this, 
                        new PropertyChangedEventArgs("TemplateInfoCollection"));
                    
                    SetNewParamCollection(
                        project.GetTemplateParam(selectedTemplateId)
                    );
                }
            }
        }

        //public List<string> TemplateInfoCollection
        //{
        //    get {
        //        if (SelectedTemplateId != null)
        //            return project.GetTemplateInfo(SelectedTemplateId);
        //        else
        //            return null;
        //    }
        //}


        //string selectedTemplateInfoId;
        //public string SelectedTemplateInfoId
        //{
        //    get { return selectedTemplateInfoId; }
        //    set {
        //        if (value != selectedTemplateInfoId) {
        //            selectedTemplateInfoId = value;
        //        }
        //    }
        //}

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
        public List<Param> ParamCollection 
        {
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

            PropertyChanged?.Invoke(this, 
                new PropertyChangedEventArgs("ParamCollection"));
        }

        public void UpdateParamCollection(Param param)
        {
            var obj = paramCollection.FirstOrDefault(x => x.Name == param.Name);
            obj.Value = param.Value;

            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs("ParamCollection"));
        }

        public class Task
        {
            public Task(string name, string status)
            {
                this.Name = name;
                this.Status = status;
            }

            public string Name {
                get;
                set;
            }
            public string Status {
                get;
                set;
            }
        }

        public List<Task> TaskCollection
        {
            get {
                if (project != null) {
                    List<Task> taskCollection = new List<Task>();

                    var taskStatuss = project.GetTasksStatus();
                    foreach (var taskStatus in taskStatuss) {
                        taskCollection.Add(
                            new Task(
                                taskStatus.Item1,
                                taskStatus.Item2
                            )
                        );
                    }

                    return taskCollection;
                } else
                    return null;
            }
        }

        List<Task> selectedTasks;
        public List<Task> SelectedTasks
        {
            get { return selectedTasks; }
            set {
                if (value != selectedTasks) {
                    selectedTasks = value;

                    //SetNewParamCollection(
                    //    project.GetTaskParam(selectedTask.Item1)
                    //);
                }
            }
        }

        public List<string> QueuedTaskCollection
        {
            get {
                if (project != null) {
                    return project.GetQueuedTasksInfo();
                } else
                    return null;
            }
        }

        public string SchedularStatus
        {
            get {
                if (project != null) {
                    return project.GetSchedulerStatus();
                } else
                    return null;
            }
        }

        public string LogText {
            get {
                return Util.GetLog();                
            }
        }
    }
}
