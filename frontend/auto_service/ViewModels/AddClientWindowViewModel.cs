using System;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using System.Xml;
using Auto_Service.Models;
using Auto_Service.Services;
using Avalonia.Controls;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class AddClientWindowViewModel : ReactiveObject
{
    public ClientModel NewClient { get; } = new ClientModel{};
    private  IWindowService _windowService;
    private Window _window;
    private  ClientService _service;
    private string first_name;
    private string last_name;
    private string contact;
    private string brand;
    private string series;
    private string number;
    private string mileage;
    private string age;
    private string vin;
    private string last_maintenance;

    public string FirstName
    {
        get => first_name;
        set => this.RaiseAndSetIfChanged(ref first_name, value); 
    }
    public string LastName
    {
        get => last_name;
        set => this.RaiseAndSetIfChanged(ref last_name, value);
    }

    public string Contact
    {
        get => contact;
        set => this.RaiseAndSetIfChanged(ref contact, value);
    }

    public string Brand
    {
        get => brand;
        set => this.RaiseAndSetIfChanged(ref brand, value);
    }

    public string Series
    {
        get => series;
        set => this.RaiseAndSetIfChanged(ref series, value);
    }

    public string Number
    {
        get => number;
        set => this.RaiseAndSetIfChanged(ref number, value);
    }

    public string Mileage
    {
        get => mileage;
        set => this.RaiseAndSetIfChanged(ref mileage, value);
    }

    public string Age
    {
        get => age;
        set => this.RaiseAndSetIfChanged(ref age, value);
    }

    public string Vin
    {
        get => vin;
        set => this.RaiseAndSetIfChanged(ref vin, value);
    }

    public string LastMaintenance
    {
        get => last_maintenance;
        set => this.RaiseAndSetIfChanged(ref last_maintenance, value);
    }
    
    
    public ReactiveCommand<Unit, Unit> AddClientCommand { get;}
    public AddClientWindowViewModel(ClientService service, IWindowService windowService, Window  window)
    {
        _window = window;
        _windowService = windowService;
        _service = service;
        AddClientCommand = ReactiveCommand.CreateFromTask(SaveClient);
    }
    private async Task SaveClient()
    {
        try
        {
            var isSuccess = await _service.AddClient(NewClient);
            Console.WriteLine($"Client Added, {isSuccess}");
            if (isSuccess)
            {
                _windowService.CloseWindow(_window);
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
        
    }
    




}