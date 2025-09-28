using System.Windows;
using VSTGUIAdvancedDesigner.Services;
using VSTGUIAdvancedDesigner.ViewModels;

namespace VSTGUIAdvancedDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var services = ServiceRegistry.CreateDefault();
        DataContext = new MainViewModel(services);
    }
}
