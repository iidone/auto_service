using System;
using System.Collections.ObjectModel;
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
    private readonly MasterService _service;
    private IWindowService _windowService;
    private ObservableCollection<MasterModel> _masters = new();
    
    
    public ObservableCollection<MasterModel> Masters
    {
        get => _masters;
        set => this.RaiseAndSetIfChanged(ref _masters, value);
    }
    
    
    public ManagerWindowViewModel(MasterService service, IWindowService windowService)
    {
        _windowService =  windowService;
        _service = service;
        _service.MasterChanged += OnMastersChanged;
        OpenAddMasterCommand = ReactiveCommand.Create(() =>
        {
            var addMasterWindow = new AddMasterWindow();
            addMasterWindow.DataContext = new AddMasterWindowViewModel(_service, windowService, addMasterWindow);
            addMasterWindow.Show();
        });
        OpenAddClientCommand = ReactiveCommand.Create(() =>
        {
            var addClientWindow = new AddClientWindow();
            addClientWindow.DataContext = new AddClientWindow();
            addClientWindow.Show();
        });
        
        ViewAllMastersCommand = ReactiveCommand.CreateFromTask(ViewAllMasters);
        LoadInitialData();
    }
    
    private void LoadInitialData()
    {
        Console.WriteLine("Запуск начальной загрузки данных");
        ViewAllMastersCommand.Execute().Subscribe(
            onNext: _ => Console.WriteLine("Данные успешно загружены"),
            onError: ex => Console.WriteLine($"Ошибка загрузки: {ex}"));
    }
    
    
    private async void OnMastersChanged()
    {
        Console.WriteLine("Получено уведомление об изменении списка мастеров");
        await ViewAllMasters();
    }
    
    public void Dispose()
    {
        _service.MasterChanged -= OnMastersChanged;
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
    
}