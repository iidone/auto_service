using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Auto_Service.ViewModels;

public class AdminWindowViewModel : ReactiveObject
{
    private readonly MasterService _masterService;
    private readonly UserService _userService;
    private readonly ManagerService _managerService;
    private readonly ClientService _clientService;
    private readonly SparePartsService _sparePartsService;
    private readonly MaintenancesService _maintenancesService;
    
    private ObservableCollection<UsersModel> _users = new();
    private ObservableCollection<MasterModel> _masters = new();
    private ObservableCollection<ManagersModel> _managers = new();
    private ObservableCollection<ClientModel> _clients = new();
    private ObservableCollection<SparePartsModel> _spareParts = new();
    private ObservableCollection<MaintenancesModel> _maintenances = new();
    
    public ObservableCollection<UsersModel> Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
    
    public ObservableCollection<MasterModel> Masters
    {
        get => _masters;
        set => this.RaiseAndSetIfChanged(ref _masters, value);
    }
    
    public ObservableCollection<ManagersModel> Managers
    {
        get => _managers;
        set => this.RaiseAndSetIfChanged(ref _managers, value);
    }
    
    public ObservableCollection<ClientModel> Clients
    {
        get => _clients;
        set => this.RaiseAndSetIfChanged(ref _clients, value);
    }
    
    public ObservableCollection<SparePartsModel> SpareParts
    {
        get => _spareParts;
        set => this.RaiseAndSetIfChanged(ref _spareParts, value);
    }
    
    public ObservableCollection<MaintenancesModel> Maintenances
    {
        get => _maintenances;
        set => this.RaiseAndSetIfChanged(ref _maintenances, value);
    }
    
    public ReactiveCommand<Unit, Unit> LoadUsersCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadMastersCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadManagersCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadClientsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadSparePartsCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadMaintenancesCommand { get; }
    
    public AdminWindowViewModel(
        MasterService masterService, 
        UserService userService, 
        ManagerService managerService,
        ClientService clientService,
        SparePartsService sparePartsService,
        MaintenancesService maintenancesService)
    {
        _masterService = masterService;
        _userService = userService;
        _managerService = managerService;
        _clientService = clientService;
        _sparePartsService = sparePartsService;
        _maintenancesService = maintenancesService;

        LoadUsersCommand = ReactiveCommand.CreateFromTask(ViewAllUsers);
        LoadMastersCommand = ReactiveCommand.CreateFromTask(ViewAllMasters);
        LoadManagersCommand = ReactiveCommand.CreateFromTask(ViewAllManagers);
        LoadClientsCommand = ReactiveCommand.CreateFromTask(ViewAllClients);
        LoadSparePartsCommand = ReactiveCommand.CreateFromTask(ViewAllSpareParts);
        LoadMaintenancesCommand = ReactiveCommand.CreateFromTask(ViewAllMaintenances);

        LoadUsersCommand.Execute().Subscribe();
        LoadMastersCommand.Execute().Subscribe();
        LoadManagersCommand.Execute().Subscribe();
        LoadClientsCommand.Execute().Subscribe();
        LoadSparePartsCommand.Execute().Subscribe();
        LoadMaintenancesCommand.Execute().Subscribe();
    }
    
    private async Task ViewAllUsers()
    {
        try
        {
            Console.WriteLine("Загрузка пользователей...");
            
            var users = await _userService.GetAllUsers();
            
            if (users == null || users.Count == 0)
            {
                Console.WriteLine("Получен пустой список пользователей");
                Users.Clear();
                return;
            }
            
            Users = new ObservableCollection<UsersModel>(users);
            Console.WriteLine($"Успешно загружено {users.Count} пользователей");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке пользователей: {e}");
        }
    }
    
    private async Task ViewAllMasters()
    {
        try
        {
            Console.WriteLine("Начало загрузки мастеров...");
            
            var masters = await _masterService.GetAllMasters();
            
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
            Console.WriteLine($"Ошибка при загрузке мастеров: {e}");
        }
    }
    
    private async Task ViewAllManagers()
    {
        try
        {
            Console.WriteLine("Начало загрузки менеджеров...");
            
            var managers = await _managerService.GetAllManagers();
            
            if (managers == null || managers.Count == 0)
            {
                Console.WriteLine("Получен пустой список менеджеров");
                Managers.Clear();
                return;
            }
            
            Managers = new ObservableCollection<ManagersModel>(managers);
            Console.WriteLine($"Успешно загружено {managers.Count} менеджеров");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке менеджеров: {e}");
        }
    }
    
    private async Task ViewAllClients()
    {
        try
        {
            Console.WriteLine("Начало загрузки клиентов...");
            
            var clients = await _clientService.GetAllClients();
            
            if (clients == null || clients.Count == 0)
            {
                Console.WriteLine("Получен пустой список клиентов");
                Clients.Clear();
                return;
            }
            
            Clients = new ObservableCollection<ClientModel>(clients);
            Console.WriteLine($"Успешно загружено {clients.Count} клиентов");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке клиентов: {e}");
        }
    }
    
    private async Task ViewAllSpareParts()
    {
        try
        {
            Console.WriteLine("Начало загрузки запчастей...");
            
            var spareParts = await _sparePartsService.GetAllSpareParts();
            
            if (spareParts == null || spareParts.Count == 0)
            {
                Console.WriteLine("Получен пустой список запчастей");
                SpareParts.Clear();
                return;
            }
            
            SpareParts = new ObservableCollection<SparePartsModel>(spareParts);
            Console.WriteLine($"Успешно загружено {spareParts.Count} запчастей");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке запчастей: {e}");
        }
    }
    
    private async Task ViewAllMaintenances()
    {
        try
        {
            Console.WriteLine("Начало загрузки ТО...");
            
            var maintenances = await _maintenancesService.GetAllMaintenances();
            
            if (maintenances == null || maintenances.Count == 0)
            {
                Console.WriteLine("Получен пустой список ТО");
                Maintenances.Clear();
                return;
            }
            
            Maintenances = new ObservableCollection<MaintenancesModel>(maintenances);
            Console.WriteLine($"Успешно загружено {maintenances.Count} ТО");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке ТО: {e}");
        }
    }
}