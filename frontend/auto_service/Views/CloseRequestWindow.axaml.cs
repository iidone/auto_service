using System.Net.Http;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class CloseRequestWindow : Window
{
    public CloseRequestWindow(int workId)
    {
        InitializeComponent();
        var maintenanceService = new MaintenancesService(new HttpClient());
        var partsService = new SparePartsService(new HttpClient());
        var current_window = this;
        DataContext = new CloseRequestViewModel(partsService, workId, current_window, maintenanceService);
    }
}