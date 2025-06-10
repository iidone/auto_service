using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class AdminService
{
    private readonly HttpClient _client;

    public AdminService(HttpClient httpClient)
    
    {
        _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<UsersModel>> GetAllUsers()
    {
        try
        {
            var url = "http://127.0.0.1:8000/users/all_users";
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка API: {error}");
                return new List<UsersModel>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<UsersModel>>();
        
            if (result == null)
            {
                Console.WriteLine("API вернуло null вместо списка пользователей");
                return new List<UsersModel>();
            }
        
            Console.WriteLine($"Получено {result.Count} пользователей");
            return result;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex}");
            return new List<UsersModel>();
        }
    }
}