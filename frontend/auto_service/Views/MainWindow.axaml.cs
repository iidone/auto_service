using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.CanResize = false;
        this.Width = 800;
        this.Height = 600;
        DataContext = new MainWindowViewModel();
    }
}