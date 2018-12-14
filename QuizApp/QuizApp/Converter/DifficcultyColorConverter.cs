using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace QuizApp.Converter
{
    public class DifficultyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var difficulty = ((string)value).ToLower();
                if (difficulty.Equals("easy"))
                    return Color.LightGreen;
                else if (difficulty.Equals("medium"))
                    return Color.Black;
                else if (difficulty.Equals("hard"))
                    return Color.Red;
                else
                    return Color.Black;
            }
            return Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
