using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using NnManager;
using System.Windows.Controls;

namespace NnManagerGUI.ViewModel
{
    partial class ProjectViewModel : INotifyPropertyChanged
    {
        Project project;

        public ProjectViewModel()
        {
            Util.Warning += OnWarnAndDecide;
            Util.Error += OnError;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // ViewDatas that would change when project updates.
        static string[] viewDatas = new string[]
        {
            "TemplateCollection",
            "ParamCollection",
            "TaskCollection", 
            "SchedularStatus",
            "LogText",
        };

        void OnProjectPropertyChange(object sender, PropertyChangedEventArgs e) {
            foreach (string vd in viewDatas)
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(vd));
        }

        void OnProjectPropertyChange() {
            OnProjectPropertyChange(this, new PropertyChangedEventArgs(""));
        }
        void OnPropertyChange(string arg)
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(arg));
        }

        //void OnTaskCollectionChange()
        //{
        //    UpdateTaskCollection();
        //}

        bool OnWarnAndDecide(Util.WarnAndDecideEventArgs e)
        {
            return UtilGUI.WarnAndDecide(e.Text);
        }

        void OnError(Util.ErrorEventArgs e)
        {
            UtilGUI.Error(e.Text);
        }
    }
}
