using NNMCore;
using NNMCore.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NnManagerGUI.ViewModel {
    
    using NNMCore.View;
    using NNMCore.NN.View;

    interface INNModuleEntryVM : INNModuleEntry, IUpdate {
        INNModuleEntry Entry { get; }
        bool IsDep { get; }
        bool Dep { get; set; }
    }

    class NNModuleEntryVM : View.Utils.Notifier, INNModuleEntryVM {
        public INNModuleEntry Entry { get; }
        Func<bool> IsDepEvaluation { get; }
        public bool Dep { get; set; } = false;
        public NNModuleEntryVM(
            INNModuleEntry entry,
            Func<bool> isDepEvaluation) {
            Entry = entry;
            IsDepEvaluation = isDepEvaluation;
            Subscribe(entry);
        }

        public string Title => Entry.Title;
        public string Info => Entry.Info;
        public ModuleStatus Status => Entry.Status;
        public string StatusText => Entry.StatusText;
        public string Summary => Entry.Summary;
        public bool IsDep => IsDepEvaluation();

        public override bool Equals(object? obj) {
            if (!(obj is NNModuleEntryVM anotherEntry)) return false;
            return anotherEntry.Entry.Equals(Entry);
        }
        public override int GetHashCode() {
            return HashCode.Combine(Entry);
        }
    }

    partial class ProjectViewModel {
        ObservableCollection<INNModuleEntryVM> collectionModuleVM = Empty<INNModuleEntryVM>();
        public ObservableCollection<INNModuleEntryVM> CollectionModuleVM {
            get {
                if (!IsProjectLoaded() || SelectedTask == null)
                    collectionModuleVM = Empty<INNModuleEntryVM>();
                else
                    Update(collectionModuleVM, GetModuleVMs());
                return collectionModuleVM;
            }
        }


        INNModuleEntryVM[] GetModuleVMs() {
            if (SelectedTask == null)
                return Empty<INNModuleEntryVM>().AsEnumerable().ToArray();
            var modules = Manager.GetModules(SelectedTask);
            return modules.Select(
                m => new NNModuleEntryVM(m,
                () => {
                    if (SelectedModule == null) return false;
                    return Manager.DoesDependOn(SelectedModule, m);
                })).ToArray();

        }
        static INNModuleEntryVM ToModuleVM(INNModuleEntry entry) =>
            new NNModuleEntryVM(entry, () => false);


        INNModuleEntryVM? selectedModuleVM = null;
        public INNModuleEntryVM? SelectedModuleVM {
            //get => FindData(selectedModule, collectionModule);
            get => selectedModuleVM;
            set {
                SetField(ref selectedModuleVM, FindData(value, collectionModuleVM));
                NewModuleParamsForm();
            }
        }
        public IList<INNModuleEntryVM> SelectedModuleVMs = new List<INNModuleEntryVM> { };


        public INNModuleEntry? SelectedModule {
            get => SelectedModuleVM?.Entry;
            set => SelectedModuleVM = value == null ? null : ToModuleVM(value);
        }
        public IList<INNModuleEntry> SelectedModules =>
            SelectedModuleVMs.Select(vm => vm.Entry).ToList();
    }
}
