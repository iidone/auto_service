using Auto_Service.Models;
using Auto_Service.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auto_Service.Views;

public partial class DetailWindow : Window
{
    public DetailWindow()
    {
        InitializeComponent();
    }
    
    public DetailWindow(WorkMasterResponce selectedWork) : this()
    {
        DataContext = new DetailWindowViewModel(selectedWork);
    }
}
