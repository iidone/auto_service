using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class MasterService
{
    private readonly HttpClient _client;
    public event Action MasterChanged;
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
    
    public async Task<bool> AddMaster(MasterModel master)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(
                "http://127.0.0.1:8000/users/add_user", 
                new {
                    role = master.role,
                    username = master.username,
                    first_name = master.first_name,
                    last_name = master.last_name,
                    password = master.password,
                    contact = master.contact
                });
            if (response.IsSuccessStatusCode)
            {
                MasterChanged?.Invoke();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении мастера: {ex}");
            return false;
        }
    }

    public async Task<bool> DeleteMaster(IEnumerable<int> masterIds)
    {
        try
        {
            var request = new { ids = masterIds };
            var response = await _client.PostAsJsonAsync("http://127.0.0.1:8000/users/delete-many", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                MasterChanged?.Invoke();
                return true;
            }
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {error}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteMasters error: {ex.Message}");
            return false;
        }
    }
    
}