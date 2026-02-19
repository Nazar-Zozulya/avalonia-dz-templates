using System;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Measure;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting;

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


        public List<int> Allah { get; } = new() {1,2,3,4,5,6,7,8,9,10};



        public ISeries[] Series { get; set; } = Array.Empty<ISeries>();
        public Axis[] XAxes { get; set; } = { new Axis() };
        public Axis[] YAxes { get; set; } = { new Axis() };
        public List<int> ForecastFor12H { get; } = new(40) {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40};
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

            


            Series = new ISeries[]
            {
                new ColumnSeries<HourlyForecastViewModel>
                {
                    Values = _hourlyForecasts,
                    Fill = new SolidColorPaint(SKColors.LightBlue),
                    Stroke = null,
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    // Labels = new[] {"12:00", "15:00", "18:00", "21:00", "00:00", "03:00"},
                    IsVisible = false,
                    // Padding = new LiveChartsCore.Drawing.Padding(0, 0, 0, 0)
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {   
                    Position = AxisPosition.End,
                    Labeler = value => $"{value}°C",
                    MinLimit = -20,
                    MaxLimit = 40,
                    MinStep = 5,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    // SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    // ForceStepToMin ,
                }
            };


            WeatherImage = LoadImageSafe(imagePath);
        }

        // --- МЕТОДИ ---

        // public void AddTemperature(double temp)
        // {
        //     _hourlyForecasts.Add(new HourlyForecastViewModel(temp));
        // }


        public void RestoreImage()
        {
            if (!string.IsNullOrEmpty(ImagePath)) WeatherImage = LoadImageSafe(ImagePath);
        }
        private Bitmap? LoadImageSafe(string path)
        {
            try { return new Bitmap(AssetLoader.Open(new Uri(path))); } catch { return null; }
        }
    }
}