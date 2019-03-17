using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;

using NnManager;
using System;

#nullable enable

namespace NnManagerGUI.ViewModel
{
    using NnParamForm = NnProjectData.NnParamForm;
    using NnModuleForm = NnProjectData.NnModuleForm;
    using ModuleType = NnManager.ModuleType;

    using Variable = NnProjectData.Variable;
    using NnPlanData = NnProjectData.NnPlanData;
    using NnTaskData = NnProjectData.NnTaskData;
    using NnTemplateData = NnProjectData.NnTemplateData;

    partial class ProjectViewModel
    {
        NnTemplateData? selectedTemplate;
        public NnTemplateData? SelectedTemplate {
            get =>
                (selectedTemplate == null) ?
                    null :
                    (selectedTemplate = FindData(selectedTemplate, CollectionTemplate));
            set {
                SetField(ref selectedTemplate, FindData(value, CollectionTemplate));
            }
        }

        NnPlanData? selectedPlan;
        public NnPlanData? SelectedPlan {
            get =>
                (selectedPlan == null) ? 
                    null :
                    (selectedPlan = FindData(selectedPlan, planCollection));
            set {
                SetField(ref selectedPlan, FindData(value, planCollection));
            }
        }        

        NnTaskData? selectedTask;
        public NnTaskData? SelectedTask {
            get =>
                (selectedTask == null) ?
                    null :
                    (selectedTask = FindData(selectedTask, taskCollection));
            set {
                SetField(ref selectedTask, FindData(value, taskCollection));
            }
        }

        ModuleType? selectedModule;
        public ModuleType? SelectedModule {
            get =>
                (selectedModule == null) ?
                    null :
                    (CollectionModule?.Contains(selectedModule ?? throw new Exception()) ?? false) ?
                        selectedModule : 
                        null;

            set => SetField(ref selectedModule, value);
        }

        public enum SelectionModes
        {
            Template,
            Plan,
            Task
        }

        SelectionModes? selectionMode;
        public SelectionModes? SelectionMode {
            get => selectionMode;
            set => SetField(ref selectionMode, value);
        }

        NnParamForm? param;
        public NnParamForm? CollectionParam {
            get {
                if (projectData == null)
                    return (param = null);
                if (SelectionMode == null)
                    return (param = null);

                switch (SelectionMode)
                {
                    case SelectionModes.Template:
                        if (SelectedTemplate == null) return null;
                        return (param =
                            SelectedTemplate.GetForm());

                    case SelectionModes.Plan:
                        if (SelectedPlan == null) return null;
                        return (param =
                            SelectedPlan.ParamForm());

                    case SelectionModes.Task:
                        if (SelectedPlan == null) return null;
                        if (SelectedTask == null) return null;
                        return (param =
                            SelectedPlan.ParamForm(SelectedTask));

                    default:
                        return (param = null);
                }
            }
        }

        public void UpdateParamCollection(Variable newVar)
        {
            if (param == null)
                return;

            foreach (Variable oldVar in param.Consts.Concat(param.Variables))
                if (oldVar.Name == newVar.Name) {
                    oldVar.Value = newVar.Value;
                    return;
                }
        }

        NnModuleForm? module;
        public NnModuleForm? Module {
            get {
                if (projectData == null)
                    return (module = null);
                if (selectedModule != null)
                    return (module = new NnModuleForm(selectedModule ?? ModuleType.NnMain));
                else
                    return null;
            }
        }

        public void UpdateModule(Variable newVar)
        {
            if (module == null)
                return;

            foreach (Variable oldVar in module.Options)
                if (oldVar.Name == newVar.Name) {
                    oldVar.Value = newVar.Value;
                    return;
                }
        }

        public List<NnTemplateData>? CollectionTemplate =>
            projectData?.TemplateDatas?.ToList();

        ObservableCollection<NnPlanData>? planCollection;
        public ObservableCollection<NnPlanData> CollectionPlan {
            get {
                if (projectData == null)
                    return (planCollection = new ObservableCollection<NnPlanData>());
                if (planCollection == null)
                    planCollection = new ObservableCollection<NnPlanData>();

                Update(planCollection, projectData.PlanDatas);
                return planCollection;
            }
        }

        ObservableCollection<NnTaskData>? taskCollection;
        public ObservableCollection<NnTaskData> CollectionTask {
            get {
                if ((projectData == null) || (SelectedPlan == null))
                    return (taskCollection = new ObservableCollection<NnTaskData>());
                if (taskCollection == null)
                    taskCollection = new ObservableCollection<NnTaskData>();

                Update(taskCollection, SelectedPlan.TaskDatas);
                return taskCollection;
            }
        }

        public List<ModuleType>? CollectionModule =>
            projectData?.Modules?.ToList();

        public string? TextSchedularStatus {
            get {
                if (projectData == null) return null;
                return projectData.IsSchedularRunning ? "Active" : "Inactive";
            }
        }

        public string TextEnqueueModuleButton {
            get {
                switch (SelectionMode)
                {
                    case SelectionModes.Plan:
                        return "Enqueue (Plan)";

                    case SelectionModes.Task:
                        return "Enqueue (Task)";

                    default:
                        return "Enqueue ...";
                }
            }
        }

        public string TextDequeueModuleButton {
            get {
                switch (SelectionMode) {
                    case SelectionModes.Plan:
                        return "Clear (Plan)";

                    case SelectionModes.Task:
                        return "Clear (Task)";

                    default:
                        return "Clear  ...";
                }
            }
        }

        //public string TextLog =>
        //    projectData?.Log ?? "";
    }
}
