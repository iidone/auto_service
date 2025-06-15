using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Auto_Service.Views;

public partial class AddSparePartWindow : Window
{
    public AddSparePartWindow()
    {
        var _service = new SparePartsService(new HttpClient());
        var window_service = new WindowService(this);
        InitializeComponent();
        DataContext = new AddSparePartWindowViewModel(_service, window_service, this);
    }
}