using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Auto_Service.ViewModels;

public class StoreWindowViewModel : ReactiveObject
{
    public SparePartsModel NewPart { get; set; }
    private readonly SparePartsService _service;
    private readonly ObservableCollection<SparePartsModel> _allParts = new();
    public ObservableCollection<SparePartsModel> FilteredParts { get; }
    
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public ReactiveCommand<Unit, Unit> LoadPartsCommand { get; }
    public ReactiveCommand<Unit, Unit> DeletePartCommand { get; }
    public ReactiveCommand<Unit, Unit> AddPartCommand { get; }

    public StoreWindowViewModel(SparePartsService service)
    {
        _service = service;
        FilteredParts = new ObservableCollection<SparePartsModel>();
        
        LoadPartsCommand = ReactiveCommand.CreateFromTask(LoadParts);
        
        LoadPartsCommand.Execute().Subscribe();
        
        this.WhenAnyValue(x => x.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(SearchParts);
    }
    private SparePartsModel _selectedPart;
    public SparePartsModel SelectedPart
    {
        get => _selectedPart;
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    private async Task LoadParts()
    {
        try
        {
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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
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
}