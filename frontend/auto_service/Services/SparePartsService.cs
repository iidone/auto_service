using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class SparePartsService
{
    private readonly HttpClient _spareParts;
    public event Action PartsUpdated;
    
    public SparePartsService(HttpClient httpClient)
    {
        _spareParts = httpClient;
    }
    
    public async Task<List<SparePartsModel>> GetAllSpareParts()
    {
        try
        {
            var url = "http://127.0.0.1:8000/spare-parts/all_spare_parts";
            var response = await _spareParts.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {   
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                return new List<SparePartsModel>();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<List<SparePartsModel>>();
            return result ??  new List<SparePartsModel>();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка в GetWorksByMasterId: {ex}");
            Console.WriteLine(ex);
            return new List<SparePartsModel>();
        }
    }

    public async Task<bool> AddPart(SparePartsModel model)
    {
        try
        {
            if (model == null)
            {
                Console.WriteLine("Model is null");
                return false;
            }

            var url = $"http://127.0.0.1:8000/spare-parts/add_spare_parts";
            var response = await _spareParts.PostAsJsonAsync(url, 
                new
                {
                    title = model.title,
                    category = model.category,
                    article =  model.article,
                    analog = model.analog,
                    supplier = model.supplier,
                    price =  model.price
                });
            if (response.IsSuccessStatusCode)
            {
                PartsUpdated?.Invoke();
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in added part: {e}");
            return false;
        }
    }

    public async Task<bool> DeletePart(IEnumerable<int> partIds)
    {
        try
        {
            var request = new { ids = partIds };
            var responce = await _spareParts.PostAsJsonAsync("http://127.0.0.1:8000/spare-parts/delete-many", request);

            if (responce.IsSuccessStatusCode)
            {
                var result = await responce.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                PartsUpdated?.Invoke();
                return true;
            }
            var error  = await responce.Content.ReadAsStringAsync();
            throw new Exception($"Error: {error}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in delete part: {e}");
            return false;
        }
    }
}