using System.Net.Http;
using Auto_Service.Services;
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
        var window = this;
        var window_service = new WindowService(window);
        var maintenance_service = new MaintenancesService(new HttpClient());
        var client_service = new ClientService(new HttpClient());
        var master_service = new MasterService(new HttpClient());
        DataContext = new AddMaintenanceWindowViewModel(client_service, master_service, maintenance_service, this, window_service );
    }
}