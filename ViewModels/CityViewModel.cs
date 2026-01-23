using System;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace avalonia_dz_templates.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        private string _name;

        private int _temperature;

        public int MaxTemp { get; set; }

        public int MinTemp { get; set; }

        public string Description { get; set; }

        private Bitmap _img;

        private ObservableCollection<HourlyForecastViewModel> _hourlyForecasts;
        
        
        
        
        
        

        public Bitmap Img
        {
            get => _img;
            set => this.RaiseAndSetIfChanged(ref _img, value);
        }
        
        public ObservableCollection<HourlyForecastViewModel> HourlyForecasts
        {
            get => _hourlyForecasts;
            set => this.RaiseAndSetIfChanged(ref _hourlyForecasts, value);
        }

        public CityViewModel(string name, int temperature, int minTemp, int maxTemp, string description, string imgPath)
        {
            Name = name;
            Temperature = temperature;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            Description = description;
            // Img = LoadImg(imgPath);

            HourlyForecasts = new();
            Img = LoadImageSafe(imgPath);

            GenerateForecast(Temperature);


        }

        private void GenerateForecast(int baseTemp)
        {
            DateTime nextHour = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1);

            for (int i = 0; i < 24; i++)
            {
                DateTime forecastTime = nextHour.AddHours(i);
                string timeDisplay = forecastTime.ToString("HH");
                int t = baseTemp + new Random().Next(-3, 4);
                
                
                string iconPath = (i % 2 == 0 ) ? "avares://avalonia_dz_templates/Assets/cloudy" : "avares://avalonia_dz_templates/Assets/sun.png";
                
                HourlyForecasts.Add(new HourlyForecastViewModel(timeDisplay, t, iconPath));
            }
        }

        private Bitmap? LoadImageSafe(string path)
        {
            try
            {
                return new Bitmap(AssetLoader.Open(new Uri(path)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public int Temperature
        {
            get => _temperature;
            set => this.RaiseAndSetIfChanged(ref _temperature, value);
        }

    }
}

