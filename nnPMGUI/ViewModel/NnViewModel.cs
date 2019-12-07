using System.Collections.Generic;
using System.Linq;

//using NnManager;
using System.Collections.ObjectModel;

using NNMCore;
using NNMCore.View;

//#nullable enable

namespace NnManagerGUI.ViewModel {
    //partial class ProjectViewModel : Notifier, INotifyPropertyChanged
    partial class ProjectViewModel : Notifier {

        INNManager Manager { get; }

        public ProjectViewModel() {
            //Util.Warning += OnWarnAndDecide;
            //Util.Error += OnError;            
            Manager = Core.GetManager();
            Subscribe(Manager);
        }

        protected override Dictionary<string, List<string>> Derivatives =>
            new Dictionary<string, List<string>>
            {
                #region Model Events

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

                #endregion

                // View Events

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
                    "Module",
                    "CollectionParam",
                    "CollectionModuleQueue",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"SelectedModule", new List<string>{
                    "Module",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"SelectedModuleQueue", new List<string>{
                    "Module",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"SelectionMode", new List<string>{
                    "CollectionParam",
                    "TextEnqueueModuleButton",
                    "TextDequeueModuleButton"}},

                {"ModuleSelectionMode", new List<string>{
                    "Module"}}
            };

        public bool IsBusy() => Manager.IsBusy();

        //bool OnWarnAndDecide(Util.WarnAndDecideEventArgs e) {
        //    return UtilGUI.WarnAndDecide(e.Text);
        //}

        //void OnError(Util.ErrorEventArgs e) {
        //    UtilGUI.Error(e.Text);
        //}

        static void Update<T>(
            ObservableCollection<T> collection,
            IEnumerable<T> newDatas) where T : class {
            // === 3-step update ===
            // 1. Update (Done automatically by ObservableCollection)

            // 2. Remove
            List<T> removing = 
                collection.Except(newDatas).ToList();
            foreach (var data in removing)
                collection.Remove(data);

            // 3. New
            List<T> adding =
                newDatas.Except(collection).ToList();
            foreach (var data in adding)
                collection.Add(data);
        }

        IEntry FindData(IEntry oldData, IEnumerable<IEntry> newCollection) {
            if (oldData.IsNull())
                return new NullEntry();

            foreach (var newData in newCollection)
                if (newData.Equals(oldData))
                    return newData;

            return new NullEntry();
        }
    }
}
