using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using LiveMarkdown.Avalonia;
using Markdig;
using OpenAI.Chat;

namespace DesktopAIShortcut.Models;

public class ChatMsgModel:INotifyPropertyChanged
{
    
    
    static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    private string _userName;
    private string _markdown;
    public bool IsUser { get; set; }
    public OpenAI.Chat.ChatMessage ChatMessage {
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

    // public string Html
    // {
    //     get
    //     {
    //         if (string.IsNullOrEmpty(_markdown))
    //         {
    //             return "";
    //             
    //         }
    //         return "<div id=\"mainmd\" class=\"markdown-body\">" + Markdig.Markdown.ToHtml(Markdown,pipeline)+ "</div>";
    //     }
    // }

    private ObservableStringBuilder _markdownStringBuilder=new ObservableStringBuilder();

    [JsonIgnore]
    public ObservableStringBuilder MarkdownStringBuilder
    {
        get => _markdownStringBuilder;
    }

    public string Markdown
    {
        get => _markdown;
        set
        {
            if (value == _markdown) return;
            _markdown = value;
            OnPropertyChanged();
            this._markdownStringBuilder.Clear().AppendLine(value);
            
            // OnPropertyChanged(nameof(Html));
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