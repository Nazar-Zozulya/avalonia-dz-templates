using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using LiveChartsCore;
using System.Linq;
using Avalonia.Markup.Xaml;
using avalonia_dz_templates.ViewModels;
using avalonia_dz_templates.Views;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace avalonia_dz_templates;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    LiveCharts.Configure(config =>
    {
        config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('a'));

        config.HasMap<HourlyForecastViewModel>((forecast, index) =>  
            new LiveChartsCore.Kernel.Coordinate(index, forecast.Temprature)
        );
    });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}