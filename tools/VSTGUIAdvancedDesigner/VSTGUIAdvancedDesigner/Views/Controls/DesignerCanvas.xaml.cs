using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VSTGUIAdvancedDesigner.Models;
using VSTGUIAdvancedDesigner.ViewModels;

namespace VSTGUIAdvancedDesigner.Views.Controls;

public partial class DesignerCanvas : UserControl
{
    public DesignerCanvas()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        DataContextChanged += OnDataContextChanged;
    }

    private DesignerCanvasViewModel? ViewModel => DataContext as DesignerCanvasViewModel;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.HostElement = DesignSurface;
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.HostElement = DesignSurface;
        }
    }

    private void OnControlClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is ControlModel control && ViewModel != null)
        {
            ViewModel.SelectedControl = control;
            ViewModel.SelectedLayer = control.ParentLayer;
            e.Handled = true;
        }
    }
}
