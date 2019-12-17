using NNMCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace NnManagerGUI.Converter {
    using NNMCore.View;
    public class ModuleStatusToBrushConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is ModuleStatus status))
                return Brushes.White;

            switch (status) {
                case ModuleStatus.ParentError:
                    return Brushes.Orange;
                case ModuleStatus.Error:
                    return Brushes.Red;
                case ModuleStatus.NotReady:
                    return Brushes.LightYellow;
                case ModuleStatus.Ready:
                    return Brushes.LightGreen;
                case ModuleStatus.Done:
                    return Brushes.Black;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

    public class BooleanToBrushConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is bool boolean))
                return Brushes.White;

            if (boolean)
                return Brushes.Black;
            else
                return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

    class EnumEntryVM {
        public EnumEntryVM(object value) {
            Value = value;
        }
        public object Value { get; }
        public string Description => Value.ToString() ?? "-";
    }

    static class EnumHelper {
        public static IEnumerable<EnumEntryVM> GetAllValuesAndDescriptions(Type t) {
            return Enum.GetValues(t).Cast<object>()
                .Select(val => new EnumEntryVM(val)).ToList();
        }
    }

    public class EnumToCollectionConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return EnumHelper.GetAllValuesAndDescriptions(
                (value?.GetType()) ?? typeof(Enum));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
