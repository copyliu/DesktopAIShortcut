using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;
// 添加一个用于模型选择状态转换的转换器
public class ModelSelectionMultiConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        var selectedModels = values[0] as ObservableCollection<string>;
        var modelName = values[1] as string;
        return selectedModels != null && modelName != null && selectedModels.Contains(modelName);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}