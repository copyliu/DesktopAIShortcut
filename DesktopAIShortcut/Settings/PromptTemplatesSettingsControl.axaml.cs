using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut.Settings;

public partial class PromptTemplatesSettingsControl : UserControl
{
    public PromptTemplatesSettingsControl()
    {
        InitializeComponent();
        DataContext = new PromptTemplatesViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
