using Auto_Service.Models;
using ReactiveUI;

namespace Auto_Service.ViewModels
{
    public class DetailWindowViewModel : ReactiveObject
    {
        public WorkMasterResponce SelectedWork { get; }

        public DetailWindowViewModel(WorkMasterResponce selectedWork)
        {
            SelectedWork = selectedWork;
        }
    }
}