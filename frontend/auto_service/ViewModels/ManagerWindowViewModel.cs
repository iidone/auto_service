using System;
using System.Collections;
using System.Collections.Generic;
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
    public ReactiveCommand<Unit, Unit> OpenAddWorkCommand { get; }
    public ReactiveCommand<Window, Unit> ExportMastersCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteMasterCommand { get; }
    
    private readonly MasterService _service;
    private readonly MaintenancesService _maintenancesService;
    private MasterModel _selectedMaster;
    private readonly ClientService _clientService;
    private readonly IExportService _exportService;
    private IWindowService _windowService;
    private ObservableCollection<MasterModel> _masters = new();
    private ObservableCollection<WorkMasterResponce> _works = new();
    private ObservableCollection<ClientModel> _clients = new();
    public MasterModel SelectedMaster
    {
        get => _selectedMaster;
        set => this.RaiseAndSetIfChanged(ref _selectedMaster, value);
    }
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
    
    
    public ManagerWindowViewModel(MasterService service,  IWindowService windowService,  MaintenancesService maintenancesService, ClientService clientService, IExportService exportService)
    {
        _maintenancesService = maintenancesService;
        _service = service;
        _exportService = exportService;
        _windowService = windowService;
        _clientService = clientService;
        _clientService.ClientsChanged += OnClientsChanged;
        _service.MasterChanged += OnMastersChanged;
        _maintenancesService.WorksChanged += OnWorksChanged;
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
        OpenAddWorkCommand = ReactiveCommand.Create(() =>
        {
            var _maintenance_service = new MaintenancesService(new HttpClient());
            var _masterService = new MasterService(new HttpClient());
            var addMaintenanceWindow = new AddMaintenanceWindow();
            addMaintenanceWindow.DataContext = new AddMaintenanceWindowViewModel(_clientService, _service,  _maintenancesService);
            addMaintenanceWindow.Show();
        });

        ExportMastersCommand = ReactiveCommand.CreateFromTask<Window>(ExportExcel);
        ViewAllMastersCommand = ReactiveCommand.CreateFromTask(ViewAllMasters);
        ViewAllWorksCommand = ReactiveCommand.CreateFromTask(ViewAllWorks);
        ViewAllClientsCommand = ReactiveCommand.CreateFromTask(ViewAllClients);
        
        DeleteMasterCommand = ReactiveCommand.CreateFromTask(DeleteSelectedMaster, 
            this.WhenAnyValue(x => x.SelectedMaster).Select(m => m != null));
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

    private async void OnWorksChanged()
    {
        await ViewAllWorks();
    }
    
    public void Dispose()
    {
        _service.MasterChanged -= OnMastersChanged;
        _clientService.ClientsChanged -= OnClientsChanged;
        _maintenancesService.WorksChanged -= OnWorksChanged;
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

    private async Task ExportExcel(Window currentWindow)
    {
        try 
        {
            
            var dataGrid = currentWindow.FindControl<DataGrid>("MastersDataGrid");
        
            if (dataGrid == null)
            {
                Console.WriteLine("DataGrid не найден!");
                return;
            }

            var itemSource = dataGrid.ItemsSource as IEnumerable;
            var itemsToExport = new List<object>();
            foreach (var item in itemSource)
            {
                itemsToExport.Add(item);
            }

            var filePath = await _exportService.GetExportFilePathAsync("masters_export.xlsx");
            if (!string.IsNullOrEmpty(filePath))
            {
                _exportService.ExportToExcel(
                    data: itemsToExport, 
                    filePath : filePath,
                    excludedFields: new List<string> { "username", "password" });
                Console.WriteLine("Данные успешно экспортированы!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
    }
    private async Task DeleteSelectedMaster()
    {
        try
        {
            var masterId = SelectedMaster.id;
            var isSuccess = await _service.DeleteMaster(new[] { SelectedMaster.id });
            if (isSuccess)
            {
                Console.WriteLine("Master is deleted");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}