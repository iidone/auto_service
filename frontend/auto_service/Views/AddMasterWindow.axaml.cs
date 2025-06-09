using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class AddMasterWindow : Window
{
    public AddMasterWindow()
    {
        InitializeComponent();
        var window_service = new WindowService();
        var _service = new MasterService(new HttpClient());
        
        DataContext = new AddMasterWindowViewModel(_service, window_service, this);
    }
}