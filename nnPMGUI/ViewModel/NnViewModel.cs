using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using NNMCore.Service;
using NNMCore.NN;


namespace NnManagerGUI.ViewModel {

    using NNMCore.View;
    using NNMCore.NN.View;

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
                {nameof(SelectedPlan), new List<string>{
                    nameof(CollectionTask)} },

                {nameof(SelectedTask), new List<string>{
                    nameof(CollectionModulePallete),
                    nameof(CollectionModuleVM)}},

                {nameof(SelectedModuleVM), new List<string>{
                    nameof(CollectionModuleVM)}},

                {nameof(ParamDiffOnly), new List<string>{
                    nameof(TemplateParamsForm)}}
            };

        protected override List<string> Minors => new List<string>{
            nameof(TextSchedulerStatus),
            nameof(TextEnqueueModuleButton),
            nameof(TextExecutionSelectionMode),
            nameof(TextClearModulesButton)
        };


        //bool OnWarnAndDecide(Util.WarnAndDecideEventArgs e) {
        //    return UtilGUI.WarnAndDecide(e.Text);
        //}

        //void OnError(Util.ErrorEventArgs e) {
        //    UtilGUI.Error(e.Text);
        //}


    }

    partial class ProjectViewModel {
        static void Update<T>(
            ObservableCollection<T> collection,
            IEnumerable<T> newDatas) where T : class, IUpdate {
            // === 3-step update ===

            // 1. Remove
            List<T> removing =
                collection.Except(newDatas).ToList();
            foreach (var data in removing)
                collection.Remove(data);

            // 2. New
            List<T> adding =
                newDatas.Except(collection).ToList();
            foreach (var data in adding)
                collection.Add(data);

            // 3. Update
            foreach (var data in collection)
                data.Update();
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
