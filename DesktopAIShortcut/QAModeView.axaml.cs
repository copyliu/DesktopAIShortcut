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
