
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NnManagerGUI.ViewModel {

    class NamedForm<T> {
        public string Key { get; }
        public T DefaultValue => Form.DefaultValue;
        T ActualValue => Form.Value;
        public T Value {
            get {
                return (DefaultValue?.Equals(ActualValue) ?? default) ? default : ActualValue;
            }
            set {
                Form.Value = value;
            }
        }
        NNMCore.View.IParamForm<T> Form { get; }
        public NamedForm(string key, NNMCore.View.IParamForm<T> form) {
            Key = key;
            Form = form;
        }
    }

    class ParamsFormVM {
        NNMCore.View.IParamsForm Form { get; }
        public ParamsFormVM(NNMCore.View.IParamsForm form) => Form = form;

        IList<NamedForm<T>> Transform<T>(
            IImmutableDictionary<string, NNMCore.View.IParamForm<T>> dict) => 
            dict.OrderBy(kvp => kvp.Key)
            .Select(kvp => new NamedForm<T>(kvp.Key, kvp.Value)).ToList();

        public IList<NamedForm<string>> Texts => Transform(Form.TextsDict);
        public IList<NamedForm<int>> Ints => Transform(Form.IntsDict);
        public IList<NamedForm<double>> Floats => Transform(Form.FloatsDict);
        public IList<NamedForm<bool>> Booleans => Transform(Form.BooleansDict);
        public IList<NamedForm<object>> Enums =>
            Form.EnumsDict.OrderBy(kvp => kvp.Key)
            .Select(kvp => new NamedForm<object>(kvp.Key, kvp.Value)).ToList();
    }
}
