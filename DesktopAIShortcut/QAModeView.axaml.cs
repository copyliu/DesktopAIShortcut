using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DesktopAIShortcut.ViewModels;
using Markdig;

namespace DesktopAIShortcut;

public partial class QAModeView : UserControl
{
    public QAModeView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ChatHistoryViewModel oldVm)
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        
        if (DataContext is ChatHistoryViewModel newVm)
        {
            newVm.PropertyChanged += OnViewModelPropertyChanged;
            UpdateMarkdownBuilders(newVm);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ChatHistoryViewModel.LastAssistant) && DataContext is ChatHistoryViewModel vm)
        {
            UpdateMarkdownBuilders(vm);
        }
    }

    private void UpdateMarkdownBuilders(ChatHistoryViewModel vm)
    {
        var assistant = vm.LastAssistant;
        if (assistant != null)
        {
            ReasoningMarkdown.MarkdownBuilder = assistant.ReasoningBuilder;
            OutputMarkdown.MarkdownBuilder = assistant.MarkdownStringBuilder;
        }
    }

    private async void CopyMarkdown_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatHistoryViewModel vm && vm.LastAssistant != null
            && TopLevel.GetTopLevel(this) is { Clipboard: { } clipboard })
        {
            await clipboard.SetTextAsync(vm.LastAssistant.Markdown);
        }
    }

    private async void CopyText_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatHistoryViewModel vm && vm.LastAssistant != null
            && TopLevel.GetTopLevel(this) is { Clipboard: { } clipboard })
        {
            var plainText = Markdown.ToPlainText(vm.LastAssistant.Markdown);
            await clipboard.SetTextAsync(plainText);
        }
    }

    private void Regenerate_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatHistoryViewModel vm)
        {
            _ = vm.RegenerateLastResponseAsync();
        }
    }
}
