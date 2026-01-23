using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace avalonia_dz_templates.ViewModels 
{
    public class HourlyForecastViewModel : ViewModelBase
    {
        public string Time { get; set; }
        public int Temprature { get; set; }
        public Bitmap? Icon { get; set; }

        public HourlyForecastViewModel(string time, int temprature, string iconPath)
        {
            Time = time;
            Temprature = temprature;

            if (string.IsNullOrEmpty(iconPath)) return;

            try
            {
                Icon = new Bitmap(AssetLoader.Open(new Uri(iconPath)));
            }
            catch (Exception)
            {
                Icon = null;
            }
        }
    }
}