using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopAIShortcut.ViewModels;

public class ChatHistoryListViewModel: INotifyPropertyChanged
{
    private ChatHistoryViewModel _currentChatHistoryViewModel;
    private ObservableCollection<ChatHistoryViewModel> _chatHistoryViewModels;

    public ChatHistoryViewModel CurrentChatHistoryViewModel
    {
        get => _currentChatHistoryViewModel;
        set => SetField(ref _currentChatHistoryViewModel, value);
    }

    public ObservableCollection<ChatHistoryViewModel> ChatHistoryViewModels
    {
        get => _chatHistoryViewModels;
        set => SetField(ref _chatHistoryViewModels, value);
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