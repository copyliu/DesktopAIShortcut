using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using LiveMarkdown.Avalonia;
using Microsoft.Extensions.AI;

namespace DesktopAIShortcut.ViewModels;

public class ChatMsgViewModel:INotifyPropertyChanged
{
    private string _userName;
    private string _markdown;
    private ObservableStringBuilder _markdownStringBuilder=new ObservableStringBuilder();
    private ObservableStringBuilder _reasoningBuilder = new ObservableStringBuilder();
    private string _reasoning = "";
    private bool _hasReasoning;
    private bool _isThinking;
    private ChatRole _role;

    public ChatRole Role
    {
        get => _role;
        set => SetField(ref _role, value);
    }

    public string Markdown
    {
        get => _markdown;
        set
        {
            if (value == _markdown) return;
            SetField(ref _markdown, value);
            OnPropertyChanged();
            this._markdownStringBuilder.Clear().AppendLine(value);
            
            // OnPropertyChanged(nameof(Html));
        }
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (value == _userName) return;
            SetField(ref _userName, value);
            OnPropertyChanged();
        }
    }

    public void AppendLine(string line)
    {
        this._markdown += line + "\n";
        this._markdownStringBuilder.AppendLine(line);
        OnPropertyChanged(nameof(Markdown));
    }

    public void AppendText(string text)
    {
        this._markdown += text;
        this._markdownStringBuilder.Append(text);
        OnPropertyChanged(nameof(Markdown));
    }
    
    [JsonIgnore]
    public ObservableStringBuilder MarkdownStringBuilder
    {
        get => _markdownStringBuilder;
    }

    public string Reasoning
    {
        get => _reasoning;
        set
        {
            if (value == _reasoning) return;
            _reasoning = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public ObservableStringBuilder ReasoningBuilder
    {
        get => _reasoningBuilder;
    }

    public bool HasReasoning
    {
        get => _hasReasoning;
        set => SetField(ref _hasReasoning, value);
    }

    public bool IsThinking
    {
        get => _isThinking;
        set => SetField(ref _isThinking, value);
    }

    public void AppendReasoning(string text)
    {
        _reasoning += text;
        _reasoningBuilder.Append(text);
        HasReasoning = true;
        OnPropertyChanged(nameof(Reasoning));
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