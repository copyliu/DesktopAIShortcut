using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DesktopAIShortcut.ViewModels;

public partial class ApplicationViewModel:INotifyPropertyChanged
{
    private readonly MainWindow _mainWindow;
    public const int DefaultWidth = 500;
    public const int DefaultHeight = 300;
    public ChatHistoryListViewModel ChatHistoryListViewModel { get; } = new ChatHistoryListViewModel();

    public ApplicationViewModel()
    {
     
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}