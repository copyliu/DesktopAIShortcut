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
using DesktopAIShortcut.Services;
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

    private PromptTemplate? _selectedPromptTemplate;
    public PromptTemplate? SelectedPromptTemplate
    {
        get => _selectedPromptTemplate;
        set => SetField(ref _selectedPromptTemplate, value);
    }

    public bool SingleAnswerMode
    {
        get => _singleAnswerMode;
        set => SetField(ref _singleAnswerMode, value);
    }



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
        UserInput = "";

        var response = new ChatMsgViewModel()
        {
            Role = ChatRole.Assistant,
            Markdown = "",
            UserName = "AI助手"
        };
        this.Messages.Add(response);

        try
        {
            var provider = ApplicationConfig.Instance.Providers.FirstOrDefault(p => p.ProviderId == ProviderId);
            if (provider == null)
            {
                response.Markdown = "**错误**: 未选择大模型供应商，请在设置中配置。";
                return;
            }

            if (string.IsNullOrEmpty(ModelName))
            {
                response.Markdown = "**错误**: 未选择模型，请在上方下拉框中选择。";
                return;
            }

            if (string.IsNullOrEmpty(provider.Key))
            {
                response.Markdown = "**错误**: 未设置API密钥，请在设置中配置。";
                return;
            }

            response.IsThinking = true;

            var adap = new OpenAIAdapter(provider, ModelName);

            var messagesToSend = new List<ChatMsgViewModel>();

            if (SelectedPromptTemplate != null)
            {
                foreach (var msg in SelectedPromptTemplate.PrefixMessages)
                {
                    messagesToSend.Add(new ChatMsgViewModel()
                    {
                        Role = msg.Role,
                        Markdown = msg.Content
                    });
                }
            }

            messagesToSend.AddRange(this._messages.Take(this._messages.Count - 1));

            if (SelectedPromptTemplate != null)
            {
                foreach (var msg in SelectedPromptTemplate.SuffixMessages)
                {
                    messagesToSend.Add(new ChatMsgViewModel()
                    {
                        Role = msg.Role,
                        Markdown = msg.Content
                    });
                }
            }

            var firstChunk = true;
            await adap.SendMessageStreamingAsync(
                messagesToSend,
                onTextChunk: chunk =>
                {
                    if (firstChunk)
                    {
                        firstChunk = false;
                        response.IsThinking = false;
                    }
                    response.AppendText(chunk);
                },
                onReasoningChunk: chunk =>
                {
                    response.AppendReasoning(chunk);
                });

            if (firstChunk)
            {
                response.IsThinking = false;
            }
        }
        catch (Exception ex)
        {
            response.Markdown = $"**调用失败**: {ex.Message}";
        }

        var historyService = new ChatHistoryService();
        historyService.SaveChat(this);
    }

    public async Task RegenerateLastResponseAsync()
    {
        var lastAssistant = _messages.LastOrDefault(m => m.Role.Value == "assistant");
        if (lastAssistant == null) return;

        var lastUser = _messages.LastOrDefault(m => m.Role.Value == "user");
        if (lastUser == null) return;

        _messages.Remove(lastAssistant);

        var response = new ChatMsgViewModel()
        {
            Role = ChatRole.Assistant,
            Markdown = "",
            UserName = "AI助手",
            IsThinking = true
        };
        _messages.Add(response);

        try
        {
            var provider = ApplicationConfig.Instance.Providers.FirstOrDefault(p => p.ProviderId == ProviderId);
            if (provider == null)
            {
                response.Markdown = "**错误**: 未选择大模型供应商，请在设置中配置。";
                return;
            }

            if (string.IsNullOrEmpty(ModelName))
            {
                response.Markdown = "**错误**: 未选择模型，请在上方下拉框中选择。";
                return;
            }

            var adap = new OpenAIAdapter(provider, ModelName);

            var messagesToSend = new List<ChatMsgViewModel>();
            if (SelectedPromptTemplate != null)
            {
                foreach (var msg in SelectedPromptTemplate.PrefixMessages)
                    messagesToSend.Add(new ChatMsgViewModel { Role = msg.Role, Markdown = msg.Content });
            }

            messagesToSend.AddRange(_messages.Take(_messages.Count - 1));

            if (SelectedPromptTemplate != null)
            {
                foreach (var msg in SelectedPromptTemplate.SuffixMessages)
                    messagesToSend.Add(new ChatMsgViewModel { Role = msg.Role, Markdown = msg.Content });
            }

            var firstChunk = true;
            await adap.SendMessageStreamingAsync(
                messagesToSend,
                onTextChunk: chunk =>
                {
                    if (firstChunk)
                    {
                        firstChunk = false;
                        response.IsThinking = false;
                    }
                    response.AppendText(chunk);
                },
                onReasoningChunk: chunk =>
                {
                    response.AppendReasoning(chunk);
                });

            if (firstChunk)
                response.IsThinking = false;
        }
        catch (Exception ex)
        {
            response.Markdown = $"**调用失败**: {ex.Message}";
        }

        var historyService = new ChatHistoryService();
        historyService.SaveChat(this);
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