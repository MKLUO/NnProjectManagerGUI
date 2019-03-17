using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using NnManager;
using System.Windows.Controls;
using System.Collections.ObjectModel;

#nullable enable

namespace NnManagerGUI.ViewModel
{
    partial class ProjectViewModel : Notifier, INotifyPropertyChanged
    {
        NnProjectData? projectData;

        public ProjectViewModel()
        {
            Util.Warning += OnWarnAndDecide;
            Util.Error += OnError;
        }

        protected override Dictionary<string, List<string>>? Derivatives =>
            new Dictionary<string, List<string>>
            {
                // Model Events

                {"Log", new List<string>{
                    "TextLog"}},

                {"SchedulerActiveFlag", new List<string>{
                    "TextSchedularStatus"}},

                {"Model - AddTemplate", new List<string>{
                    "CollectionTemplate"}},

                {"Model - DeleteTemplate", new List<string>{
                    "CollectionTemplate"}},

                {"Model - AddPlan", new List<string>{
                    "CollectionPlan"}},

                {"Model - DeletePlan", new List<string>{
                    "CollectionPlan"}},


                {"Plan - AddTask", new List<string>{
                    "CollectionTask"} },

                {"Plan - DeleteTask", new List<string>{
                    "CollectionTask"}},

                // View Events

                //{"ParamMode", new List<string>{
                //    "Param"}},

                {"SelectedTemplate", new List<string>{
                    "CollectionParam"
                } },

                {"SelectedPlan", new List<string>{
                    "CollectionParam",
                    "CollectionTask",
                    "CollectionModule",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"} },

                {"SelectedTask", new List<string>{
                    "CollectionParam",
                    "CollectionModule",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"SelectedModule", new List<string>{
                    "Module",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"SelectionMode", new List<string>{
                    "CollectionParam",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}}
            };

        public bool IsBusy()
        {
            if (!IsProjectLoaded())
                return false;
            else
                return projectData?.IsBusy ?? false;
        }

        bool OnWarnAndDecide(Util.WarnAndDecideEventArgs e)
        {
            return UtilGUI.WarnAndDecide(e.Text);
        }

        void OnError(Util.ErrorEventArgs e)
        {
            UtilGUI.Error(e.Text);
        }

        static void Update<T>(
            ObservableCollection<T> collection, 
            IEnumerable<T> newDatas) where T : class, NnProjectData.IRefCompare<T>
        {
            // === 3-step update ===
            // 1. Update (Done automatically by ObservableCollection)

            // 2. Remove
            List<T> removing =
                collection
                    .Where(x => newDatas.Where(y => x.HasSameRef(y)).Count() == 0).ToList();
            foreach (var data in removing)
                collection.Remove(data);

            // 3. New
            List<T> adding =
                newDatas
                    .Where(x => collection.Where(y => x.HasSameRef(y)).Count() == 0).ToList();
            foreach (var data in adding)
                collection.Add(data);
        }

        T? FindData<T>(T? oldData, IEnumerable<T>? newCollection) where T : class, NnProjectData.IRefCompare<T>
        {
            if (oldData == null)
                return null;

            if (newCollection == null)
                return null;

            foreach (var newData in newCollection)
                if (newData.HasSameRef(oldData))
                    return newData;

            return null;
        }

    }
}
