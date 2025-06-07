using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class MasterService
{
    private readonly HttpClient _client;

    public MasterService(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<List<MasterModel>> GetAllMasters()
    {
        try
        {
            var url = "http://127.0.0.1:8000/users/all_masters";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                return new List<MasterModel>();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<List<MasterModel>>();
            return result ??  new List<MasterModel>();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка в GetWorksByMasterId: {ex}");
            Console.WriteLine(ex);
            return new List<MasterModel>();
        }
    }
    
}