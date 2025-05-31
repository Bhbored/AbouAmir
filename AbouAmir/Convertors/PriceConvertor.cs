using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AbouAmir.Convertors
{
    public class PriceConvertor : IValueConverter
    {
        // Convert from double to display as string like "120000"
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double price)
            {
                return (price * 1000).ToString(culture); // Convert 120 -> "120000"
            }
            return null;
        }

        // Convert back from "120000" string to 120000 as double
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string priceString)
            {
                priceString = priceString.Replace(" ", "").Replace(",", "");

                if (double.TryParse(priceString, NumberStyles.Number, culture, out double result))
                {
                    return result; 
                }
            }
            return null;
        }
    }
}
