using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Markdig;
using OpenAI.Chat;

namespace DesktopAIShortcut.Models;

public class ChatMsgModel:INotifyPropertyChanged
{
    
    
    static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    private string _userName;
    private string _markdown;
    public bool IsUser { get; set; }
    public ChatMessage ChatMessage {
        get
        {
            if (IsUser)
            {
                return new UserChatMessage(_markdown);
            }
            else
            {
                return new AssistantChatMessage(_markdown);
            }
        }
    }
    public string UserName
    {
        get => _userName;
        set
        {
            if (value == _userName) return;
            _userName = value;
            OnPropertyChanged();
        }
    }

    public string Html
    {
        get
        {
            if (string.IsNullOrEmpty(_markdown))
            {
                return "";
                
            }
            return "<div id=\"mainmd\" class=\"markdown-body\">" + Markdig.Markdown.ToHtml(Markdown,pipeline)+ "</div>";
        }
    }
    
    public string Markdown
    {
        get => _markdown;
        set
        {
            if (value == _markdown) return;
            _markdown = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Html));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}