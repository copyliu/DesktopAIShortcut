using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.Services;

namespace DesktopAIShortcut.ViewModels;

public partial class ApplicationViewModel:INotifyPropertyChanged
{
    private readonly MainWindow _mainWindow;
    public const int DefaultWidth = 500;
    public const int DefaultHeight = 300;
    public ChatHistoryListViewModel ChatHistoryListViewModel { get; } = new ChatHistoryListViewModel();

    
    public List<ModelSelectionViewModel> ModelList=>ApplicationConfig.Instance. Providers.Select(p=>p.Model.Select(m=>new ModelSelectionViewModel(p,m))).SelectMany(t=>t).ToList();
    public List<PromptTemplate> PromptTemplates => ApplicationConfig.Instance.ChatPresets.Select(p=>new PromptTemplate(p)).ToList();

    public ApplicationViewModel()
    {
        ApplicationConfig.Instance.OnSettingsSaved+= (sender, args) =>
        {
            OnPropertyChanged(nameof(ModelList));
            OnPropertyChanged(nameof(PromptTemplates));
            foreach (var chat in ChatHistoryListViewModel.ChatHistoryViewModels)
            {
                var promptName = chat.SelectedPromptTemplate?.Name ?? chat.PromptName;
                chat.SelectedPromptTemplate = PromptTemplates.FirstOrDefault(p => p.Name == promptName);
                chat.ResolveSelectedModel();
            }
        };

        LoadChatHistory();
    }

    private void LoadChatHistory()
    {
        try
        {
            var historyService = new ChatHistoryService();
            var chats = historyService.LoadAllChats();
            foreach (var chat in chats)
            {
                chat.ResolveSelectedModel();
                if (!string.IsNullOrEmpty(chat.PromptName))
                {
                    chat.SelectedPromptTemplate = PromptTemplates.FirstOrDefault(p => p.Name == chat.PromptName);
                }
                ChatHistoryListViewModel.ChatHistoryViewModels.Add(chat);
            }
            if (ChatHistoryListViewModel.ChatHistoryViewModels.Count > 0)
            {
                ChatHistoryListViewModel.CurrentChatHistoryViewModel = ChatHistoryListViewModel.ChatHistoryViewModels[0];
            }
        }
        catch
        {
            // 加载失败时忽略，后续可由用户手动创建新对话
        }
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