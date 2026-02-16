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
    // Створюємо клас ViewModel
    public class MainWindowViewModel : ViewModelBase
    {
        // Ствоємо файл який буде сохраняти городи в json файлі
        private const string SaveFileName = "saved_cities.json";
        // отримуємо функцію відправки запроса
        private readonly WeatherService _weatherService = new WeatherService();

        // Отримуємо коллекцію міст і данних про них
        public ObservableCollection<CityViewModel> Cities { get; set; } = new();
        // Створюємо поле з вказаними в ньому доступними містами 
        public List<string> AvailableCities { get; }

        // Створюємо приватну змінну для показу кнопки  
        private bool _showAddButton;

        // Робимо для цієї змінної властивість
        public bool ShowAddButton
        {
            get => _showAddButton;
            set => this.RaiseAndSetIfChanged(ref _showAddButton, value);
        }

        // Створюємо змінну вибраного міста
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

        // Створюємо змінну введеного в пошут текста
        private string _searchText = "";

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        // ICommand це інтерфейс який связує між собою дії користувача та ViewModel файл.
        // У ICommand є 3 обов'язкові елементи:
        // Execute(object parameter) - це логіка яка виконується при визові команди
        // CanExecute(object parameter) - це функція яка повертає статус активності кнопки (true - false)
        // CanExecuteUpdate - це подія яка повідомляє систему про те що CanExecute змінилась і треба перепровірити доступність команди
        
        // ?
        public ICommand SearchCommand { get; }
        // Змінна кнопки яка відповідає за збереження нового городу
        public ICommand SaveCityCommand { get; }

        // --- ВЛАСТИВОСТІ ГОДИННИКА (Повернув CurrentDayOfWeek) ---
        private string _currentTime = "";

        public string CurrentTime
        {
            get => _currentTime;
            set => this.RaiseAndSetIfChanged(ref _currentTime, value);
        }

        private string _currentDate = "";

        public string CurrentDate
        {
            get => _currentDate;
            set => this.RaiseAndSetIfChanged(ref _currentDate, value);
        }

        private string _currentDayOfWeek = "";

        public string CurrentDayOfWeek
        {
            get => _currentDayOfWeek;
            set => this.RaiseAndSetIfChanged(ref _currentDayOfWeek, value);
        }

        public MainWindowViewModel()
        {
            // Доступні міста
            AvailableCities = new List<string>
            {
                "Київ", "Львів", "Харків", "Одеса", "Дніпро", "Запоріжжя", "Вінниця", "Полтава", "Чернігів", "Черкаси",
                "Житомир", "Суми", "Хмельницький", "Чернівці", "Рівне", "Івано-Франківськ", "Тернопіль", "Луцьк",
                "Ужгород",
                "Варшава", "Лондон", "Париж", "Берлін", "Мюнхен", "Рим", "Мілан", "Мадрид", "Барселона", "Нью-Йорк",
                "Токіо"
            };
            // Сортуємо цей список
            AvailableCities.Sort();


            // Завантажуємо дані
            LoadData();
            _ = UpdateAllCitiesWeather();

            // Якщо немає міст, додаємо Київ як замінник
            if (Cities.Count == 0)
            {
                Cities.Add(new CityViewModel("Київ", 0, "Завантаження...", 0, 0, "avares://WeatherApp/Assets/cloud.png", 7200));
            }

            // Встановлюємо перше місто вибраним
            SelectedCity = Cities.First();

            // Команди
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

            // Запускаємо відлік часу
            StartClock();
        }

        // Методи для отримання погоди та її оновлення
        private async Task SearchCityApi(string query)
        {
            var data = await _weatherService.GetWeatherAsync(query);
            System.Console.WriteLine("data: ",data);

            if (data == null) return;

            string iconPath = data.Weather[0].Main.ToLower().Contains("cloud") || data.Weather[0].Main.ToLower().Contains("rain")
                ? "avares://avalonia_dz_templates/Assets/cloudy.png"
                : "avares://avalonia_dz_templates/Assets/sun.png";

            var newCity = new CityViewModel(
                data.Name,
                (int)data.Main.Temp,
                data.Weather[0].Description,
                (int)data.Main.TempMax,
                (int)data.Main.TempMin,
                iconPath,
                data.Timezone
            );

            try
            {
                var forecastData = await _weatherService.GetForecastAsync(data.Name);

                System.Console.WriteLine("forecastData: ", forecastData);
                if (forecastData != null)
                {
                    foreach (var item in forecastData.List.Take(15))
                    {
                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(data.Timezone);
                        string itemIconMain = item.Weather?.Count > 0 ? item.Weather[0].Main.ToLower() : "";
                        string itemIcon = item.Weather[0].Main.ToLower().Contains("cloud") || item.Weather[0].Main
                            .ToLower().Contains("rain")
                            ? "avares://avalonia_dz_templates/Assets/cloudy.png"
                            : "avares://avalonia_dz_templates/Assets/sun.png";
                        newCity.HourlyForecasts.Add(new HourlyForecastViewModel(date.ToString("HH:mm"),
                            (int)item.Main.Temp, itemIcon));
                    }
                }
            }
            catch
            {
            }

            SelectedCity = newCity;
            CheckButtonVisibility();
        }

        // Оновлюємо погоду для всіх міст
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
                    city.TimezoneOffsetSeconds = data.Timezone;

                    string icon = data.Weather[0].Main.ToLower().Contains("cloud") || data.Weather[0].Main.ToLower().Contains("rain")
                        ? "avares://avalonia_dz_templates/Assets/cloudy.png"
                        : "avares://avalonia_dz_templates/Assets/sun.png";
                    city.ImagePath = icon;
                    city.RestoreImage();

                    try
                    {
                        var forecast = await _weatherService.GetForecastAsync(city.Name);
                        if (forecast != null)
                        {
                            city.HourlyForecasts.Clear();
                            foreach (var item in forecast.List.Take(8))
                            {
                                DateTime d = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime
                                    .AddSeconds(city.TimezoneOffsetSeconds);
                                string ic = item.Weather[0].Main.ToLower().Contains("cloud")
                                    ? "avares://avalonia_dz_templates/Assets/cloudy.png"
                                    : "avares://avalonia_dz_templates/Assets/sun.png";
                                city.HourlyForecasts.Add(new HourlyForecastViewModel(d.ToString("HH:mm"),
                                    (int)item.Main.Temp, ic));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            UpdateTime();
        }

        private void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Cities, options);
                File.WriteAllText(SaveFileName, json);
            }
            catch
            {
            }
        }

        private void LoadData()
        {
            if (!File.Exists(SaveFileName)) return;
            try
            {
                string json = File.ReadAllText(SaveFileName);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<CityViewModel>>(json);
                if (loaded != null)
                {
                    Cities.Clear();
                    foreach (var c in loaded)
                    {
                        c.RestoreImage();
                        Cities.Add(c);
                    }
                }
            }
            catch
            {
            }
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

            var cult = new System.Globalization.CultureInfo("uk-UA");
            string day = target.ToString("dddd", cult);
            CurrentDayOfWeek = char.ToUpper(day[0]) + day.Substring(1);
        }

        // Перевіряємо коли кнопка додавання міста буде потрібна
        private void CheckButtonVisibility()
        {
            if (SelectedCity == null)
            {
                ShowAddButton = false;
                return;
            }

            bool exists = Cities.Any(c => c.Name.Equals(SelectedCity.Name, StringComparison.OrdinalIgnoreCase));
            ShowAddButton = !exists;
        }
    }
}
