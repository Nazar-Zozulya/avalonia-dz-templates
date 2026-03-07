using Avalonia.Controls;
using Avalonia.Interactivity;

namespace avalonia_dz_templates.Views;

public partial class SettingsModalView : Window
{

    public SettingsModalView()
    {
        InitializeComponent();
    }

    public void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}