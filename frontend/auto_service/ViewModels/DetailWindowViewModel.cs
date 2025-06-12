using System.Net.Http;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using Auto_Service.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Auto_Service.ViewModels
{
    public class DetailWindowViewModel : ReactiveObject
    {
        private readonly Window _currentWindow;
        public WorkMasterResponce SelectedWork { get; }
        public ReactiveCommand<Unit, Unit> ShowCloseRequestDialogCommand { get; }

        public DetailWindowViewModel(WorkMasterResponce selectedWork, Window current_window)
        {
            _currentWindow = current_window;
            SelectedWork = selectedWork;

            ShowCloseRequestDialogCommand = ReactiveCommand.CreateFromTask(async work =>
            {
                var maintenance_service = new MaintenancesService(new HttpClient());
                var closeWIndow = new CloseRequestWindow(selectedWork.Maintenance.id);
                var PartsService = new SparePartsService(new HttpClient());
                var dialog = new CloseRequestWindow(selectedWork.Maintenance.id)
                {
                    DataContext = new CloseRequestViewModel(PartsService, selectedWork.Maintenance.id,  closeWIndow, maintenance_service)
                };
                dialog.Show();
            });
        }
    }
}