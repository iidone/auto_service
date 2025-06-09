using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using DynamicData;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class AddMaintenanceWindowViewModel : ReactiveObject
{
    
    public ObservableCollection<ClientModel> Clients { get; } = new();
    public ObservableCollection<MasterModel> Masters { get; } = new();
    private MaintenancesService _service;
    
    private ClientModel _selectedClient;
    public ClientModel SelectedClient
    {
        get => _selectedClient;
        set => this.RaiseAndSetIfChanged(ref _selectedClient, value);
    }
    
    private MasterModel _selectedMaster;
    public MasterModel SelectedMaster
    {
        get => _selectedMaster;
        set => this.RaiseAndSetIfChanged(ref _selectedMaster, value);
    }
    public string WorkDescription { get; set; }
    public string MaintenanceDate { get; set; }
    public string NextMaintenanceDate { get; set; }
    public string Comment { get; set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    
    public AddMaintenanceWindowViewModel(
        ClientService clientService,
        MasterService masterService,
        MaintenancesService maintenancesService)
    {
        _service = maintenancesService;
        
        LoadClients(clientService);
        LoadMasters(masterService);
        
        SaveCommand = ReactiveCommand.CreateFromTask(SaveMaintenance);
    }
    
    private async void LoadClients(ClientService service)
    {
        var clients = await service.GetAllClients();
        Clients.AddRange(clients);
    }
    
    private async void LoadMasters(MasterService service)
    {
        var masters = await service.GetAllMasters();
        Masters.AddRange(masters);
    }
    
    private async Task SaveMaintenance()
    {
        try
        {
            if (SelectedClient == null || SelectedMaster == null)
            {
                Console.WriteLine("Клиент или мастер не выбраны");
                return;
            }

            int ClientId = SelectedClient.id;
            int masterId = SelectedMaster.id;
            var NewWork = new AddMaintenanceRequest
            {
                user_id = masterId,
                client_id = ClientId,
                description = WorkDescription,
                date = MaintenanceDate,
                next_maintenance = NextMaintenanceDate,
                comment = Comment,
                price = "0",
                status = "В работе"
            };
            bool isSuccess = await _service.AddMaintenance(NewWork);
            if (isSuccess)
            {
                Console.WriteLine("Added Maintenance");
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


    }
}