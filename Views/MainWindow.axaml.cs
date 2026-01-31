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