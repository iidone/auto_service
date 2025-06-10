using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using Auto_Service.Models;

namespace Auto_Service.Services
{
    public class MaintenancesService
    {
        private readonly HttpClient _client;

        public MaintenancesService(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<List<WorkMasterResponce>> GetWorksByMasterId(int masterId)
        {
            Debug.WriteLine($"Начало GetWorksByMasterId для masterId: {masterId}");
            
            try
            {
                var url = $"http://127.0.0.1:8000/maintenances/by_master/{masterId}";
                Console.WriteLine($"Отправка запроса на: {url}");
                
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка в ответе: {errorContent}");
                    return new List<WorkMasterResponce>();
                }

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Полученные данные: {content}");

                var result = await response.Content.ReadFromJsonAsync<List<WorkMasterResponce>>();
                Debug.WriteLine($"Успешно десериализовано {result?.Count ?? 0} записей");

                return result ?? new List<WorkMasterResponce>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Критическая ошибка в GetWorksByMasterId: {ex}");
                Console.WriteLine(ex);
                return new List<WorkMasterResponce>();
            }
        }
        
        public async Task<List<MaintenancesModel>> GetAllMaintenances()
        {
            try
            {
                var url = "http://127.0.0.1:8000/maintenances/all_maintenances";
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {   
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return new List<MaintenancesModel>();
                }
            
                var content = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<List<MaintenancesModel>>();
                return result ??  new List<MaintenancesModel>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex}");
                Console.WriteLine(ex);
                return new List<MaintenancesModel>();
            }
        }
    }
}