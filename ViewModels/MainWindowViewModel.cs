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
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Measure;

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
                // ===== Украина =====
                "Київ", "Kyiv",
                "Львів", "Lviv",
                "Харків", "Kharkiv",
                "Одеса", "Odesa",
                "Дніпро", "Dnipro",
                "Запоріжжя", "Zaporizhzhia",
                "Вінниця", "Vinnytsia",
                "Полтава", "Poltava",
                "Чернігів", "Chernihiv",
                "Черкаси", "Cherkasy",
                "Житомир", "Zhytomyr",
                "Суми", "Sumy",
                "Хмельницький", "Khmelnytskyi",
                "Чернівці", "Chernivtsi",
                "Рівне", "Rivne",
                "Івано-Франківськ", "Ivano-Frankivsk",
                "Тернопіль", "Ternopil",
                "Луцьк", "Lutsk",
                "Ужгород", "Uzhhorod",
                "Миколаїв", "Mykolaiv",
                "Херсон", "Kherson",
                "Кропивницький", "Kropyvnytskyi",
                "Кривий Ріг", "Kryvyi Rih",

                // ===== Польща =====
                "Варшава", "Warsaw",
                "Краків", "Krakow",
                "Вроцлав", "Wroclaw",
                "Гданськ", "Gdansk",
                "Познань", "Poznan",
                "Лодзь", "Lodz",

                // ===== Німеччина =====
                "Берлін", "Berlin",
                "Мюнхен", "Munich",
                "Гамбург", "Hamburg",
                "Франкфурт", "Frankfurt",
                "Кельн", "Cologne",
                "Штутгарт", "Stuttgart",

                // ===== Франція =====
                "Париж", "Paris",
                "Марсель", "Marseille",
                "Ліон", "Lyon",
                "Тулуза", "Toulouse",
                "Ніцца", "Nice",

                // ===== Італія =====
                "Рим", "Rome",
                "Мілан", "Milan",
                "Неаполь", "Naples",
                "Турин", "Turin",
                "Венеція", "Venice",

                // ===== Іспанія =====
                "Мадрид", "Madrid",
                "Барселона", "Barcelona",
                "Валенсія", "Valencia",
                "Севілья", "Seville",

                // ===== Велика Британія =====
                "Лондон", "London",
                "Манчестер", "Manchester",
                "Бірмінгем", "Birmingham",
                "Ліверпуль", "Liverpool",
                "Глазго", "Glasgow",
                "Единбург", "Edinburgh",

                // ===== Інша Європа =====
                "Прага", "Prague",
                "Відень", "Vienna",
                "Амстердам", "Amsterdam",
                "Брюссель", "Brussels",
                "Цюрих", "Zurich",
                "Стокгольм", "Stockholm",
                "Осло", "Oslo",
                "Копенгаген", "Copenhagen",
                "Гельсінкі", "Helsinki",
                "Лісабон", "Lisbon",
                "Афіни", "Athens",
                "Будапешт", "Budapest",
                "Бухарест", "Bucharest",
                "Братислава", "Bratislava",
                "Вільнюс", "Vilnius",
                "Рига", "Riga",
                "Таллінн", "Tallinn",
                "Софія", "Sofia",
                "Белград", "Belgrade",
                "Загреб", "Zagreb"
            };
            // Сортуємо цей список
            AvailableCities.Sort();


            // Завантажуємо дані
            LoadData();
            // _ = UpdateAllCitiesWeather();

            Task.Run(async () => {await Task.Delay(1000); await UpdateAllCitiesWeather(); });

            // Якщо немає міст, додаємо Київ як замінник
            if (Cities.Count == 0)
            {


                Cities.Add(new CityViewModel("Київ", 0, "Завантаження...", 0, 0, "avares://avalonia_dz_templates/Assets/cloudy.png", 7200));
            }

            // Встановлюємо перше місто вибраним
            SelectedCity = Cities.First();

            // Команди
            SearchCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    Console.WriteLine("city not found");
                    return;
                }
                System.Console.WriteLine("Search Text:" + SearchText);
                await Task.Delay(1000);
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
            
            System.Console.WriteLine(123);

            if (data == null) return;

            string iconPath = GetIconPath(data.Weather[0].Main.ToLower());


            // string iconPath = data.Weather[0].Main.ToLower().Contains("cloud") || data.Weather[0].Main.ToLower().Contains("rain")
            //     ? "avares://avalonia-dz-templates/Assets/cloudy.png"
            //     : "avares://avalonia-dz-templates/Assets/sun.png";

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

                if (forecastData != null)
                {
                    foreach (var item in forecastData.List.Take(15))
                    {
                        if (item == null)
                            continue;

                        // if (item.Main == null)
                        //     continue;

                        if (item.Weather == null || item.Weather.Count == 0)
                            continue;


                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(data.Timezone);
                        string itemIconMain = item.Weather?.Count > 0 ? item.Weather[0].Main.ToLower() : "";
                        string itemIcon = GetHourlyIconPath(itemIconMain);
                        newCity.HourlyForecasts.Add(new HourlyForecastViewModel(date.ToString("HH:mm"),
                            (int)item.Main.Temp, itemIcon));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                SelectedCity = null;
                SelectedCity = newCity;
                this.RaisePropertyChanged(nameof(SelectedCity));
            });
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

                    string icon = GetIconPath(data.Weather[0].Main);

                    // string icon = data.Weather[0].Main.ToLower().Contains("clouds") || data.Weather[0].Main.ToLower().Contains("rain") // snow
                    //     ? "avares://avalonia-dz-templates/Assets/cloudy.png"
                    //     : "avares://avalonia-dz-templates/Assets/sun.png";
                    city.ImagePath = icon;
                    city.RestoreImage();

                    try
                    {
                        var forecast = await _weatherService.GetForecastAsync(city.Name);
                        if (forecast != null)
                        {
                            city.HourlyForecasts.Clear();
                            foreach (var item in forecast.List.Take(15))
                            {
                                DateTime d = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime
                                    .AddSeconds(city.TimezoneOffsetSeconds);
                                string ic = GetHourlyIconPath(item.Weather[0].Main);
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

        private string GetHourlyIconPath(string weatherMain)
        {
            if (string.IsNullOrEmpty(weatherMain))
                return "avares://avalonia-dz-templates/Assets/unknown-files.png";

            weatherMain = weatherMain.ToLower();
            switch (weatherMain)
            {
                case "clouds":
                    return "avares://avalonia-dz-templates/Assets/wb/cloudy.png";
                case "rain":
                    return "avares://avalonia-dz-templates/Assets/wb/rainy.png";
                case "snow":
                    return "avares://avalonia-dz-templates/Assets/wb/snowy.png";
                default:
                    return "avares://avalonia-dz-templates/Assets/wb/sunny.png";
            }

        }

        private string GetIconPath(string weatherMain)
        {
            if (string.IsNullOrEmpty(weatherMain))
                return "avares://avalonia-dz-templates/Assets/unknown-files.png";

            weatherMain = weatherMain.ToLower();
            // System.Console.WriteLine("aaa:" + weatherMain);
            switch (weatherMain)
            {
                case "clouds":
                    return "avares://avalonia-dz-templates/Assets/forecast/clouds.png";
                case "rain":
                    return "avares://avalonia-dz-templates/Assets/forecast/rain.png";
                case "snow":
                    return "avares://avalonia-dz-templates/Assets/forecast/snow.png";
                default:
                    return "avares://avalonia-dz-templates/Assets/forecast/sun.png";
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
