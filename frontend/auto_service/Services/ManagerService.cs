namespace Auto_Service.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

public class ManagerService
{
    private readonly HttpClient _client;
    public ManagerService(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<List<ManagersModel>> GetAllManagers()
    {
        try
        {
            var url = "http://127.0.0.1:8000/users/all_managers";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {   
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                return new List<ManagersModel>();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<List<ManagersModel>>();
            return result ??  new List<ManagersModel>();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex}");
            Console.WriteLine(ex);
            return new List<ManagersModel>();
        }
    }
}