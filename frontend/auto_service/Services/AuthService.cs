using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponce> LoginAsync(string username, string password)
    {
        try
        {
            var responce =
                await _httpClient.PostAsJsonAsync("", new AuthRequest { Login = username, Password = password });
            if (!responce.IsSuccessStatusCode)
            {
                return new AuthResponce { Error = "Ошибка авторизации" };
            }
            return await responce.Content.ReadFromJsonAsync<AuthResponce>() ?? throw new InvalidOperationException();
        }
        catch (Exception)
        {
            return new AuthResponce { Error = "Сервер недоступен" };
        }
    }
    
}