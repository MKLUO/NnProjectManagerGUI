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

        void OnProjectPropertyChange(object sender, PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("TemplateCollection"));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("TemplateInfoCollection"));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("TaskCollection"));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("QueuedTaskCollection"));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("SchedularStatus"));
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs("LogText"));
        }

        void OnProjectPropertyChange() {
            OnProjectPropertyChange(this, new PropertyChangedEventArgs(""));
        }

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
