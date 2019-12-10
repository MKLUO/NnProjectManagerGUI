using System.Collections.Generic;
using System.Linq;

//using NnManager;
using System.Collections.ObjectModel;

using NNMCore;
using NNMCore.View;
using System.Runtime.CompilerServices;

//#nullable enable

namespace NnManagerGUI.ViewModel {
    //partial class ProjectViewModel : Notifier, INotifyPropertyChanged
    partial class ProjectViewModel : View.Utils.Notifier {

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

                //{"Log", new List<string>{
                //    "TextLog"}},

                //{"SchedulerActiveFlag", new List<string>{
                //    "TextSchedularStatus"}},

                //{"Model - AddTemplate", new List<string>{
                //    "CollectionTemplate"}},

                //{"Model - DeleteTemplate", new List<string>{
                //    "CollectionTemplate"}},

                //{"Model - AddPlan", new List<string>{
                //    "CollectionPlan"}},

                //{"Model - DeletePlan", new List<string>{
                //    "CollectionPlan"}},


                //{"Plan - AddTask", new List<string>{
                //    "CollectionTask"} },

                //{"Plan - DeleteTask", new List<string>{
                //    "CollectionTask"}},

                #endregion

                // View Events

                {"SelectedPlan", new List<string>{
                    "CollectionTask"} },

                {"SelectedTask", new List<string>{
                    "CollectionModulePallete",
                    "CollectionModule"}},

                //{"SelectedModulePallete", new List<string>{
                //    "Module"}},

                //{"SelectedModule", new List<string>{
                //    "Module"}},
                
                {"ParamDiffOnly", new List<string>{
                    "TemplateParamsForm"}}
            };

        protected override List<string> Minors => new List<string>{
            "TextSchedulerStatus",
            "TextEnqueueModuleButton",
            "TextDequeueModuleButton"};

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

        void Update<T>(
            ref ObservableCollection<T> collection,
            IEnumerable<T> newDatas) where T : class {
            collection = new ObservableCollection<T>(newDatas);
        }

        TIEntry? FindData<TIEntry>(
            TIEntry? oldData,
            IEnumerable<TIEntry> newCollection,
            bool selectFirstIfNull = false) where TIEntry : class {

            var returnEntry = newCollection.FirstOrDefault(data => data.Equals(oldData));

            if (selectFirstIfNull && returnEntry == null)
                returnEntry = newCollection.FirstOrDefault();

            return returnEntry;
        }
    }
}
