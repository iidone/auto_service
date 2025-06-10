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
}