using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Auto_Service.Models;
using Auto_Service.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Auto_Service.Views;
using Avalonia.Controls;

namespace Auto_Service.ViewModels;

public class AdminWindowViewModel : ReactiveObject
{
    private readonly AdminService _service;
    private ObservableCollection<UsersModel> _users = new();
    
    public ObservableCollection<UsersModel> Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }
    
    public ReactiveCommand<Unit, Unit> LoadUsersCommand { get; }
    
    public AdminWindowViewModel(AdminService service)
    {
        _service = service;

        LoadUsersCommand = ReactiveCommand.CreateFromTask(ViewAllUsers);
        LoadUsersCommand.Execute().Subscribe();
    }
    
    private async Task ViewAllUsers()
    {
        try
        {
            Console.WriteLine("Загрузка пользователей...");
            
            var users = await _service.GetAllUsers();
            
            if (users == null || users.Count == 0)
            {
                Console.WriteLine("Получен пустой список пользователей");
                Users.Clear();
                return;
            }
            
            Users = new ObservableCollection<UsersModel>(users);
            Console.WriteLine($"Успешно загружено {users.Count} пользователей");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке пользователей: {e}");
        }
    }
}