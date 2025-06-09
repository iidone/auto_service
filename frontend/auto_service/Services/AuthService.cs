using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services
{
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
                var formData = new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password }
                };
                
                var content = new FormUrlEncodedContent(formData);
                
                var response = await _httpClient.PostAsync("http://127.0.0.1:8000/users/login", content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error status: {response.StatusCode}");
                    return new AuthResponce { Error = $"Ошибка авторизации: {responseContent}" };
                }
                
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponce>();
                return authResponse ?? new AuthResponce { Error = "Не удалось десериализовать ответ сервера" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return new AuthResponce { Error = $"Сервер недоступен: {ex.Message}" };
            }
        }
    }
}