using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.Services;

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
        model.ResolveSelectedModel();
        if (!string.IsNullOrEmpty(model.PromptName))
        {
            model.SelectedPromptTemplate = ApplicationConfig.Instance.ChatPresets
                .Select(p => new PromptTemplate(p))
                .FirstOrDefault(p => p.Name == model.PromptName);
        }
    }
    [RelayCommand]
    private void RemoveChat(ChatHistoryViewModel model)
    {
        if (_chatHistoryViewModels.Contains(model))
        {
            _chatHistoryViewModels.Remove(model);
            try
            {
                var historyService = new ChatHistoryService();
                historyService.DeleteChat(model.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除聊天记录失败: {ex.Message}");
            }
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
            Id = Guid.NewGuid().ToString(),
            Title = $"聊天{_chatHistoryViewModels.Count + 1}"
        };

        var firstProvider = ApplicationConfig.Instance.Providers.FirstOrDefault();
        if (firstProvider != null)
        {
            var defaultModel = firstProvider.DefaultModel ?? firstProvider.Model.FirstOrDefault() ?? "";
            newmodel.ProviderId = firstProvider.ProviderId;
            newmodel.ModelName = defaultModel;
            newmodel.SelectedModelItem = new ModelSelectionViewModel(firstProvider, defaultModel);
        }

        var defaultTemplate = ApplicationConfig.Instance.ChatPresets.FirstOrDefault(p => p.IsDefault)
            ?? ApplicationConfig.Instance.ChatPresets.FirstOrDefault();
        if (defaultTemplate != null)
        {
            newmodel.SelectedPromptTemplate = new PromptTemplate(defaultTemplate);
        }

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