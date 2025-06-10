using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto_Service.Models;

namespace Auto_Service.Services;

public class ClientService
{
    private readonly HttpClient _client;
    
    public ClientService(HttpClient httpClient)
    {
        _client = httpClient;
    }
    
    public async Task<bool> AddClient(ClientModel client)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(
                "http://127.0.0.1:8000/users/add_client", 
                new {
                    first_name = client.first_name,
                    last_name = client.last_name,
                    contact = client.contact,
                    brand = client.brand,
                    series = client.series,
                    number = client.number,
                    mileage =  client.mileage,
                    age = client.age,
                    vin =  client.vin,
                    last_maintenance = client.last_maintenance
                });
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
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
    
    public async Task<List<ClientModel>> GetAllClients()
    {
        try
        {
            var response = await _client.GetAsync("http://127.0.0.1:8000/clients/all_clients");
            if (!response.IsSuccessStatusCode)
            {
                return new List<ClientModel>();
            }
            return await response.Content.ReadFromJsonAsync<List<ClientModel>>() ?? new List<ClientModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке клиентов: {ex}");
            return new List<ClientModel>();
        }
    }
}