using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;

namespace avalonia_dz_templates.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    
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
            new CityViewModel("Київ", 14,1 ,16,"description", "avares://avalonia_dz_templates/Assets/sun.png"),
            new CityViewModel("Дніпро", 15,2 ,17,"description2", "avares://avalonia_dz_templates/Assets/cloudy.png"),
            new CityViewModel("Харків", 16,3 ,18,"description3", "avares://avalonia_dz_templates/Assets/sun.png"),
            new CityViewModel("Донбасс", 17,4 ,19,"description4", "avares://avalonia_dz_templates/Assets/cloudy.png")
        };
        
        SelectedCity = Cities.FirstOrDefault();
    }

}
