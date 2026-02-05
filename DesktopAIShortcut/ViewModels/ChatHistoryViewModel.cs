using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DesktopAIShortcut.Adapter;
using DesktopAIShortcut.Models;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace DesktopAIShortcut.ViewModels;

public partial class ChatHistoryViewModel : INotifyPropertyChanged
{
    
    
    
    private string _title = "";
    private DateTime _createTime = DateTime.Now;
    private string _id = Guid.NewGuid().ToString();
    private string _title1 = "";
    private DateTime _createTime1 = DateTime.Now;
    private DateTime _lastUpdateTime = DateTime.Now;
    private ObservableCollection<ChatMsgViewModel> _messages = new();
    private string _providerId;
    private string _modelName;
    private string _promptName;
    private bool _singleAnswerMode;
    private string _userInput;

    public string UserInput
    {
        get => _userInput;
        set => SetField(ref _userInput, value);
    }

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Title
    {
        get => _title1;
        set => SetField(ref _title1, value);
    }

    public DateTime CreateTime
    {
        get => _createTime1;
        set => SetField(ref _createTime1, value);
    }

    public DateTime LastUpdateTime
    {
        get => _lastUpdateTime;
        set => SetField(ref _lastUpdateTime, value);
    }

    public ObservableCollection<ChatMsgViewModel> Messages
    {
        get => _messages;
        set => SetField(ref _messages, value);
    }

    public string ProviderId
    {
        get => _providerId;
        set => SetField(ref _providerId, value);
    }

    public string ModelName
    {
        get => _modelName;
        set => SetField(ref _modelName, value);
    }

    public string PromptName
    {
        get => _promptName;
        set => SetField(ref _promptName, value);
    }

    public bool SingleAnswerMode
    {
        get => _singleAnswerMode;
        set => SetField(ref _singleAnswerMode, value);
    }



    //TODO: client????
    private ChatClient? client;
    
[RelayCommand]
    public async Task AddUserMessageAsync()
    {
        _messages.Add(new ChatMsgViewModel()
        {
            Role = ChatRole.User,
            Markdown = UserInput,
            UserName = "用户"
        });

        var response = new ChatMsgViewModel()
        {
            Role = ChatRole.Assistant,
            Markdown = "",
            UserName = "AI助手"
        };
        this.Messages.Add(response);
        var provider=ApplicationConfig.Instance.Providers.First(p=>p.ProviderId==ProviderId);
        var adap = new OpenAIAdapter(provider, ModelName);
        var resp = await adap.SendMessageAsync(this._messages.ToList());
        response.Markdown = resp.Markdown;
        
        
        return;
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