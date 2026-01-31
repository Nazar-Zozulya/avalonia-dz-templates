using System;
using ReactiveUI;


namespace avalonia_dz_templates.ViewModels
{
    public class DayForecastViewModel : ViewModelBase
    {
        private int _time;

        private int _temprature;

        public int Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }
        
        public int Temprature
        {
            get => _temprature;
            set => this.RaiseAndSetIfChanged(ref _temprature, value);
        }

        public DayForecastViewModel(int time, int temprature)
        {
            Time = time;
            Temprature = temprature;
        }
    }
}