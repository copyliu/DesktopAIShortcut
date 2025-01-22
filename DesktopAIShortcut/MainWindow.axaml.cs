using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.Services;
using OpenAI;
using OpenAI.Chat;
using System.IO;
using System.Text.Json;

namespace DesktopAIShortcut;

public partial class MainWindow : Window
{
    private ChatClient? client;
    private ObservableCollection<ChatMsgModel> _models = new ObservableCollection<ChatMsgModel>();
    private ChatHistoryService _historyService = new ChatHistoryService();
    private ChatHistoryModel _currentChat = new ChatHistoryModel();
    private bool _isAutoSaving = false; // 添加标志位避免循环保存

    public MainWindow()
    {
        InitializeComponent();

        ItemsControl.ItemsSource = _models;

        RefreshHistoryList();

        InitializeChat();

        //HistoryList.ItemsSource = _historyService.LoadAllChats();

        // 监听消息集合变化，自动保存
        _models.CollectionChanged += (sender, args) =>
        {
            if (_isAutoSaving) return; // 如果是自动加载历史记录，不触发保存

            _currentChat.Messages = _models.ToList();
            _historyService.SaveChat(_currentChat);
            RefreshHistoryList();
        };
    }

    public ChatMsgModel DefaultMsg => new ChatMsgModel()
    {
        UserName = AISettings.Instance.AIName,
        Markdown = AISettings.Instance.AIName + "：你好。",
        IsUser = false
    };

    void EnableInput()
    {
        this.ChatInput.IsEnabled = true;
        this.SendBtn.IsEnabled = true;
        this.ChatInput.Text = "";
        this.ChatInput.Focus();
    }

    void DisableInput()
    {
        this.ChatInput.IsEnabled = false;
        this.SendBtn.IsEnabled = false;
    }

    private void RefreshHistoryList()
    {
        HistoryList.ItemsSource = _historyService.LoadAllChats();
    }

    private void InitializeChat()
    {
        try
        {
            AISettings.Instance.LoadSettings();
            // 修改此处的初始化方式
            client = new ChatClient(
                AISettings.Instance.Model,
                new ApiKeyCredential(AISettings.Instance.Key),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(AISettings.Instance.Endpoint)
                });

            defaultMsg.DataContext = DefaultMsg;
        }
        catch (Exception e)
        {
            defaultMsg.DataContext = new ChatMsgModel()
            {
                UserName = "AI",
                Markdown = "初始化错误, 请在托盘图标点击右键, 选择设置中确认你已经设置好API信息. ",
                IsUser = false
            };
            client = null;
        }

        if (client != null)
        {
            EnableInput();
        }
        else
        {
            DisableInput();
        }
    }

    private void HistoryItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ChatHistoryModel chat)
        {
            _currentChat = chat;
            _isAutoSaving = true; // 设置标志位，避免触发自动保存
            _models.Clear();
            foreach (var msg in chat.Messages)
            {
                _models.Add(msg);
            }
            _isAutoSaving = false; // 恢复自动保存

            if (client == null)
            {
                InitializeChat();
            }
        }
    }

    private void NewChat_OnClick(object? sender, RoutedEventArgs e)
    {
        _currentChat = new ChatHistoryModel();
        _isAutoSaving = true;
        _models.Clear();
        _isAutoSaving = false;
        InitializeChat();
    }

    private void DeleteHistory_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ChatHistoryModel chat)
        {
            _historyService.DeleteChat(chat.Id);
            RefreshHistoryList();
        }
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        this.Hide();
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            this.SendBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    private async void SendBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var userMessage = new ChatMsgModel()
        {
            UserName = "用户",
            Markdown = this.ChatInput.Text?.Trim() + "",
            IsUser = true
        };

        DisableInput();
        _models.Add(userMessage);
        ScrollViewer.ScrollToEnd();

        var model = new ChatMsgModel()
        {
            UserName = AISettings.Instance.AIName,
            Markdown = "",
            IsUser = false
        };

        try
        {
            var messages = new List<ChatMessage>();
            // 添加聊天前的预定义消息
            messages.AddRange(AISettings.Instance.ContextMessages
                .Where(m => m.IsBeforeChat)
                .OrderBy(m => m.Order)
                .Select(m => m.ChatMessage));
            // 添加当前聊天消息
            messages.AddRange(_models.Select(p => p.ChatMessage));
            // 添加聊天后的预定义消息
            messages.AddRange(AISettings.Instance.ContextMessages
                .Where(m => !m.IsBeforeChat)
                .OrderBy(m => m.Order)
                .Select(m => m.ChatMessage));

            // 创建选项并设置参数
            var options = new ChatCompletionOptions();

            // 设置参数
            if (AISettings.Instance.EnableAdvancedSettings != false)
            {
                options.MaxOutputTokenCount = AISettings.Instance.MaxTokens;
                options.Temperature = AISettings.Instance.Temperature;
                options.FrequencyPenalty = AISettings.Instance.FrequencyPenalty;
                options.PresencePenalty = AISettings.Instance.PresencePenalty;
                options.TopP = AISettings.Instance.TopP;
            }

            // 将其他参数添加到 Metadata 中
            // 注意：仅备用，应该不会生效，叼库不支持
            //options.Metadata["temperature"] = AISettings.Instance.Temperature.ToString();
            options.Metadata["top_k"] = AISettings.Instance.TopK.ToString();
            //options.Metadata["frequency_penalty"] = AISettings.Instance.FrequencyPenalty.ToString();
            options.Metadata["repetition_penalty"] = AISettings.Instance.RepetitionPenalty.ToString();
            options.Metadata["context_window_size"] = AISettings.Instance.ContextWindowSize.ToString();

            _models.Add(model);

            // 在调用请求前添加日志记录
            if (AISettings.Instance.EnableLogging != false)
            {
                var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DesktopAIShortcut",
                "requests.log"
            );

                // 构造日志对象
                var logEntry = new
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    Model = model, // 假设 ChatClient 有 Model 属性
                    Messages = messages,
                    Options = options
                };

                // 序列化为 JSON 并追加写入文件
                var json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.AppendAllText(logPath, $"{json}\n\n"); // 用空行分隔不同请求
            }
            // 使用带选项的调用方式
            var ret = client.CompleteChatStreamingAsync(messages.ToArray(), options);
            await foreach (var r in ret)
            {
                if (r.ContentUpdate.Any())
                {
                    model.Markdown += r.ContentUpdate[0].Text;
                    ScrollViewer.ScrollToEnd();
                }
            }

            // 保存聊天记录
            _currentChat.Messages = _models.ToList();
            if (string.IsNullOrEmpty(_currentChat.Title) && _models.Count > 0)
            {
                // 使用用户的第一条消息作为标题
                var firstUserMessage = _models.FirstOrDefault(m => m.IsUser)?.Markdown;
                _currentChat.Title = !string.IsNullOrEmpty(firstUserMessage)
                    ? firstUserMessage.Substring(0, Math.Min(30, firstUserMessage.Length))
                    : "新对话";
            }
            _historyService.SaveChat(_currentChat);
            RefreshHistoryList();
        }
        catch (Exception exception)
        {
            model.Markdown += "\n\n出现错误：" + exception.Message;
        }
        finally
        {
            EnableInput();
        }
    }

    public void NewChat()
    {
        _currentChat = new ChatHistoryModel();
        _isAutoSaving = true;
        _models.Clear();
        _isAutoSaving = false;
        InitializeChat();
    }
}