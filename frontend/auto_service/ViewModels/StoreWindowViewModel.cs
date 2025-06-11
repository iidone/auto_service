using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class StoreWindowViewModel : ReactiveObject, IDisposable
{
    public SparePartsModel NewPart { get; } = new SparePartsModel { };
    private readonly SparePartsService _service;
    private readonly ObservableCollection<SparePartsModel> _allParts = new();
    public ObservableCollection<SparePartsModel> FilteredParts { get; }

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    private SparePartsModel _selectedPart;

    public SparePartsModel SelectedPart
    {
        get => _selectedPart;
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    private string _selectedCategory = "Все";

    public string SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    public ObservableCollection<string> Categories { get; } = new();
    public ReactiveCommand<Unit, Unit> LoadPartsCommand { get; }
    public ReactiveCommand<Unit, Unit> DeletePartCommand { get; }
    public ReactiveCommand<Unit, Unit> AddPartCommand { get; }

    public StoreWindowViewModel(SparePartsService service)
    {
        _service = service;
        _service.PartsUpdated += OnPartsUpdated;
        FilteredParts = new ObservableCollection<SparePartsModel>();
        Categories.Add("Все");
        AddPartCommand = ReactiveCommand.CreateFromTask(AddPart);
        LoadPartsCommand = ReactiveCommand.CreateFromTask(LoadParts);
        DeletePartCommand = ReactiveCommand.CreateFromTask(DeletePart,
            this.WhenAnyValue(x => x.SelectedPart).Select(m => m != null));

        LoadPartsCommand.Execute().Subscribe();

        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(SearchParts);
        this.WhenAnyValue(x => x.SelectedCategory)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ => ApplyFilters());
    }

    private async void OnPartsUpdated()
    {
        await LoadParts();
    }

    public void Dispose()
    {
        _service.PartsUpdated -= OnPartsUpdated;
    }

private async Task LoadParts()
    {
        try
        {
            Console.WriteLine("Start loading parts");
            var parts = await _service.GetAllSpareParts();
            _allParts.Clear();
            FilteredParts.Clear();
            if (parts != null && parts.Any())
            {
                foreach (var part in parts)
                {
                    _allParts.Add(part);
                    FilteredParts.Add(part);
                }
                foreach (var category in parts.Select(p => p.category).Distinct())
                {
                    if (!string.IsNullOrEmpty(category) && !Categories.Contains(category))
                    {
                        Categories.Add(category);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
    }

    private async Task AddPart()
    {
        try
        {
            bool isSuccess = await _service.AddPart(NewPart);
            if (isSuccess)
            {
                Console.WriteLine($"Part added, {isSuccess}");
                _allParts.Add(NewPart);
                FilteredParts.Add(NewPart);
                if (!Categories.Contains(NewPart.category))
                {
                    Categories.Add(NewPart.category);
                }
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

    private async Task DeletePart()
    {
        if (SelectedPart == null || _service == null) 
        {
            Console.WriteLine("Не выбран элемент или сервис не инициализирован");
            return;
        }
        try
        {
            var partId =  SelectedPart.Id;
            var isSuccess = await _service.DeletePart(new [] { partId });
            if (isSuccess)
            {
                Console.WriteLine("Master is deleted");
                _allParts.Remove(SelectedPart);
                FilteredParts.Remove(SelectedPart);
                UpdateCategories();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void SearchParts(string searchTerm)
    {
        FilteredParts.Clear();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            foreach (var part in _allParts)
                FilteredParts.Add(part);
            return;
        }

        var term = searchTerm.ToLower();
        foreach (var part in _allParts.Where(p => 
            p.title?.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
            p.article?.Contains(term, StringComparison.OrdinalIgnoreCase) == true))
        {
            FilteredParts.Add(part);
        }
    }
    private void ApplyFilters()
    {
        var term = SearchText?.ToLower() ?? "";
        var category = SelectedCategory;
        
        var filtered = _allParts.Where(p =>
            (category == "Все" || p.category == category) &&
            (string.IsNullOrEmpty(term) ||
             p.title?.ToLower().Contains(term) == true ||
             p.article?.ToLower().Contains(term) == true))
            .ToList();
        
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            FilteredParts.Clear();
            foreach (var part in filtered)
            {
                FilteredParts.Add(part);
            }
        });
    }

    private void UpdateCategories()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var currentCategories = new HashSet<string>(Categories);
            var neededCategories = new HashSet<string>(_allParts.Select(p => p.category)) { "Все" };

            foreach (var category in neededCategories.Except(currentCategories))
                Categories.Add(category);

            foreach (var category in currentCategories.Except(neededCategories))
                Categories.Remove(category);
        });
    }
}