using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class UserService
{
    private readonly HttpClient _client;
    
    public UserService(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<List<UsersModel>> GetAllUsers()
    {
        try
        {
            var response = await _client.GetAsync("http://127.0.0.1:8000/users/all_users");
            if (!response.IsSuccessStatusCode)
            {
                return new List<UsersModel>();
            }
            return await response.Content.ReadFromJsonAsync<List<UsersModel>>() ?? new List<UsersModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке пользователей: {ex}");
            return new List<UsersModel>();
        }
    }
}