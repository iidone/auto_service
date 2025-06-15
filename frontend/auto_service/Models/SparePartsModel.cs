using System.Globalization;
using ReactiveUI;

namespace Auto_Service.Models;

public class SparePartsModel : ReactiveObject
{
    private string _priceString;
    public int Id { get; set; }
    public string title { get; set; }
    public string category { get; set; }
    public string article { get; set; }
    public string analog { get; set; }
    public string supplier { get; set; }
    public string price
    {
        get => _priceString;
        set
        {
            this.RaiseAndSetIfChanged(ref _priceString, value);
            this.RaisePropertyChanged(nameof(PriceDecimal));
        }
    }
    public decimal PriceDecimal => 
        decimal.TryParse(price, NumberStyles.Currency, CultureInfo.CurrentCulture, out var result) 
            ? result 
            : 0m;
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
}