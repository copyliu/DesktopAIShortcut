using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.ViewModels;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace DesktopAIShortcut.Adapter;

public class OpenAIAdapter
{
    private readonly ChatClient client;

    public OpenAIAdapter(ApplicationConfig.Provider provider, string model)
    {
         
        client = new ChatClient(
           model,
            new ApiKeyCredential(provider.Key),
            new OpenAIClientOptions
            {
                Endpoint = new Uri(provider.Endpoint)
            });
    }
    
    
    public async Task<ChatMsgViewModel> SendMessageAsync(List<ChatMsgViewModel> request)
    {
        try
        {
            var r = await client.CompleteChatAsync(request.Select(p =>
                {
                    switch (p.Role.Value)
                    {
                        case "assistant":
                            return (ChatMessage)new AssistantChatMessage(p.Markdown);
                        case "user":
                            return (ChatMessage)new UserChatMessage(p.Markdown);
                        case "tool":
                            return (ChatMessage)new ToolChatMessage(p.Markdown);
                        case "system":
                        default:
                            return (ChatMessage)new SystemChatMessage(p.Markdown);
                    }
                }
            ).ToList());
            return new ChatMsgViewModel()
            {
                UserName = "AI",
                Markdown = string.Join("", r.Value.Content.Select(p => p.Text)),
                Role = ChatRole.Assistant
            };
        }
        catch (Exception ex)
        {
            return new ChatMsgViewModel()
            {
                UserName = "系统",
                Markdown = $"**错误**: {ex.Message}",
                Role = ChatRole.Assistant
            };
        }
    }

    public async Task SendMessageStreamingAsync(
        List<ChatMsgViewModel> request,
        Action<string> onTextChunk,
        Action<string>? onReasoningChunk = null)
    {
        try
        {
            var messages = request.Select(p =>
            {
                return p.Role.Value switch
                {
                    "assistant" => (ChatMessage)new AssistantChatMessage(p.Markdown),
                    "user" => (ChatMessage)new UserChatMessage(p.Markdown),
                    "tool" => (ChatMessage)new ToolChatMessage(p.Markdown),
                    "system" => (ChatMessage)new SystemChatMessage(p.Markdown),
                    _ => (ChatMessage)new SystemChatMessage(p.Markdown)
                };
            }).ToList();

            await foreach (var update in client.CompleteChatStreamingAsync(messages))
            {
                foreach (var part in update.ContentUpdate)
                {
                    if (!string.IsNullOrEmpty(part.Text))
                    {
                        onTextChunk(part.Text);
                    }
                }

                if (onReasoningChunk != null)
                {
                    TryExtractReasoning(update, onReasoningChunk);
                }
            }
        }
        catch (Exception ex)
        {
            onTextChunk($"\n\n**错误**: {ex.Message}");
        }
    }

    private static void TryExtractReasoning(object update, Action<string> onReasoningChunk)
    {
        try
        {
            var type = update.GetType();
            foreach (var propName in new[] { "ReasoningContentUpdate", "ReasoningTokenDelta", "ReasoningDelta" })
            {
                var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) continue;

                var value = prop.GetValue(update);
                if (value is string str && !string.IsNullOrEmpty(str))
                {
                    onReasoningChunk(str);
                    return;
                }
                if (value is System.Collections.IList list && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var textProp = item.GetType().GetProperty("Text", BindingFlags.Public | BindingFlags.Instance);
                        if (textProp != null)
                        {
                            var textVal = textProp.GetValue(item) as string;
                            if (!string.IsNullOrEmpty(textVal))
                                onReasoningChunk(textVal);
                        }
                    }
                }
            }
        }
        catch
        {
            // 不支持思考过程的模型，静默跳过
        }
}

    
    
    
    
}