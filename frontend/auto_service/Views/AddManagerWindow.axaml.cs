using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Auto_Service.Views;

public partial class AddManagerWindow : Window
{
    public AddManagerWindow()
    {
        InitializeComponent();
        var window_service = new WindowService(this);
        var _service = new MasterService(new HttpClient());
        DataContext = new AddManagerWindowViewModel(_service, window_service, this);
    }
}