using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DesktopAIShortcut.Settings;

namespace DesktopAIShortcut;

public partial class SettingWindow : Window
{
    private readonly UserControl _modelProvidersControl;
    private readonly UserControl _promptTemplatesControl;
    private readonly UserControl _mcpServicesControl;
    private readonly UserControl _webSearchControl;
    private readonly UserControl _generalSettingsControl;

    public SettingWindow()
    {
        InitializeComponent();
           
        // 初始化所有子页面控件
        _modelProvidersControl = new ModelProvidersSettingsControl();
        _promptTemplatesControl = new PromptTemplatesSettingsControl();
        _mcpServicesControl = new McpServicesSettingsControl();
        _webSearchControl = new WebSearchSettingsControl();
        _generalSettingsControl = new GeneralSettingsControl();
        
        // 默认选中第一项
        var settingsList = this.FindControl<ListBox>("SettingsList");
        settingsList.SelectedIndex = 0;
        
        // 手动触发选择变更以显示第一个页面
        SettingsList_OnSelectionChanged(null, null);
    }
    private void SettingsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs? e)
    {
        var contentArea = this.FindControl<ContentControl>("ContentArea");
        var settingsList = this.FindControl<ListBox>("SettingsList");
        
        if (settingsList.SelectedItem is ListBoxItem selectedItem)
        {
            var tag = selectedItem.Tag as string;
            contentArea.Content = tag switch
            {
                "ModelProviders" => _modelProvidersControl,
                "PromptTemplates" => _promptTemplatesControl,
                "McpServices" => _mcpServicesControl,
                "WebSearch" => _webSearchControl,
                "GeneralSettings" => _generalSettingsControl,
                _ => null
            };
        }
    }
}