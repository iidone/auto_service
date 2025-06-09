using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;

namespace Auto_Service.Services;

public interface IWindowService
{
    void ShowWindow(Window window);
    void CloseWindow(Window window);

}

public class WindowService : IWindowService
{
    private Window _currentWindow;
    
    public void SetCurrentWindow(Window window)
    {
        _currentWindow = window;
    }
    

    public void ShowWindow(Window window)
    {
        window.Show();
    }

    public void CloseWindow(Window window)
    {
        window.Close();
    }
}