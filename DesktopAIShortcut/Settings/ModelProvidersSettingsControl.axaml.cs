using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut.Settings;

public partial class ModelProvidersSettingsControl : UserControl
{
    public ModelProvidersSettingsControl()
    {
        InitializeComponent();
        DataContext = new ModelProvidersSettingsViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
