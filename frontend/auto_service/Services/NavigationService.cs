using System;
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
    private Window _ownerWindow;
    
    public WindowService(Window ownerWindow)
    {
        _ownerWindow = ownerWindow ?? throw new ArgumentNullException(nameof(ownerWindow));
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