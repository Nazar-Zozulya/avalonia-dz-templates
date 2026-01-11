using ReactiveUI;

namespace avalonia_dz_templates.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        private string _name;

        private int _temperature;

        public int MaxTemp { get; set; }

        public int MinTemp { get; set; }

        public string Description { get; set; }

        public CityViewModel(string name, int temperature, int minTemp, int maxTemp, string description)
        {
            Name = name;
            Temperature = temperature;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            Description = description;
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public int Temperature
        {
            get => _temperature;
            set => this.RaiseAndSetIfChanged(ref _temperature, value);
        }

    }
}

