using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PosSystem.App.Helpers
{
    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DependencyObject item)
            {
                DataGrid grid = ItemsControl.ItemsControlFromItemContainer(item) as DataGrid;
                if (grid != null)
                {
                    int index = grid.ItemContainerGenerator.IndexFromContainer(item) + 1;
                    return index;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}