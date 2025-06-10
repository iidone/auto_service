using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using ReactiveUI;
using Avalonia;
using System.Reactive;
using System.Windows.Input;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace Auto_Service.ViewModels;

public class LoginWindowViewModel : ReactiveObject
{
    private string _username = "";
    private string _password = "";
    private string _errorMessage = "";
    private bool _isLoading;
    private readonly IWindowService _windowService;
    private static int _user_id;
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
    public ReactiveCommand<Window, Unit> LoginCommand { get; }

    public LoginWindowViewModel(AuthService authService,  IWindowService windowService)
    {
        _windowService = windowService;
        LoginCommand = ReactiveCommand.CreateFromTask<Window>(async (currentWindow) =>
        {
            try
            {
                _isLoading = true;
                ErrorMessage = "";

                var result = await authService.LoginAsync(Username, Password);
                
                
                if (string.IsNullOrEmpty(result.Error))
                {
                    UserInfoService.UserId = result.user_info.user_id;
                    UserInfoService.FirstName = result.user_info.first_name;
                    UserInfoService.Role = result.user_info.user_role;
                    TokenStorageService.AuthToken = result.access_token;
                    _user_id = result.user_info.user_id;
                    var mainWindow = CreateWindowForRole(result.user_info.user_role, currentWindow);
                    mainWindow.Show();
                    _windowService.CloseWindow(currentWindow);
                }
                else
                {
                    ErrorMessage = result.Error;
                }

                _isLoading = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        });
    }

    private Window CreateWindowForRole(string role, Window OwnerWindow)
    {
        var windowService = new WindowService(OwnerWindow);
        var maintenanceService = new MaintenancesService(new HttpClient());
        var userService = new UserService(new HttpClient());
        var masterService = new MasterService(new HttpClient());
        var managerService = new ManagerService(new HttpClient());
        var clientService = new ClientService(new HttpClient());
        var sparePartsService = new SparePartsService(new HttpClient());

        switch (role)
        {
            case "master":
                return new MasterWindow(_user_id) 
                { 
                    DataContext = new MasterWindowViewModel(maintenanceService, _user_id) 
                };
        
            case "manager":
                var managerWindow = new ManagerWindow();
                var exportService = new ExportService(managerWindow);
                managerWindow.DataContext = new ManagerWindowViewModel(
                    masterService, 
                    windowService, 
                    maintenanceService, 
                    clientService, 
                    exportService
                );
                return managerWindow;
        
            case "admin":
                return new AdminWindow() 
                { 
                    DataContext = new AdminWindowViewModel(
                        masterService,
                        userService,
                        managerService,
                        clientService,
                        sparePartsService,
                        maintenanceService
                    )
                };
        
            case "storekeeper":
                return new StoreWindow() 
                { 
                    DataContext = new StoreWindowViewModel(sparePartsService)
                };
        
            default:
                return new MasterWindow(_user_id) 
                { 
                    DataContext = new MasterWindowViewModel(maintenanceService, _user_id) 
                };
        }
    }

}