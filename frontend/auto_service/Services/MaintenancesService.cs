using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using Auto_Service.Models;

namespace Auto_Service.Services
{
    public class MaintenancesService
    {
        private readonly HttpClient _client;
        public event Action WorksChanged;

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

        public async Task<List<WorkMasterResponce>> GetAllWorks()
        {
            try
            {
                var url = $"http://127.0.0.1:8000/maintenances/maintenances_with_clients";
                Console.WriteLine($"Sending responce: {url}");
                var response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                    return new List<WorkMasterResponce>();
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fetched: {content}");

                var result = await response.Content.ReadFromJsonAsync<List<WorkMasterResponce>>();
                Debug.WriteLine($"Deserialize: {result?.Count ?? 0} works");
                return result ?? new List<WorkMasterResponce>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critizal error in GeAllWorks: {ex}");
                Console.WriteLine(ex);
                return new List<WorkMasterResponce>();
            }

        }

        public async Task<bool> AddMaintenance(AddMaintenanceRequest request)
        {
            try
            {
                var responce = await _client.PostAsJsonAsync(
                    "http://127.0.0.1:8000/maintenances/add_maintenance",
                    new
                    {
                        user_id = request.user_id,
                        client_id = request.client_id,
                        description = request.description,
                        date = request.date,
                        next_maintenance = request.next_maintenance,
                        comment = request.comment,
                        status = request.status,
                        price = request.price
                    });
                if (responce.IsSuccessStatusCode)
                {
                    Console.WriteLine("success");
                    WorksChanged?.Invoke();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critizal error in AddMaintenance: {ex}");
                return false;
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


        public async Task<bool> UpdateMaintenance(int WorkId, string NewStatus, decimal TotalPrice)
        {
            try
            {
                var TotalPriceString = TotalPrice.ToString(CultureInfo.InvariantCulture);
                var responce = await _client.PatchAsJsonAsync($"http://127.0.0.1:8000/maintenances/update_maintenance_status/{WorkId}",
                    new {status = "completed", price = TotalPriceString});
                if (responce.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HTTP Code: {responce.StatusCode}");
                    return true;
                }
                else
                {
                    var errorContent = await responce.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}