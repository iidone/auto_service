using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DocumentFormat.OpenXml.Drawing.Charts;


namespace Auto_Service.Views;

public partial class StoreWindow : Window
{
    public StoreWindow()
    {
        InitializeComponent();
        var parts_service = new SparePartsService(new HttpClient());
        DataContext = new StoreWindowViewModel(parts_service);
    }
}