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


            var  a = UpdateAllCitiesWeather();

            System.Console.WriteLine(a);
            
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

            // ?
            LoadData();

            // Робимо перевірку якщо міст немає то ми добавляєм київ(поки костиль і можливо треба буде заміннювати на те місто де ти знаходишся)
            if (Cities.Count == 0)
            {
                Cities.Add(new CityViewModel("Київ", 0, "Завантаження...", 0, 0, "avares://WeatherApp/Assets/cloud.png",
                    7200));
            }

            // Ставимо перше місто в нашому массиві вибранним
            SelectedCity = Cities.First();

            // ?
            _ = UpdateAllCitiesWeather();

            // ?
            SearchCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return;
                await SearchCityApi(SearchText);
            });

            // Створюємо нове місто в массиві Cities при натисканні на створення міста через функцію ICommand.Execute 
            SaveCityCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedCity == null) return;

                // ?
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

        // Task - Аналог promise. По суті обіцянка що функція буде повертати щось
        
        // Створємо метод який буде отримувати інформацію про погоду у нового міста
        private async Task SearchCityApi(string query)
        {
            // Визиваємо API запрос для отримання данних про місто. query = назва міста
            var data = await _weatherService.GetWeatherAsync(query);
            if (data == null) return;
            Console.WriteLine(data);

            // Якщо погода хмарна ставимо дождливу картинку а якщо ні то сонячну(костиль)
            string iconPath = data.List[0].Weather[0].Main.ToLower().Contains("cloud") || data.List[0].Weather[0].Main.ToLower().Contains("rain")
                ? "avares://WeatherApp/Assets/cloud.png"
                : "avares://WeatherApp/Assets/sun.png";

            // Створюємо об'єкт нового міста
            var newCity = new CityViewModel(
                data.List[0].Name,
                (int)data.List[0].Main.Temp,
                data.List[0].Weather[0].Description,
                (int)data.List[0].Main.TempMax,
                (int)data.List[0].Main.TempMin,
                iconPath,
                data.List[0].Timezone
            );
            newCity.Humidity = data.List[0].Main.Humidity;
            newCity.WindSpeed = (int)data.List[0].Wind.Speed;
    
            try
            {
                
                var forecastData = await _weatherService.GetWeatherAsync(data.List[0].Name);
                if (forecastData != null)
                {
                    foreach (var item in forecastData.List.Take(8))
                    {
                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(data.List[0].Timezone);
                        string itemIcon = item.Weather[0].Main.ToLower().Contains("cloud") || item.Weather[0].Main
                            .ToLower().Contains("rain")
                            ? "avares://WeatherApp/Assets/cloud.png"
                            : "avares://WeatherApp/Assets/sun.png";
                        // Запускаємо час для нового городу
                        newCity.HourlyForecasts.Add(new HourlyForecastViewModel(date.ToString("HH:mm"),
                            (int)item.Main.Temp, itemIcon));
                    }
                }
            }
            catch
            {
            }

            SelectedCity = newCity;
            CheckButtonVisibility(); // Перевіряємо кнопку
        }

        // Перевіряємо коли кнопка додавання міста буде потрібна
        private void CheckButtonVisibility()
        {
            if (SelectedCity == null)
            {
                ShowAddButton = false;
                return;
            }
            // якщо немає ніяких городів то кнопка буде показуватися і наоборот
            bool exists = Cities.Any(c => c.Name.Equals(SelectedCity.Name, StringComparison.OrdinalIgnoreCase));
            ShowAddButton = !exists;
        }
        
        
        // функція для оновлення погоди у всіх містах які є в Cities
        private async Task UpdateAllCitiesWeather()
        {
            foreach (var city in Cities)
            {
                var data = await _weatherService.GetWeatherAsync(city.Name);
                if (data != null)
                {
                    Console.WriteLine(data);
                    
                    city.Temperature = (int)data.List[0].Main.Temp;
                    city.Description = data.List[0].Weather[0].Description;
                    city.MaxTemp = (int)data.List[0].Main.TempMax;
                    city.MinTemp = (int)data.List[0].Main.TempMin;
                    city.Humidity = data.List[0].Main.Humidity;
                    city.WindSpeed = (int)data.List[0].Wind.Speed;
                    city.TimezoneOffsetSeconds = data.List[0].Timezone;

                    string icon = data.List[0].Weather[0].Main.ToLower().Contains("cloud") ||
                                  data.List[0].Weather[0].Main.ToLower().Contains("rain")
                        ? "avares://WeatherApp/Assets/cloud.png"
                        : "avares://WeatherApp/Assets/sun.png";
                    city.ImagePath = icon;
                    city.RestoreImage();

                    try
                    {
                        var forecast = await _weatherService.GetWeatherAsync(city.Name);
                        if (forecast != null)
                        {
                            city.HourlyForecasts.Clear();
                            foreach (var item in forecast.List.Take(8))
                            {
                                DateTime d = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime
                                    .AddSeconds(city.TimezoneOffsetSeconds);
                                string ic = item.Weather[0].Main.ToLower().Contains("cloud")
                                    ? "avares://WeatherApp/Assets/cloud.png"
                                    : "avares://WeatherApp/Assets/sun.png";
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
                // Вказуємо настройки Json Серіалайзера
                var options = new JsonSerializerOptions { WriteIndented = true };
                // Переводимо всі міста в json строку
                string json = JsonSerializer.Serialize(Cities, options);
                // сохраняємо їх в json файл
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
                // Получаємо текст с json
                string json = File.ReadAllText(SaveFileName);
                // Переводимо json строку обратно в коллекцію
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

            // Заповнюємо День Тижня
            var cult = new System.Globalization.CultureInfo("uk-UA");
            string day = target.ToString("dddd", cult);
            CurrentDayOfWeek = char.ToUpper(day[0]) + day.Substring(1);
        }
    }
}