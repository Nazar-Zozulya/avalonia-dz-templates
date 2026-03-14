

using Avalonia;
using Avalonia.Media;

namespace avalonia_dz_templates.Helpers
{
    public static class WeatherBackgroundHelper
    {
        public static IBrush GetGradientWeather(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            string desc = description.ToLower();

            string startColorHex = "#87CEFA";
            string endColorHex = "#FFDF56";

            if (desc.Contains("дощ") || desc.Contains("rain") || desc.Contains("мряка") || desc.Contains("drizzle"))
            {
                startColorHex = "#808080";
                endColorHex = "#5DADE2";
            }
            else if (desc.Contains("сніг") || desc.Contains("snow"))
            {
                startColorHex = "#FFF";
                endColorHex = "#80C4DE";
            }

            else if (desc.Contains("хмар"))
            {
                startColorHex = "#6C6C6C";
                endColorHex = "#C0C0C0";
            }

            return new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse(startColorHex), 0.05),
                    new GradientStop(Color.Parse(endColorHex), 1),
                }

            };

        }
    }
}