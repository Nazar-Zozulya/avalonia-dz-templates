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
using avalonia_dz_templates.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Avalonia.Styling;
using avalonia_dz_templates.Helpers;
using Avalonia.Media;

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
                UpdateAppBackground();
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
        private string _themeIcon = "avares://avalonia-dz-templates/Assets/button-theme-black.png";

        public string ThemeIcon
        {
            get => _themeIcon;
            set => this.RaiseAndSetIfChanged(ref _themeIcon, value);
        }

        public ICommand SearchCommand { get; }
        // Змінна кнопки яка відповідає за збереження нового городу
        public ICommand SaveCityCommand { get; }

        public ICommand DeleteCityCommand { get; }

        public ICommand OpenSettingsCommand { get; }

        public ICommand ToggleThemeCommand { get; }

        public ICommand SaveSettingsCityCommand { get; }

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


        private Dictionary<string, List<string>> _cityDictionary = new Dictionary<string, List<string>>
        {
            { "Ukraine", new List<string> { "Kyiv", "Lviv", "Kharkiv", "Odesa", "Dnipro", "Zaporizhzhia", "Vinnytsia", "Poltava", "Chernihiv", "Cherkasy", "Zhytomyr", "Sumy", "Khmelnytskyi", "Chernivtsi", "Rivne", "Ivano-Frankivsk", "Ternopil", "Lutsk", "Uzhhorod", "Mykolaiv", "Kherson", "Kropyvnytskyi", "Kryvyi Rih" } },
            { "Poland", new List<string> { "Warsaw", "Krakow", "Wroclaw", "Gdansk", "Poznan", "Lodz", "Szczecin", "Bydgoszcz", "Lublin", "Bialystok" } },
            { "Germany", new List<string> { "Berlin", "Munich", "Hamburg", "Frankfurt", "Cologne", "Stuttgart", "Dusseldorf", "Leipzig", "Dortmund", "Essen", "Bremen", "Dresden" } },
            { "France", new List<string> { "Paris", "Marseille", "Lyon", "Toulouse", "Nice", "Nantes", "Strasbourg", "Montpellier", "Bordeaux", "Lille" } },
            { "Italy", new List<string> { "Rome", "Milan", "Naples", "Turin", "Venice", "Florence", "Bologna", "Genoa", "Palermo", "Bari", "Catania" } },
            { "Spain", new List<string> { "Madrid", "Barcelona", "Valencia", "Seville", "Zaragoza", "Malaga", "Murcia", "Palma", "Las Palmas", "Bilbao" } },
            { "United Kingdom", new List<string> { "London", "Manchester", "Birmingham", "Liverpool", "Glasgow", "Edinburgh", "Bristol", "Leeds", "Sheffield", "Newcastle", "Cardiff", "Belfast" } },
            { "USA", new List<string> { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville", "San Francisco", "Indianapolis", "Seattle", "Denver", "Washington", "Boston", "Miami", "Atlanta" } },
            { "Canada", new List<string> { "Toronto", "Montreal", "Vancouver", "Calgary", "Edmonton", "Ottawa", "Winnipeg", "Quebec City", "Hamilton", "Halifax", "Victoria" } },
            { "Australia", new List<string> { "Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide", "Gold Coast", "Canberra", "Newcastle", "Wollongong", "Hobart", "Darwin" } },
            { "Japan", new List<string> { "Tokyo", "Yokohama", "Osaka", "Nagoya", "Sapporo", "Fukuoka", "Kobe", "Kyoto", "Kawasaki", "Saitama", "Hiroshima" } },
            { "Brazil", new List<string> { "Sao Paulo", "Rio de Janeiro", "Brasilia", "Salvador", "Fortaleza", "Belo Horizonte", "Manaus", "Curitiba", "Recife", "Porto Alegre" } },
            { "Other Europe", new List<string> { "Prague", "Vienna", "Amsterdam", "Brussels", "Zurich", "Stockholm", "Oslo", "Copenhagen", "Helsinki", "Lisbon", "Athens", "Budapest", "Bucharest", "Bratislava", "Vilnius", "Riga", "Tallinn", "Sofia", "Belgrade", "Zagreb", "Ljubljana", "Sarajevo", "Skopje", "Tirana" } },
            { "Asia", new List<string> { "Beijing", "Shanghai", "Seoul", "Bangkok", "Singapore", "Kuala Lumpur", "Jakarta", "Manila", "Hanoi", "Ho Chi Minh City", "New Delhi", "Mumbai", "Dubai", "Istanbul", "Taipei", "Hong Kong" } }
        };


        private IBrush _appBackground = new SolidColorBrush(Colors.Transparent);

        public IBrush AppBackground
        {
            get => _appBackground;
            set => this.RaiseAndSetIfChanged(ref _appBackground, value);
        }

        private ObservableCollection<string> _citiesForSelectedCountry = new ObservableCollection<string>();
        public ObservableCollection<string> CitiesForSelectedCountry
        {
            get => _citiesForSelectedCountry;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _citiesForSelectedCountry, value);
            }
            
        }

        private string _selectedNewCity = string.Empty;
        public string SelectedNewCity
        {
            get => _selectedNewCity;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _selectedNewCity, value);
                System.Console.WriteLine(SelectedNewCity);
            }
        }

        private string _coordinatesText = string.Empty;
        public string CoordinatesText
        {
            get => _coordinatesText;
            set => this.RaiseAndSetIfChanged(ref _coordinatesText, value);
        }

        private ObservableCollection<string> _availableCountries = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableCountries
        {
            get => _availableCountries;
            set => this.RaiseAndSetIfChanged(ref _availableCountries, value);
        }

        private string _selectedCountry = string.Empty;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCountry, value);
                SetNewSelectedCountries();
            }
        }


        private void SetNewSelectedCountries()
        {
            CitiesForSelectedCountry = new ObservableCollection<string>();

            if (_cityDictionary.TryGetValue(SelectedCountry, out List<string> cities))
            {
               CitiesForSelectedCountry = new ObservableCollection<string>(cities);
            }
        }
        

        public MainWindowViewModel()
        {

            // Доступні міста


            // Завантажуємо дані

            AvailableCountries = new ObservableCollection<string>(_cityDictionary.Keys);

            AvailableCities = new List<string>();
            foreach (var citiesList in _cityDictionary.Values)
            {
                AvailableCities.AddRange(citiesList);
            }

            AvailableCities.Sort();
            // AvailableCountries = new ObservableCollection<string> {"1", "2", "3"};

            System.Console.WriteLine("Available countries: " + JsonSerializer.Serialize(AvailableCountries));

            LoadData();
            _ = UpdateAllCitiesWeather();

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

            DeleteCityCommand = ReactiveCommand.Create<CityViewModel>(DeleteCity);

            OpenSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var settingsWindow = new SettingsModalView();
                var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

                if (lifetime != null && lifetime.MainWindow != null)
                {
                    settingsWindow.DataContext = this;
                    await settingsWindow.ShowDialog(lifetime.MainWindow);
                }
            });

            ToggleThemeCommand = ReactiveCommand.Create(() =>
            {
                if (Application.Current != null)
                {
                    var currentTheme = Application.Current.RequestedThemeVariant;
                    Application.Current.RequestedThemeVariant = currentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;     
                    
                }
            });


            SaveSettingsCityCommand = ReactiveCommand.Create(async () =>
            {
                var searchCityAnswer = await SearchAndAddNewCity(SelectedNewCity);
                if (searchCityAnswer == null) return;

                System.Console.WriteLine(searchCityAnswer.Name);

                if (!Cities.Any(c => c.Name.Equals(searchCityAnswer.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    Cities.Add(searchCityAnswer);

                    SelectedCity = searchCityAnswer;
                    SaveData();
                }
            });




            // Запускаємо відлік часу
            StartClock();
        }

        private void DeleteCity(CityViewModel cityToRemove)
        {
            if (cityToRemove != null && Cities.Contains(cityToRemove))
            {
                Cities.Remove(cityToRemove);

                if (SelectedCity == cityToRemove)
                {
                    SelectedCity = Cities.First();
                }
            }
            SaveData();
        }

        // Методи для отримання погоди та її оновлення

        private async Task<CityViewModel> SearchAndAddNewCity(string query)
        {
            var data = await _weatherService.GetWeatherAsync(query);

            if (data == null) return null!; // или throw, если предпочитаешь

            string iconPath = GetIconPath(data.Weather[0].Main.ToLower());

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
                        if (item == null || item.Weather == null || item.Weather.Count == 0)
                            continue;

                        DateTime date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.AddSeconds(data.Timezone);
                        string itemIconMain = item.Weather[0].Main.ToLower();
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
                SelectedCity = newCity;
                this.RaisePropertyChanged(nameof(SelectedCity));
            });

            CheckButtonVisibility();

            return newCity; // <- вот сюда нужно вернуть объект
        }

        private void UpdateAppBackground()
        {
            if (SelectedCity != null)
            {
                AppBackground = WeatherBackgroundHelper.GetGradientWeather(SelectedCity.Description);
            }
        }

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
            UpdateAppBackground();
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
