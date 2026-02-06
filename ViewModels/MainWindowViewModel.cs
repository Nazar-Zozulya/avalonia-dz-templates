using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using avalonia_dz_templates.Models;
using avalonia_dz_templates.Services;

namespace avalonia_dz_templates.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string SaveFileName = "saved_cities.json";
        private readonly WeatherService _weatherService = new WeatherService();

        public ObservableCollection<CityViewModel> Cities { get; set; } = new();
        public List<string> AvailableCities { get; }

        private bool _showAddButton;
        public bool ShowAddButton
        {
            get => _showAddButton;
            set => this.RaiseAndSetIfChanged(ref _showAddButton, value);
        }

        private CityViewModel _selectedCity = null!;
        public CityViewModel SelectedCity
        {
            get => _selectedCity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCity, value);
                CheckButtonVisibility();
                UpdateTime();
            }
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand SaveCityCommand { get; }

        // --- ВЛАСТИВОСТІ ГОДИННИКА (Повернув CurrentDayOfWeek) ---
        private string _currentTime = "";
        public string CurrentTime { get => _currentTime; set => this.RaiseAndSetIfChanged(ref _currentTime, value); }
        
        private string _currentDate = "";
        public string CurrentDate { get => _currentDate; set => this.RaiseAndSetIfChanged(ref _currentDate, value); }
        
          private string _currentDayOfWeek = "";
        public string CurrentDayOfWeek { get => _currentDayOfWeek; set => this.RaiseAndSetIfChanged(ref _currentDayOfWeek, value); }

        public MainWindowViewModel()
        {
            AvailableCities = new List<string> 
            { 
                "Київ", "Львів", "Харків", "Одеса", "Дніпро", "Запоріжжя", "Вінниця", "Полтава", "Чернігів", "Черкаси", 
                "Житомир", "Суми", "Хмельницький", "Чернівці", "Рівне", "Івано-Франківськ", "Тернопіль", "Луцьк", "Ужгород", 
                "Варшава", "Лондон", "Париж", "Берлін", "Мюнхен", "Рим", "Мілан", "Мадрид", "Барселона", "Нью-Йорк", "Токіо"
            };
            AvailableCities.Sort();

            LoadData();

            if (Cities.Count == 0)
            {
                Cities.Add(new CityViewModel("Київ", 0, "Завантаження...", 0, 0, "avares://WeatherApp/Assets/cloud.png", 7200));
            }
            SelectedCity = Cities.First();

            _ = UpdateAllCitiesWeather();

            SearchCommand = ReactiveCommand.CreateFromTask(async () => 
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return;
                await SearchCityApi(SearchText);
            });

            SaveCityCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedCity == null) return;
                
                if (!Cities.Any(c => c.Name.Equals(SelectedCity.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    Cities.Add(SelectedCity);
                    SaveData();
                    SearchText = "";
                    ShowAddButton = false; // Ховаємо кнопку
                }
            });

            StartClock();
        }

        private async Task SearchCityApi(string query)
        {
            var data = await _weatherService.GetWeatherAsync(query);
            if (data == null) return;


string iconPath = data.Weather[0].Main.ToLower().Contains("cloud")  data.Weather[0].Main.ToLower().Contains("rain")
                ? "avares://WeatherApp/Assets/cloud.png" 
                : "avares://WeatherApp/Assets/sun.png";

            var newCity = new CityViewModel(
                data.Name,
                (int)data.Main.Temp,
                data.Weather[0].Description,
                (int)data.Main.TempMax,
                (int)data.Main.TempMin,
                iconPath,
                data.Timezone 
            );
            newCity.Humidity = data.Main.Humidity;
            newCity.WindSpeed = (int)data.Wind.Speed;

            try 
            {
                var forecastData = await _weatherService.GetForecastAsync(data.Name);
                if (forecastData != null)
                {
                    foreach (var item in forecastData.List.Take(8))
                    {
                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(data.Timezone);
                        string itemIcon = item.Weather[0].Main.ToLower().Contains("cloud")  item.Weather[0].Main.ToLower().Contains("rain")
                            ? "avares://WeatherApp/Assets/cloud.png" 
                            : "avares://WeatherApp/Assets/sun.png";

                        newCity.HourlyForecasts.Add(new HourlyForecastViewModel(date.ToString("HH:mm"), (int)item.Main.Temp, itemIcon));
                    }
                }
            }
            catch {}

            SelectedCity = newCity;
            CheckButtonVisibility(); // Перевіряємо кнопку
        }

        private void CheckButtonVisibility()
        {
            if (SelectedCity == null) { ShowAddButton = false; return; }
            bool exists = Cities.Any(c => c.Name.Equals(SelectedCity.Name, StringComparison.OrdinalIgnoreCase));
            ShowAddButton = !exists;
        }

        private async Task UpdateAllCitiesWeather()
        {
            foreach (var city in Cities)
            {
                var data = await _weatherService.GetWeatherAsync(city.Name);
                if (data != null)
                {
                    city.Temperature = (int)data.Main.Temp;
                    city.Description = data.Weather[0].Description;
                    city.MaxTemp = (int)data.Main.TempMax;
                    city.MinTemp = (int)data.Main.TempMin;
                    city.Humidity = data.Main.Humidity;
                    city.WindSpeed = (int)data.Wind.Speed;
                    city.TimezoneOffsetSeconds = data.Timezone;

                    string icon = data.Weather[0].Main.ToLower().Contains("cloud") || data.Weather[0].Main.ToLower().Contains("rain")
                        ? "avares://WeatherApp/Assets/cloud.png" : "avares://WeatherApp/Assets/sun.png";
                    city.ImagePath = icon;
                    city.RestoreImage();

                    try {
                        var forecast = await _weatherService.GetForecastAsync(city.Name);
                        if (forecast != null) {
                            city.HourlyForecasts.Clear();
                            foreach (var item in forecast.List.Take(8)) {
                                DateTime d = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(city.TimezoneOffsetSeconds);
                                string ic = item.Weather[0].Main.ToLower().Contains("cloud") ? "avares://WeatherApp/Assets/cloud.png" : "avares://WeatherApp/Assets/sun.png";
                                city.HourlyForecasts.Add(new HourlyForecastViewModel(d.ToString("HH:mm"), (int)item.Main.Temp, ic));
                            }
                        }
                    } catch {}
                }
            }
            UpdateTime();
        }
        
private void SaveData()
        {
             try {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Cities, options);
                File.WriteAllText(SaveFileName, json);
             } catch { }
        }

        private void LoadData()
        {
            if (!File.Exists(SaveFileName)) return;
            try {
                string json = File.ReadAllText(SaveFileName);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<CityViewModel>>(json);
                if (loaded != null) {
                    Cities.Clear();
                    foreach(var c in loaded) { c.RestoreImage(); Cities.Add(c); }
                }
            } catch { }
        }
        
        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => UpdateTime();
            timer.Start();
            UpdateTime();
        }

        // Оновлюємо метод UpdateTime, щоб він заповнював День Тижня
        private void UpdateTime()
        {
            int offset = SelectedCity != null ? SelectedCity.TimezoneOffsetSeconds : 0;
            DateTime target = DateTime.UtcNow.AddSeconds(offset);
            
            CurrentTime = target.ToString("HH:mm");
            CurrentDate = target.ToString("dd.MM.yyyy");

            // Заповнюємо День Тижня
            var cult = new System.Globalization.CultureInfo("uk-UA");
            string day = target.ToString("dddd", cult);
            CurrentDayOfWeek = char.ToUpper(day[0]) + day.Substring(1);
        }
    }
}