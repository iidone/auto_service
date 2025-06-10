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
    public event Action ClientsChanged;
    
    public ClientService(HttpClient httpClient)
    {
        _client = httpClient;
    }
    
    public async Task<bool> AddClient(ClientModel client)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(
                "http://127.0.0.1:8000/clients/add_client", 
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
                ClientsChanged?.Invoke();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении клиента: {ex}");
            return false;
        }
    }

    public async Task<List<ClientModel>> GetAllClients()
    {
        try
        {
            var url = $"http://127.0.0.1:8000/clients/all_clients";
            Console.WriteLine($"Sending responce: {url}");
            var responce = await _client.GetAsync(url);

            if (!responce.IsSuccessStatusCode)
            {
                var error = await responce.Content.ReadAsStringAsync();
                Console.WriteLine(error);
                return new List<ClientModel>();
            }
            var content = await responce.Content.ReadAsStringAsync();
            Console.WriteLine($"Fetched: {content}");
            var result = await responce.Content.ReadFromJsonAsync<List<ClientModel>>();
            Console.WriteLine($"Deserialize: {result?.Count ?? 0} clients");
            return result ?? new  List<ClientModel>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Fatal Error in GetAllClients: {e}");
            return new List<ClientModel>();
        }
        
    }
}