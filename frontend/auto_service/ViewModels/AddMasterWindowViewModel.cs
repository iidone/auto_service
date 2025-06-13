using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using ReactiveUI;
using Avalonia;
using System.Reactive;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
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
    public AddMasterWindowViewModel(MasterService masterService, IWindowService window_service, Window currentWindow)
    {
        _service = masterService;
        _currentWindow = currentWindow;
        _windowService = window_service;
        
        
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
    {
        string Transliterate(string text)
        {
            var translit = new Dictionary<string, string>
            {
                {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
                {"е", "e"}, {"ё", "yo"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"},
                {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
                {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
                {"у", "u"}, {"ф", "f"}, {"х", "kh"}, {"ц", "ts"}, {"ч", "ch"},
                {"ш", "sh"}, {"щ", "shch"}, {"ъ", ""}, {"ы", "y"}, {"ь", ""},
                {"э", "e"}, {"ю", "yu"}, {"я", "ya"}
            };

            return string.Concat(text.ToLower().Select(c => 
                translit.TryGetValue(c.ToString(), out var latin) ? latin : c.ToString()));
        }
        string CleanUsername(string username)
        {
            var cleaned = Regex.Replace(username, @"[^a-z0-9.]", "");
            return Regex.Replace(cleaned, @"\.+", ".").Trim('.');
        }
        
        var firstNameTranslit = Transliterate(NewMaster.first_name);
        var lastNameTranslit = Transliterate(NewMaster.last_name);
    
        var baseName = $"{firstNameTranslit}.{lastNameTranslit}";
        
        
        return CleanUsername($"{baseName}.{Guid.NewGuid().ToString("N").Substring(0, 4)}");

    }
    
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
                    new TextBox { 
                        Text = $"Имя пользователя: {NewMaster.username}", 
                        Margin = new Thickness(5),
                        IsReadOnly = true,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    },
                    new TextBox { 
                        Text = $"Пароль: {NewMaster.password}", 
                        Margin = new Thickness(10),
                        IsReadOnly = true,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
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