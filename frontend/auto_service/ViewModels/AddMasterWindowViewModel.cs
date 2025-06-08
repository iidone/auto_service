using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using ReactiveUI;
using Avalonia;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Material.Avalonia;

namespace Auto_Service.ViewModels;

public class AddMasterWindowViewModel : ReactiveObject
{
    private readonly MasterService _service;
    private readonly IWindowService _windowService;
    private readonly Window _currentWindow;
    
    public MasterModel NewMaster { get; } = new MasterModel 
    { 
        role = "master" 
    };
    
    private string _lastName;
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }
        
    private string _firstName;
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }
        
    private string _contact;
    public string Contact
    {
        get => _contact;
        set => this.RaiseAndSetIfChanged(ref _contact, value);
    }

    private string _username;
    
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private string _password;

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public AddMasterWindowViewModel(MasterService masterService, IWindowService windowService, Window currentWindow)
    {
        _service = masterService;
        _windowService = windowService;
        _currentWindow = currentWindow;
        
        
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                
                NewMaster.username = GenerateUsername();
                NewMaster.password = GeneratePassword();
                bool isSuccess = await _service.AddMaster(NewMaster);
        
                if (isSuccess)
                {
                    await ShowSuccessDialog();
                }
                else
                {
                    Console.WriteLine("[ERROR] Сервер вернул ошибку при сохранении");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[NETWORK ERROR] Ошибка соединения: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UNHANDLED ERROR] Критическая ошибка: {ex}");
                throw;
            }
            
        });
        CancelCommand = ReactiveCommand.Create(() => { });
    }
    
    
    private string GenerateUsername() 
        => $"{NewMaster.first_name.ToLower()}.{NewMaster.last_name.ToLower()}";
    
    private string GeneratePassword()
        => Guid.NewGuid().ToString().Substring(0, 8);
    
    private async Task ShowSuccessDialog()
    {
        var dialog = new Window
        {
            Title = "Успешное создание",
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { 
                        Text = $"Имя пользователя: {NewMaster.username}", 
                        Margin = new Thickness(10) 
                    },
                    new TextBlock { 
                        Text = $"Пароль: {NewMaster.password}", 
                        Margin = new Thickness(10) 
                    },
                    new Button 
                    { 
                    }
                }
            }
        };
        var okButton = (Button)((StackPanel)dialog.Content).Children[2];
        okButton.Content = "Закрыть";
        okButton.Command = ReactiveCommand.Create(() => dialog.Close());
        okButton.HorizontalAlignment = HorizontalAlignment.Center;
        okButton.Margin = new Thickness(10);
        dialog.CanResize = false;
        

        await dialog.ShowDialog(_currentWindow);
    }
}