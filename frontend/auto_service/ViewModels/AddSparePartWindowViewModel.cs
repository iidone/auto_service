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

public class AddSparePartWindowViewModel : ReactiveObject
{
    private readonly SparePartsService _service;
    private readonly IWindowService _windowService;
    private readonly Window _currentWindow;

    public SparePartsModel NewPart { get; } = new SparePartsModel { };
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public AddSparePartWindowViewModel(SparePartsService service, IWindowService windowService, Window currentWindow)
    {
        _service = service;
        _windowService = windowService;
        _currentWindow = currentWindow;
        
        
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                bool isSuccess = await _service.AddPart(NewPart);
        
                if (isSuccess)
                {
                    Console.WriteLine("Part successfully added");
                    _windowService.CloseWindow(_currentWindow);
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