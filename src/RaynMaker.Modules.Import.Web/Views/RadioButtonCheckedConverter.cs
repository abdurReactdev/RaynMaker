﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace RaynMaker.Modules.Import.Web.Views
{
    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value.Equals( parameter );
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value.Equals( true ) ? parameter : Binding.DoNothing;
        }
    }
}