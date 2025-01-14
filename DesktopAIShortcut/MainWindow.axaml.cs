using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DesktopAIShortcut.Models;
using OpenAI;
using OpenAI.Chat;

namespace DesktopAIShortcut;

public partial class MainWindow : Window
{
    private  ChatClient? client;
    private ObservableCollection<ChatMsgModel> _models = new ObservableCollection<ChatMsgModel>();
    public MainWindow()
    {
        InitializeComponent();
       
        ItemsControl.ItemsSource = _models;
       
       
       
        if (Design.IsDesignMode || true)
        {
            for (int i = 0; i < 10; i++)
            {
                _models.Add(new ChatMsgModel()
                {
                    UserName = AISettings.Instance.AIName,
                    Markdown = "你好，我是" + AISettings.Instance.AIName,
                    IsUser = false
                });
            }
        }

       
    }

    public ChatMsgModel DefaultMsg=>new ChatMsgModel()
    {
        UserName = AISettings.Instance.AIName,
        Markdown = "你好，我是" + AISettings.Instance.AIName,
        IsUser = false
    };

    void EnableInput()
    {
        this.ChatInput.IsEnabled = true;
        this.SendBtn.IsEnabled = true;
        this.ChatInput.Text = "";
        this.ChatInput.Focus();
        
    }
    void DisableInput()
    {
        this.ChatInput.IsEnabled = false;
        this.SendBtn.IsEnabled = false;
    }
    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        this.Hide();
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            this.SendBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    private async void SendBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var model= new ChatMsgModel()
        {
            UserName = AISettings.Instance.AIName,
            Markdown = "",
            IsUser =false
                
        };
        DisableInput();
        ;
        _models.Add(new ChatMsgModel()
        {
            UserName = "用户",
            Markdown = this.ChatInput.Text?.Trim()+"",
            IsUser = true
        });
        ScrollViewer.ScrollToEnd();
        try
        {
            var ret = client.CompleteChatStreamingAsync( [new SystemChatMessage(AISettings.Instance.SysPrompt + ""), .._models.Select(p => p.ChatMessage).ToArray()]);
           
            _models.Add(model);
            await foreach (var r in ret)
            {
                if (r.ContentUpdate.Any())
                {
                    model.Markdown += r.ContentUpdate[0].Text;
                }
                ScrollViewer.ScrollToEnd();
            }
            
        }
        catch (Exception exception)
        {
            model.Markdown +="\n\n出现错误："+ exception.Message;
            // this.MarkdownScrollViewer.Markdown = exception.ToString();
        }
        finally
        {
            EnableInput();
        }
      
    }

    public void NewChat()
    {
        
            try
            {
                AISettings.Instance.LoadSettings();
                client = new OpenAIClient(new(AISettings.Instance.Key), new()
                {
                    Endpoint = new(AISettings.Instance.Endpoint),
                }).GetChatClient(AISettings.Instance.Model);
                defaultMsg.DataContext = DefaultMsg;
            }
            catch (Exception e)
            {
                defaultMsg.DataContext = new ChatMsgModel()
                {
                    UserName = "AI",
                    Markdown = "初始化错误, 请在托盘图标点击右键, 选择设置中确认你已经设置好API信息. ",
                    IsUser = false
                };
                client = null;
            }
        
        this._models.Clear();
        if (client != null)
        {
            EnableInput();
        }
        else
        {
            DisableInput();
        }
    }
}