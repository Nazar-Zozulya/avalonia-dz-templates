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
            new CityViewModel("Київ", 14,1 ,16,"description"),
            new CityViewModel("Дніпро", 14,1 ,16,"description"),
            new CityViewModel("Харків", 14,1 ,16,"description"),
            new CityViewModel("Донбасс", 14,1 ,16,"description")
        };
        
        SelectedCity = Cities.FirstOrDefault();
    }

}
