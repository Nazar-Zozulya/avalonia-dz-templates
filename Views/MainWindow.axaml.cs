using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;

namespace avalonia_dz_templates.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TopBar_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e) // для перетягування вікна мишкою
    {
        // дозволяє рухати вікно при натисканні на верхню панель
        BeginMoveDrag(e);
    }

    // Закриття
    private void Close_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    // Згортання
    private void Minimize_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = Avalonia.Controls.WindowState.Minimized;
    }

    // Розгортання на весь екран або повернення до нормального розміру
    private void Maximize_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState == Avalonia.Controls.WindowState.Maximized
            ? Avalonia.Controls.WindowState.Normal
            : Avalonia.Controls.WindowState.Maximized;
    }

    
    

    public void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            foreach (var item in listBox.Items)
            {
                var container = listBox.ContainerFromItem(item) as ListBoxItem;
                if (container == null)
                    continue;

                var rectangle = container.GetVisualDescendants()
                    .OfType<Rectangle>()
                    .FirstOrDefault();

                if (rectangle == null)
                    continue;
                
                if (item == listBox.SelectedItem)
                {
                    rectangle.Opacity = 0;
                }
                else
                {
                    rectangle.Opacity = 0.2;
                }
            }
        }
    }
}