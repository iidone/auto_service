using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO;
using System.Net.Http;
using Auto_Service.Services;
using Auto_Service.ViewModels;
using Avalonia.Interactivity;

namespace Auto_Service.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        this.CanResize = false;
        this.Width = 800;
        this.Height = 600;
        InitializeComponent();
        var windowService = new WindowService();
        var authService = new AuthService(new HttpClient());
        DataContext = new LoginWindowViewModel(authService, windowService);
    }
    
}