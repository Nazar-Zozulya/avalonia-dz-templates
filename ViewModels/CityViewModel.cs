using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

        private Bitmap _img;

        public Bitmap Img
        {
            get => _img;
            set => this.RaiseAndSetIfChanged(ref _img, value);
        }

        public CityViewModel(string name, int temperature, int minTemp, int maxTemp, string description, string imgPath)
        {
            Name = name;
            Temperature = temperature;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
            Description = description;
            Img = LoadImg(imgPath);
        }

        private Bitmap LoadImg(string path)
        {
            try
            {
                return new Bitmap(AssetLoader.Open(new Uri(path)));
            }
            catch (Exception)
            {
                Console.WriteLine(0);
                return null;
            }
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

