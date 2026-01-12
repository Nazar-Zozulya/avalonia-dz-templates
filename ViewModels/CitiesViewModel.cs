using System.Collections.ObjectModel;
using avalonia_dz_templates.ViewModels;
using ReactiveUI;

namespace avalonia_dz_templates;


public partial class CitiesViewModel : ViewModelBase
{
    public ObservableCollection<CityViewModel> Cities { get; }
    
    public CitiesViewModel()
    {
        Cities = new ObservableCollection<CityViewModel>
        {
            new CityViewModel("Київ", 14,1 ,16,"description", "fdf")
        };
    }
}