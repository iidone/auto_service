using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class AddMaintenanceWindow : Window
{
    public AddMaintenanceWindow()
    {
        InitializeComponent();
        DataContext = new AddMaintenanceWindowViewModel();
    }
}