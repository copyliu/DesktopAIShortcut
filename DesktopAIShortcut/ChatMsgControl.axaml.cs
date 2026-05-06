using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.ViewModels;
using Markdig;
using Microsoft.Extensions.AI;

namespace DesktopAIShortcut;

public partial class ChatMsgControl : UserControl
{
    public ChatMsgControl()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = new ChatMsgViewModel()
            {
                UserName = "AI",
                Markdown = "Hello, I am AI",
                Role = ChatRole.Assistant
            };
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        UpdateVisibility();

        if (DataContext is ChatMsgViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ChatMsgViewModel.Role)
            or nameof(ChatMsgViewModel.IsThinking)
            or nameof(ChatMsgViewModel.HasReasoning))
        {
            UpdateVisibility();
        }

        if (e.PropertyName == nameof(ChatMsgViewModel.IsThinking))
        {
            if (DataContext is ChatMsgViewModel vm)
            {
                ReasoningExpander.Header = vm.IsThinking ? "思考中..." : "思考过程";
            }
        }
    }

    private void UpdateVisibility()
    {
        if (DataContext is not ChatMsgViewModel vm) return;

        var isAssistant = vm.Role.Value == "assistant" || vm.Role.Value == "system";
        ActionsPanel.IsVisible = isAssistant && !vm.IsThinking;
        ReasoningExpander.IsVisible = isAssistant && (vm.IsThinking || vm.HasReasoning);
    }

    private async void CopyMarkdownBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatMsgViewModel vm && TopLevel.GetTopLevel(this) is { Clipboard: { } clipboard })
        {
            await clipboard.SetTextAsync(vm.Markdown);
        }
    }

    private async void CopyTextBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatMsgViewModel vm && TopLevel.GetTopLevel(this) is { Clipboard: { } clipboard })
        {
            var plainText = Markdown.ToPlainText(vm.Markdown);
            await clipboard.SetTextAsync(plainText);
        }
    }

    private void RegenerateBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var itemsControl = this.FindAncestorOfType<ItemsControl>();
        if (itemsControl?.DataContext is ChatHistoryViewModel chatHistory)
        {
            _ = chatHistory.RegenerateLastResponseAsync();
        }
    }
}
