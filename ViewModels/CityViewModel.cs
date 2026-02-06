using System;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using System.Text.Json.Serialization;

namespace avalonia_dz_templates.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        
        private string _name = "";
        private string _description = "";
        private int _temperature;
        private int _maxTemp;
        private int _minTemp;
        private int _humidity;
        private int _windSpeed;
        private int _timezoneOffsetSeconds;
        private string _imagePath = "";
        private Bitmap? _image;
        private ObservableCollection<HourlyForecastViewModel> _hourlyForecasts = new();

        // --- ВЛАСТИВОСТІ З ПОВІДОМЛЕННЯМ (RaiseAndSetIfChanged) ---

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value); // Тепер екран оновиться!
        }

        public int Temperature
        {
            get => _temperature;
            set => this.RaiseAndSetIfChanged(ref _temperature, value);
        }

        public int MaxTemp
        {
            get => _maxTemp;
            set => this.RaiseAndSetIfChanged(ref _maxTemp, value);
        }

        public int MinTemp
        {
            get => _minTemp;
            set => this.RaiseAndSetIfChanged(ref _minTemp, value);
        }

        public int Humidity
        {
            get => _humidity;
            set => this.RaiseAndSetIfChanged(ref _humidity, value);
        }

        public int WindSpeed
        {
            get => _windSpeed;
            set => this.RaiseAndSetIfChanged(ref _windSpeed, value);
        }

        public int TimezoneOffsetSeconds
        {
            get => _timezoneOffsetSeconds;
            set => this.RaiseAndSetIfChanged(ref _timezoneOffsetSeconds, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => this.RaiseAndSetIfChanged(ref _imagePath, value);
        }

        [JsonIgnore]
        public Bitmap? WeatherImage
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        [JsonIgnore]
        public ObservableCollection<HourlyForecastViewModel> HourlyForecasts
        {
            get => _hourlyForecasts;
            set => this.RaiseAndSetIfChanged(ref _hourlyForecasts, value);
        }

        // --- КОНСТРУКТОРИ ---

        public CityViewModel() 
        { 
            // Ініціалізація для JSON
        }

        public CityViewModel(string name, int temp, string desc, int max, int min, string imagePath, int timezoneOffsetSeconds)
        {
            Name = name;
            Temperature = temp;
            Description = desc;
            MaxTemp = max;
            MinTemp = min;
            ImagePath = imagePath;
            TimezoneOffsetSeconds = timezoneOffsetSeconds;

            WeatherImage = LoadImageSafe(imagePath);
            GenerateForecast(Temperature);
        }

        // --- МЕТОДИ ---

        public void RestoreImage()
        {
            if (!string.IsNullOrEmpty(ImagePath)) WeatherImage = LoadImageSafe(ImagePath);
            if (HourlyForecasts.Count == 0) GenerateForecast(Temperature);
        }

        private void GenerateForecast(int baseTemp)
        {
            HourlyForecasts.Clear();
            var nextHour = DateTime.Now.Date.AddHours(DateTime.Now.Hour + 1);
            var rnd = new Random();

            for (int i = 0; i < 24; i++)
            {
                var time = nextHour.AddHours(i).ToString("HH");
                int t = baseTemp + rnd.Next(-3, 4);
                string icon = (i % 2 == 0) ? "avares://WeatherApp/Assets/cloud.png" : "avares://WeatherApp/Assets/sun.png";
                HourlyForecasts.Add(new HourlyForecastViewModel(time, t, icon));
            }
        }
        
private Bitmap? LoadImageSafe(string path)
        {
            try { return new Bitmap(AssetLoader.Open(new Uri(path))); } catch { return null; }
        }
    }
}