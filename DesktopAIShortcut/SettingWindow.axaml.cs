using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using DesktopAIShortcut.Models;
using System.Linq;

namespace DesktopAIShortcut;

public partial class SettingWindow : Window
{
    private ObservableCollection<ContextMessage> _contextMessages;

    private void UpdateContextList()
    {
        var sortedMessages = _contextMessages
            .OrderByDescending(m => m.IsBeforeChat)
            .ThenBy(m => m.Order)
            .ToList();

        _contextMessages.Clear();
        foreach (var message in sortedMessages)
        {
            _contextMessages.Add(message);
        }
    }

    public SettingWindow()
    {
        InitializeComponent();
        AISettings.Instance.LoadSettings();
        _contextMessages = new ObservableCollection<ContextMessage>(AISettings.Instance.ContextMessages);
        UpdateContextList();  // 初始化时排序
        this.FindControl<ItemsControl>("ContextList").ItemsSource = _contextMessages;

        // 使用现有设置初始化数据上下文
        this.DataContext = new AISettings()
        {
            Endpoint = AISettings.Instance.Endpoint,
            Key = AISettings.Instance.Key,
            Model = AISettings.Instance.Model,
            AIName = AISettings.Instance.AIName,
            SysPrompt = AISettings.Instance.SysPrompt,
            // 初始化AI参数
            Temperature = AISettings.Instance.Temperature,
            MaxTokens = AISettings.Instance.MaxTokens,
            TopP = AISettings.Instance.TopP,
            TopK = AISettings.Instance.TopK,
            RepetitionPenalty = AISettings.Instance.RepetitionPenalty,
            FrequencyPenalty = AISettings.Instance.FrequencyPenalty,
            PresencePenalty = AISettings.Instance.PresencePenalty,
            ContextWindowSize = AISettings.Instance.ContextWindowSize,
            // 初始化新增的设置项
            EnableAdvancedSettings = AISettings.Instance.EnableAdvancedSettings,
            EnableLogging = AISettings.Instance.EnableLogging
        };

        this.SizeToContent = SizeToContent.WidthAndHeight;
    }

    private void AddContext_Click(object? sender, RoutedEventArgs e)
    {
        var roleComboBox = this.FindControl<ComboBox>("RoleComboBox");
        var positionComboBox = this.FindControl<ComboBox>("PositionComboBox");
        var contextInput = this.FindControl<TextBox>("ContextInput");

        if (string.IsNullOrWhiteSpace(contextInput.Text))
            return;

        var message = new ContextMessage
        {
            Role = ((ComboBoxItem)roleComboBox.SelectedItem).Content.ToString(),
            IsBeforeChat = positionComboBox.SelectedIndex == 0,
            Content = contextInput.Text,
            Order = _contextMessages.Count
        };
        _contextMessages.Add(message);
        UpdateContextList();  // 添加后重新排序
        contextInput.Text = "";
    }

    private void DeleteContext_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is ContextMessage message)
        {
            _contextMessages.Remove(message);
            // 更新剩余项的 Order
            int newOrder = 0;
            foreach (var msg in _contextMessages.OrderByDescending(m => m.IsBeforeChat))
            {
                msg.Order = newOrder++;
            }
            UpdateContextList();  // 删除后重新排序
        }
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AISettings temp)
        {
            AISettings.Instance.Endpoint = temp.Endpoint;
            AISettings.Instance.Key = temp.Key;
            AISettings.Instance.Model = temp.Model;
            AISettings.Instance.AIName = temp.AIName;
            AISettings.Instance.SysPrompt = temp.SysPrompt;
            AISettings.Instance.ContextMessages = _contextMessages.ToList();

            // 保存AI参数
            AISettings.Instance.Temperature = temp.Temperature;
            AISettings.Instance.MaxTokens = temp.MaxTokens;
            AISettings.Instance.TopP = temp.TopP;
            AISettings.Instance.TopK = temp.TopK;
            AISettings.Instance.RepetitionPenalty = temp.RepetitionPenalty;
            AISettings.Instance.FrequencyPenalty = temp.FrequencyPenalty;
            AISettings.Instance.PresencePenalty = temp.PresencePenalty;
            AISettings.Instance.ContextWindowSize = temp.ContextWindowSize;

            // 保存新增的设置项
            AISettings.Instance.EnableAdvancedSettings = temp.EnableAdvancedSettings;
            AISettings.Instance.EnableLogging = temp.EnableLogging;
        }
        AISettings.Instance.SaveSettings();
    }
}