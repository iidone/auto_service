using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.Views;
using Avalonia.Controls;
using ReactiveUI;

namespace Auto_Service.ViewModels
{
    public class MasterWindowViewModel : ReactiveObject
    {
        private readonly MaintenancesService _service;
        private readonly int _masterId;
        private ObservableCollection<WorkMasterResponce> _works = new();
        private bool _isLoading;
        private readonly IWindowService _windowService;

        public ObservableCollection<WorkMasterResponce> Works
        {
            get => _works;
            set => this.RaiseAndSetIfChanged(ref _works, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ReactiveCommand<Unit, Unit> LoadWorksCommand { get; }
        public ReactiveCommand<WorkMasterResponce, Unit> ShowDetailsCommand { get; }

        public MasterWindowViewModel(MaintenancesService service, int masterId)
        {
            Debug.WriteLine($"Инициализация ViewModel для мастера {masterId}");
            
            _service = service;
            _masterId = masterId;
            
            LoadWorksCommand = ReactiveCommand.CreateFromTask(LoadWorksAsync);
            ShowDetailsCommand = ReactiveCommand.Create<WorkMasterResponce>(ShowDetails);
            LoadInitialData();
        }
        
        private static void ShowDetails(WorkMasterResponce selectedWork)
        {
            var detailWindow = new DetailWindow(selectedWork);
            Console.WriteLine($"Loading for {selectedWork.Maintenance.id}");
            var detailsWindow = new DetailWindow
            {
                DataContext = new DetailWindowViewModel(selectedWork,  detailWindow)
            };
            detailsWindow.Show();
        }
        private void LoadInitialData()
        {
            Debug.WriteLine("Запуск начальной загрузки данных");
            LoadWorksCommand.Execute().Subscribe(
                onNext: _ => Debug.WriteLine("Данные успешно загружены"),
                onError: ex => Debug.WriteLine($"Ошибка загрузки: {ex}"));
        }

        private async Task LoadWorksAsync()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine("Начало загрузки работ...");

                var works = await _service.GetWorksByMasterId(_masterId);
                
                if (works == null || works.Count == 0)
                {
                    Debug.WriteLine("Получен пустой список работ");
                    Works.Clear();
                    return;
                }

                Works = new ObservableCollection<WorkMasterResponce>(works);
                Console.WriteLine($"Успешно загружено {works.Count} работ");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Критическая ошибка при загрузке: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}