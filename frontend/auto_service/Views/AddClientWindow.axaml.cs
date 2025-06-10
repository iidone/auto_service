using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class AddClientWindow : Window
{
    public AddClientWindow()
    {
        InitializeComponent();
        var windowService = new WindowService(this);
        var service = new ClientService(new HttpClient());
        DataContext = new AddClientWindowViewModel(service, windowService);
    }
}