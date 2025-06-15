using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

public class AdminWindowViewModel : ReactiveObject
{
    private readonly MasterService _masterService;
    private readonly UserService _userService;
    private readonly ManagerService _managerService;
    private readonly ClientService _clientService;
    private readonly SparePartsService _sparePartsService;
    private readonly MaintenancesService _maintenancesService;
    private readonly ExportService _exportService;
    private Window _window;
    
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
    public ReactiveCommand<Unit, Unit> OpenAddUserCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportUsersCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportMastersCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddMasterCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddClientCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportClientsCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddMaintenanceCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportMaintenancesCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddManagerCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportManagerCommand { get; } 
    public ReactiveCommand<Unit, Unit> OpenAddSparePartsCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportPartsCommand { get; }

    public AdminWindowViewModel(
        MasterService masterService,
        UserService userService,
        ManagerService managerService,
        ClientService clientService,
        SparePartsService sparePartsService,
        MaintenancesService maintenancesService,
        ExportService exportService,
        Window window)
    {
        _exportService = exportService;
        _window = window;
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
        ExportUsersCommand = ReactiveCommand.CreateFromTask(ExportUserExcel);
        ExportMastersCommand = ReactiveCommand.CreateFromTask(ExportMasterExcel);
        ExportClientsCommand = ReactiveCommand.CreateFromTask(ExportClientExcel);
        ExportMaintenancesCommand = ReactiveCommand.CreateFromTask(ExportMaintenanceExcel);
        ExportManagerCommand = ReactiveCommand.CreateFromTask(ExportManagerExcel);
        ExportPartsCommand = ReactiveCommand.CreateFromTask(ExportPartsExcel);
        

        LoadUsersCommand.Execute().Subscribe();
        LoadMastersCommand.Execute().Subscribe();
        LoadManagersCommand.Execute().Subscribe();
        LoadClientsCommand.Execute().Subscribe();
        LoadSparePartsCommand.Execute().Subscribe();
        LoadMaintenancesCommand.Execute().Subscribe();

        OpenAddUserCommand = ReactiveCommand.Create(() =>
        {
            var _MasterService = new MasterService(new HttpClient());
            var addUserWindow = new AddUserWindow();
            addUserWindow.DataContext = new AddUserViewModel(addUserWindow, _MasterService);
            addUserWindow.Show();

        });
        OpenAddMasterCommand = ReactiveCommand.Create(() =>
        {
            var Window = new AddMasterWindow();
            var window_service = new WindowService(Window);
            var addMasterWindow = new AddMasterWindow();
            addMasterWindow.DataContext = new AddMasterWindowViewModel(_masterService, window_service, addMasterWindow);
            addMasterWindow.Show();
        });
        OpenAddClientCommand = ReactiveCommand.Create(() =>
        {
            var addClientWindow = new AddClientWindow();
            var window_service = new WindowService(addClientWindow);
            addClientWindow.DataContext = new AddClientWindowViewModel(_clientService, window_service, addClientWindow);
            addClientWindow.Show();
        });
        OpenAddMaintenanceCommand = ReactiveCommand.Create(() =>
        {
            var _maintenance_service = new MaintenancesService(new HttpClient());
            var _masterService = new MasterService(new HttpClient());
            var addMaintenanceWindow = new AddMaintenanceWindow();
            var window_service = new WindowService(addMaintenanceWindow);
            addMaintenanceWindow.DataContext = new AddMaintenanceWindowViewModel(_clientService, masterService,  _maintenancesService, addMaintenanceWindow, window_service);
            addMaintenanceWindow.Show();
        });
        OpenAddManagerCommand = ReactiveCommand.Create(() =>
        {
            var Window = new AddManagerWindow();
            var window_service = new WindowService(Window);
            var addMasterWindow = new AddManagerWindow();
            addMasterWindow.DataContext = new AddManagerWindowViewModel(_masterService, window_service, addMasterWindow);
            addMasterWindow.Show();
        });
        OpenAddSparePartsCommand = ReactiveCommand.Create(() =>
        {
            var window =  new AddSparePartWindow();
            var window_service = new WindowService(window);
            var addSparePartWindow = new AddSparePartWindow();
            addSparePartWindow.DataContext = new AddSparePartWindowViewModel(sparePartsService,  window_service, addSparePartWindow);
            addSparePartWindow.Show();
        });


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

    private async Task ExportUserExcel()
    {
        try 
        {
            if (Users == null || !Users.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in Users)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("users_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath,
                    excludedFields: new List<string> { "password"});
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
    private async Task ExportMasterExcel()
    {
        try 
        {
            if (Masters == null || !Masters.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in Masters)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("masters_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath,
                    excludedFields: new List<string> { "password"});
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
    private async Task ExportClientExcel()
    {
        try 
        {
            if (Clients == null || !Clients.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in Clients)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("clients_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath);
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
    private async Task ExportMaintenanceExcel()
    {
        try 
        {
            if (Maintenances == null || !Maintenances.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in Maintenances)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("maintenances_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath);
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
    private async Task ExportManagerExcel()
    {
        try 
        {
            if (Managers == null || !Managers.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in Managers)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("managers_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath,
                    excludedFields: new List<string> { "password"});
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
    private async Task ExportPartsExcel()
    {
        try 
        {
            if (SpareParts == null || !SpareParts.Any())
            {
                Console.WriteLine("Нет данных для экспорта");
                return;
            }
            var itemsToExport = new List<object>();
            foreach (var item in SpareParts)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("parts_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath,
                    excludedFields: new List<string> { "IsSelected", "Changing", "Changed", "ThrownExceptions", "PriceDecimal"});
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
        
    }
}