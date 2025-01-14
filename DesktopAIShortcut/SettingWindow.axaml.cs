using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace DesktopAIShortcut;

public partial class SettingWindow : Window
{
   
    public SettingWindow()
    {
        InitializeComponent();
        AISettings.Instance.LoadSettings();
        this.DataContext = new AISettings()
        {
            Endpoint = AISettings.Instance.Endpoint,
            Key = AISettings.Instance.Key,
            Model = AISettings.Instance.Model,
            AIName = AISettings.Instance.AIName,
            SysPrompt = AISettings.Instance.SysPrompt,

        };
        this.SizeToContent = SizeToContent.WidthAndHeight;
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
        }
        AISettings.Instance.SaveSettings();
      
    }
}