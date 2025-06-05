using Avalonia.Controls;

namespace Auto_Service.Services;

public interface IWindowService
{
    void ShowWindow(Window window);
    void CloseWindow(Window window);
}

public class WindowService : IWindowService
{
    public void ShowWindow(Window window) => window.Show();
    public void CloseWindow(Window window) => window.Close();
}