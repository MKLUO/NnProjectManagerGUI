using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace NnManagerGUI.View.Utils {
    class Notifier : INotifyPropertyChanged, NNMCore.View.IUpdate {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual Dictionary<string, List<string>> Derivatives { get; } = new Dictionary<string, List<string>> { };

        protected virtual List<string> Minors { get; } = new List<string> { };

        protected void Subscribe(INotifyPropertyChanged target) =>
            target.PropertyChanged += OnComponentPropertyChanged;
        public void Update() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));

        protected void OnPropertyChanged(Expression<Func<object>> objExpr) =>
            OnPropertyChanged(((MemberExpression)objExpr.Body).Member.Name);

        protected void OnPropertyChanged(string str) {
            OnPropertyChanged(new PropertyChangedEventArgs(str));

            if (Derivatives?.ContainsKey(str) ?? false)
                foreach (var derivative in Derivatives?[str] ?? (new List<string>()))
                    OnPropertyChanged(derivative);
            foreach (var minor in Minors)
                OnPropertyChanged(new PropertyChangedEventArgs(minor));
        }

        void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChanged?.Invoke(this, e);
        }

        void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(e.PropertyName);
        }

        protected bool SetField<T>(
            ref T field,
            T value, [CallerMemberName] string? propertyName = null) {

            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;

            if (propertyName != null)
                OnPropertyChanged(propertyName);

            return true;
        }
    }

}
