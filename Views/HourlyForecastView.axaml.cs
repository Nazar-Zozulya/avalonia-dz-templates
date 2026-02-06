using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace avalonia_dz_templates.Views;

public partial class HourlyForecastView : UserControl
{
    public HourlyForecastView()
    {
        InitializeComponent();
    }
    
    private void Left_ButtonClick(object? sender, RoutedEventArgs e)
    {
        int forecastList = ForecastList.SelectedIndex;

        if (forecastList > 0)
        {
            ForecastList.SelectedIndex--;
            ForecastList.ScrollIntoView(forecastList);
            
        }
    }

    private void Right_ButtonClick(object? sender, RoutedEventArgs e)
    {
        int forecastList = ForecastList.SelectedIndex;

        if (forecastList < ForecastList.ItemCount -2)
        {
            ForecastList.SelectedIndex++;
            ForecastList.ScrollIntoView(forecastList);
            
        }
    }

}