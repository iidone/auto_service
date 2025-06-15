using Auto_Service.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Auto_Service.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Auto_Service.ViewModels
{
    public class AddUserViewModel : ReactiveObject
    {
        private string _username;
        private string _firstName;
        private string _lastName;
        private string _contact;
        private string _password;
        private string _confirmPassword;
        private string _selectedRole;
        private readonly MasterService _service;
        private readonly Window _window;
        public MasterModel NewUser { get; } = new MasterModel() { };

        public List<string> Roles { get; } = new() { "admin", "manager", "master", "storekeeper" };

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => this.RaiseAndSetIfChanged(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => this.RaiseAndSetIfChanged(ref _lastName, value);
        }

        public string Contact
        {
            get => _contact;
            set => this.RaiseAndSetIfChanged(ref _contact, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set => this.RaiseAndSetIfChanged(ref _selectedRole, value);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public AddUserViewModel(Window window, MasterService service)
        {
            _window = window;
            _service = service;
            SaveCommand = ReactiveCommand.CreateFromTask(SaveUser);
            CancelCommand = ReactiveCommand.Create(() => window.Close());
        }

        private async Task SaveUser()
        {
            try
            {
                NewUser.username = GenerateUsername();
                NewUser.password = GeneratePassword();
                NewUser.role = SelectedRole;
                bool isSuccess = await _service.AddMaster(NewUser);
                if (isSuccess)
                {
                    Console.WriteLine($"User {NewUser.username} added");
                    await ShowSuccessDialog();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private string GenerateUsername()
        {
            string Transliterate(string text)
            {
                var translit = new Dictionary<string, string>
                {
                    { "а", "a" }, { "б", "b" }, { "в", "v" }, { "г", "g" }, { "д", "d" },
                    { "е", "e" }, { "ё", "yo" }, { "ж", "zh" }, { "з", "z" }, { "и", "i" },
                    { "й", "y" }, { "к", "k" }, { "л", "l" }, { "м", "m" }, { "н", "n" },
                    { "о", "o" }, { "п", "p" }, { "р", "r" }, { "с", "s" }, { "т", "t" },
                    { "у", "u" }, { "ф", "f" }, { "х", "kh" }, { "ц", "ts" }, { "ч", "ch" },
                    { "ш", "sh" }, { "щ", "shch" }, { "ъ", "" }, { "ы", "y" }, { "ь", "" },
                    { "э", "e" }, { "ю", "yu" }, { "я", "ya" }
                };

                return string.Concat(text.ToLower().Select(c =>
                    translit.TryGetValue(c.ToString(), out var latin) ? latin : c.ToString()));
            }

            string CleanUsername(string username)
            {
                var cleaned = Regex.Replace(username, @"[^a-z0-9.]", "");
                return Regex.Replace(cleaned, @"\.+", ".").Trim('.');
            }

            var firstNameTranslit = Transliterate(NewUser.first_name);
            var lastNameTranslit = Transliterate(NewUser.last_name);

            var baseName = $"{firstNameTranslit}.{lastNameTranslit}";


            return CleanUsername($"{baseName}.{Guid.NewGuid().ToString("N").Substring(0, 4)}");

        }

        private string GeneratePassword()
            => Guid.NewGuid().ToString().Substring(0, 8);
        
        private async Task ShowSuccessDialog()
        {
            var dialog = new Window
            {
                Title = "Успешное создание",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBox { 
                            Text = $"Имя пользователя: {NewUser.username}", 
                            Margin = new Thickness(5),
                            IsReadOnly = true,
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0)
                        },
                        new TextBox { 
                            Text = $"Пароль: {NewUser.password}", 
                            Margin = new Thickness(10),
                            IsReadOnly = true,
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                        },
                        new Button 
                        { 
                        }
                    }
                }
            };
            var okButton = (Button)((StackPanel)dialog.Content).Children[2];
            okButton.Content = "Закрыть";
            okButton.Command = ReactiveCommand.Create(() => dialog.Close());
            okButton.HorizontalAlignment = HorizontalAlignment.Center;
            okButton.Margin = new Thickness(10);
            dialog.CanResize = false;
        

            await dialog.ShowDialog(_window);
        }

    }
}