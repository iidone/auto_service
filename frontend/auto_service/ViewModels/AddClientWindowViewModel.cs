using System;
using System.Net.Http;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class AddClientWindowViewModel : ReactiveObject
{
    public ClientModel NewClient { get; set; } = new ClientModel();
    private readonly IWindowService _windowService;
    private readonly ClientService _service;
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
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public AddClientWindowViewModel(ClientService service, IWindowService windowService)
    {
        _windowService = windowService;
        _service = service;

        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                
                bool isSuccess = await _service.AddClient(NewClient);
        
                if (isSuccess)
                {
                    Console.WriteLine($"[SUCCESS] <UNK> <UNK> <UNK> <UNK> <UNK> <UNK>");
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





}