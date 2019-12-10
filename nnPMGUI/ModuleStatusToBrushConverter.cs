using NNMCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace NnManagerGUI.Converter {
    class ModuleStatusToBrushConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (!(value is ModuleStatus status))
                return Brushes.White;

            switch (status) {
                case ModuleStatus.ParentError:
                    return Brushes.MediumPurple;
                case ModuleStatus.NotReady:
                    return Brushes.LightYellow;
                case ModuleStatus.Ready:
                    return Brushes.LightGreen;
                case ModuleStatus.Error:
                    return Brushes.Red;
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
}
