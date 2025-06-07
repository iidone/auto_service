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
        this.CanResize = false;
        this.Width = 800;
        this.Height = 600;
        InitializeComponent();
        var _service = new MasterService(new HttpClient());
        DataContext = new ManagerWindowViewModel(_service);
    }
}