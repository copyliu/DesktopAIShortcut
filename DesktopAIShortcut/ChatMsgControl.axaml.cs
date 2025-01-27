using System;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut;

public partial class ChatMsgControl : UserControl
{
    public ChatMsgControl()
    {
        InitializeComponent();
        if (Application.Current?.TryGetResource("SystemBaseHighColor", Application.Current.ActualThemeVariant,
                out var foregroundBrush)==true)
        {
            if (foregroundBrush is Color color)
            {
                var htmlcolor=$"#{color.R:X2}{color.G:X2}{color.B:X2}";
                _htmllabel.BaseStylesheet="#mainmd{color: "+htmlcolor+"}";
               
            }
        }; 

        if (Application.Current?.TryGetResource("SystemAltHighColor", Application.Current.ActualThemeVariant,
                out var backgroundBrush)==true)
        {
            if (backgroundBrush is Color color)
            {
                 this.Border.Background=new SolidColorBrush(new Color(0x80,color.R,color.G,color.B));
            }
        };
        if (Application.Current!.ActualThemeVariant.Key == "Dark")
        {
            var ass=AssetLoader.Open(new Uri("avares://DesktopAIShortcut/Assets/github-markdown-dark.css"));
            var reader = new StreamReader(ass,Encoding.UTF8);
            _htmllabel.BaseStylesheet += reader.ReadToEnd();
        }
        else
        {
            var ass = AssetLoader.Open(new Uri("avares://DesktopAIShortcut/Assets/github-markdown-light.css"));
            var reader = new StreamReader(ass, Encoding.UTF8);
            _htmllabel.BaseStylesheet += reader.ReadToEnd();
        }
        if (Design.IsDesignMode)
        {
            this.DataContext = new ChatMsgModel()
            {
                UserName = "AI",
                Markdown = "Hello, I am AI",
                IsUser = false
            };
        }
       
    }
}