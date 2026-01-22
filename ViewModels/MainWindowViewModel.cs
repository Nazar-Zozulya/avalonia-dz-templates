using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;
using Avalonia.Threading;

namespace avalonia_dz_templates.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    // день недели, число, время
    private string _currentTime = "";
    private string _currentDate = "";
    private string _currentDayOfWeek = "";
    
    public string CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }
    
    public string CurrentDate
    {
        get => _currentDate;
        set => this.RaiseAndSetIfChanged(ref _currentDate, value);
    }
    
    public string CurrentDayOfWeek
    {
        get => _currentDayOfWeek;
        set => this.RaiseAndSetIfChanged(ref _currentDayOfWeek, value);
    }
    
    
    public ObservableCollection<CityViewModel> Cities { get; }
    
    private CityViewModel _selectedCity;

    public CityViewModel SelectedCity
    {
        get => _selectedCity;
        set => this.RaiseAndSetIfChanged(ref _selectedCity, value);
    }
    
    public MainWindowViewModel()
    {
        Cities = new ObservableCollection<CityViewModel>
        {
            new CityViewModel("Київ", 14,1 ,16,"description", "avares://avalonia-dz-templates/Assets/sun.png"),
            new CityViewModel("Дніпро", 15,2 ,17,"description2", "avares://avalonia-dz-templates/Assets/cloudy.png"),
            new CityViewModel("Харків", 16,3 ,18,"description3", "avares://avalonia-dz-templates/Assets/sun.png"),
            new CityViewModel("Донбасс", 17,4 ,19,"description4", "avares://avalonia-dz-templates/Assets/cloudy.png")
        };
        
        SelectedCity = Cities.FirstOrDefault();
        StartClock();
    }

    public void StartClock()
    {
        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (sender, e) => UpdateTime();
        timer.Start();
        UpdateTime();
        
    }

    public void UpdateTime()
    {
        CurrentTime = DateTime.Now.ToString("HH:mm");
        CurrentDate = DateTime.Now.ToString("dd.MM.yyyy");
        
        var culture = new System.Globalization.CultureInfo("uk-UA");
        string day = DateTime.Now.ToString("dddd", culture);
        
        
        CurrentDayOfWeek = char.ToUpper(day[0]) + day.Substring(1);
    }

}
