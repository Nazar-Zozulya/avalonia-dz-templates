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
using LiveChartsCore.SkiaSharpView.Painting.Effects;


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

        [JsonIgnore]
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value); // Тепер екран оновиться!
        }

        [JsonIgnore]
        public int Temperature
        {
            get => _temperature;
            set => this.RaiseAndSetIfChanged(ref _temperature, value);
        }

        [JsonIgnore]
        public int MaxTemp
        {
            get => _maxTemp;
            set => this.RaiseAndSetIfChanged(ref _maxTemp, value);
        }

        [JsonIgnore]
        public int MinTemp
        {
            get => _minTemp;
            set => this.RaiseAndSetIfChanged(ref _minTemp, value);
        }

        // public int Humidity
        // {
        //     get => _humidity;
        //     set => this.RaiseAndSetIfChanged(ref _humidity, value);
        // }

        [JsonIgnore]
        public int WindSpeed
        {
            get => _windSpeed;
            set => this.RaiseAndSetIfChanged(ref _windSpeed, value);
        }

        [JsonIgnore]
        public int TimezoneOffsetSeconds
        {
            get => _timezoneOffsetSeconds;
            set => this.RaiseAndSetIfChanged(ref _timezoneOffsetSeconds, value);
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public List<int> Icons { get; } = new() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

        [JsonIgnore]
        public ISeries[] Series { get; set; } = Array.Empty<ISeries>();

        [JsonIgnore]
        public Axis[] XAxes { get; set; } = { new Axis() };

        [JsonIgnore]
        public Axis[] YAxes { get; set; } = { new Axis() };

        public CityViewModel() 
        { 
            // Ініціалізація для JSON

           ChartsSettings();

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


            System.Console.WriteLine("Name: " + name);

        
            ChartsSettings();


            WeatherImage = LoadImageSafe(imagePath);
        }

        // --- МЕТОДИ ---

        // public void AddTemperature(double temp)
        // {
        //     _hourlyForecasts.Add(new HourlyForecastViewModel(temp));
        // }

        private void ChartsSettings()
        {
            var gradient = new LinearGradientPaint(
                new[] {SKColors.Yellow, SKColors.LightSkyBlue},
                new SKPoint(0.5f, 0),
                new SKPoint(0.5f, 1)
            );
            Series = new ISeries[]
            {
                new ColumnSeries<HourlyForecastViewModel>
                {
                    Values = _hourlyForecasts,
                    Fill = gradient,
                    Stroke = null,
                    Padding = 1,
                    MaxBarWidth = double.PositiveInfinity,
                    Mapping = (model, index) => new LiveChartsCore.Kernel.Coordinate(index, model.Temprature + 10),
                    YToolTipLabelFormatter = point => $"{point.Model?.Temprature}"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = null,
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255, 30))
                    {
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    }
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {   
                    Position = AxisPosition.End,
                    Labeler = value => $"{value - 10}°C",
                    MinLimit = -10,
                    MaxLimit = 25,
                    MinStep = 5,
                    ForceStepToMin = true,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    SeparatorsPaint = new SolidColorPaint(new SKColor(255, 255, 255, 30))
                    {
                        StrokeThickness = 1,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    }
                    // SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    // ForceStepToMin ,
                }
            };
        }


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