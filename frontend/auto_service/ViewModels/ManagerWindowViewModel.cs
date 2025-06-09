using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Views;
using Avalonia.Controls;

namespace Auto_Service.ViewModels;

public class ManagerWindowViewModel : ReactiveObject, IDisposable
{
    public ReactiveCommand<Unit, Unit> ViewAllMastersCommand;
    public ReactiveCommand<Unit, Unit> OpenAddMasterCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddClientCommand { get; }
    public ReactiveCommand<Unit, Unit> ViewAllWorksCommand { get; }
    public ReactiveCommand<Unit, Unit> ViewAllClientsCommand { get; }
    private readonly MasterService _service;
    private readonly MaintenancesService _maintenancesService;
    private readonly ClientService _clientService;
    private IWindowService _windowService;
    private ObservableCollection<MasterModel> _masters = new();
    private ObservableCollection<WorkMasterResponce> _works = new();
    private ObservableCollection<ClientModel> _clients = new();
    
    
    public ObservableCollection<MasterModel> Masters
    {
        get => _masters;
        set => this.RaiseAndSetIfChanged(ref _masters, value);
    }

    public ObservableCollection<WorkMasterResponce> Works
    {
        get => _works;
        set => this.RaiseAndSetIfChanged(ref _works, value);
    }

    public ObservableCollection<ClientModel> Clients
    {
        get => _clients;
        set => this.RaiseAndSetIfChanged(ref _clients, value);
    }
    
    
    public ManagerWindowViewModel(MasterService service,  IWindowService windowService,  MaintenancesService maintenancesService, ClientService clientService)
    {
        _maintenancesService = maintenancesService;
        _service = service;
        _windowService = windowService;
        _clientService = clientService;
        _clientService.ClientsChanged += OnClientsChanged;
        _service.MasterChanged += OnMastersChanged;
        OpenAddMasterCommand = ReactiveCommand.Create(() =>
        {
            var addMasterWindow = new AddMasterWindow();
            addMasterWindow.DataContext = new AddMasterWindowViewModel(_service, _windowService, addMasterWindow);
            addMasterWindow.Show();
        });
        OpenAddClientCommand = ReactiveCommand.Create(() =>
        {
            var addClientWindow = new AddClientWindow();
            addClientWindow.DataContext = new AddClientWindowViewModel(_clientService, _windowService);
            addClientWindow.Show();
        });
        
        ViewAllMastersCommand = ReactiveCommand.CreateFromTask(ViewAllMasters);
        ViewAllWorksCommand = ReactiveCommand.CreateFromTask(ViewAllWorks);
        ViewAllClientsCommand = ReactiveCommand.CreateFromTask(ViewAllClients);
        LoadInitialData();
    }
    
    private void LoadInitialData()
    {
        Console.WriteLine("Запуск начальной загрузки данных");
        ViewAllMastersCommand.Execute().Subscribe(
            onNext: _ => Console.WriteLine("Данные успешно загружены"),
            onError: ex => Console.WriteLine($"Ошибка загрузки: {ex}"));
        ViewAllWorksCommand.Execute().Subscribe(
            onNext: _ => Console.WriteLine("Данные успешно загружены"),
            onError: ex => Console.WriteLine($"Ошибка загрузки: {ex}"));
        ViewAllClientsCommand.Execute().Subscribe(
            onNext: _ => Console.WriteLine("Данные успешно загружены"),
            onError: ex => Console.WriteLine($"Ошибка загрузки: {ex}"));
    }
    
    
    private async void OnMastersChanged()
    {
        await ViewAllMasters();
    }

    private async void OnClientsChanged()
    {
        await ViewAllClients();
    }
    
    public void Dispose()
    {
        _service.MasterChanged -= OnMastersChanged;
        _clientService.ClientsChanged -= OnClientsChanged;
    }


    private async Task ViewAllMasters()
    {
        try
        {
            Console.WriteLine("Начало загрузки мастеров...");
            
            var masters = await _service.GetAllMasters();
            
            if (masters == null || masters.Count == 0)
            {
                Console.WriteLine("Получен пустой список мастеров");
                Masters.Clear();
                return;
            }
            
            Masters = new ObservableCollection<MasterModel>(masters);
            Console.WriteLine($"Успешно загружено {masters.Count} мастеров");
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task ViewAllWorks()
    {
        try
        {
            Console.WriteLine("Starting loading works...");
            var works = await _maintenancesService.GetAllWorks();

            if (works == null || works.Count == 0)
            {
                Console.WriteLine("Null list");
                return;
            }

            Works = new ObservableCollection<WorkMasterResponce>(works);
            Console.WriteLine($"Loaded: {works.Count} works");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task ViewAllClients()
    {
        try
        {
            Console.WriteLine("Starting loading works...");
            var clients = await _clientService.GetAllClients();

            if (clients == null || clients.Count == 0)
            {
                Console.WriteLine("Null list");
                return;
            }

            Clients = new ObservableCollection<ClientModel>(clients);
            Console.WriteLine($"Loaded: {clients.Count} works");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}