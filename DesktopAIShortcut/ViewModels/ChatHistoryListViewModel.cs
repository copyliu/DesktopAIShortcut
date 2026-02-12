using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace DesktopAIShortcut.ViewModels;

public partial class ChatHistoryListViewModel: INotifyPropertyChanged
{
    private ChatHistoryViewModel _currentChatHistoryViewModel;
    private ObservableCollection<ChatHistoryViewModel> _chatHistoryViewModels=new ObservableCollection<ChatHistoryViewModel>();

    [RelayCommand]
    private void SelectChat(ChatHistoryViewModel model)
    {
        if (!_chatHistoryViewModels.Contains(model))
        {
            _chatHistoryViewModels.Add(model);
        }
        this.CurrentChatHistoryViewModel = model;
    }
    [RelayCommand]
    private void RemoveChat(ChatHistoryViewModel model)
    {
        if (_chatHistoryViewModels.Contains(model))
        {
            _chatHistoryViewModels.Remove(model);
        }
        if (this.CurrentChatHistoryViewModel == model)
        {
            this.CurrentChatHistoryViewModel = _chatHistoryViewModels.Count > 0 ? _chatHistoryViewModels[0] : null;
        }
    }
    
    [RelayCommand]
    private void AddChat()
    {
        var newmodel = new ChatHistoryViewModel()
        {
            Id = Guid.NewGuid().ToString()
        };
        this._chatHistoryViewModels.Add(newmodel);
        this.CurrentChatHistoryViewModel = newmodel;

    }
    
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