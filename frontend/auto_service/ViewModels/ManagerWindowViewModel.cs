using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Auto_Service.ViewModels;

public class ManagerWindowViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> ViewAllMastersCommand;
    private readonly MasterService _service;
    private ObservableCollection<MasterModel> _masters = new();
    
    
    public ObservableCollection<MasterModel> Masters
    {
        get => _masters;
        set => this.RaiseAndSetIfChanged(ref _masters, value);
    }
    
    
    public ManagerWindowViewModel(MasterService service)
    {
        _service = service;
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