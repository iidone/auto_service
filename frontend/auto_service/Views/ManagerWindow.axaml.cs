using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class ManagerWindow : Window
{
    public ManagerWindow()
    {
        this.CanResize = true;
        this.Height = 650;
        this.Width = 1500;
        InitializeComponent();
        var _client_service = new ClientService(new HttpClient());
        var _maintenance_service = new MaintenancesService(new HttpClient());
        var window_service = new WindowService(this);
        var _service = new MasterService(new HttpClient());
        DataContext = new ManagerWindowViewModel(_service, window_service,  _maintenance_service,  _client_service);
    }
}