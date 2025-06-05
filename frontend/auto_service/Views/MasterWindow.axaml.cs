using System.Net.Http;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class MasterWindow : Window
{
    public MasterWindow(int user_id)
    {
        this.CanResize = false;
        this.Width = 800;
        this.Height = 600;
        var _service = new MaintenancesService(new HttpClient());
        DataContext = new MasterWindowViewModel(_service, user_id );
        InitializeComponent();
    }
}