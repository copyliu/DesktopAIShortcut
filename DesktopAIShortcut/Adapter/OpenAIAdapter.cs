using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
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
           var r=  await client.CompleteChatAsync(request.Select(p =>
               {
                   switch (p.Role.Value)
                   {
                      
                       case "assistant":
                           return (ChatMessage)new AssistantChatMessage(p.Markdown);
                           break;
                       case "user":
                           return (ChatMessage)new UserChatMessage(p.Markdown);
                           break;
                       case "tool":
                           return (ChatMessage)new ToolChatMessage(p.Markdown);
                           break;
                       case "system":
                       default:
                           return (ChatMessage)new SystemChatMessage(p.Markdown);
                           break;
                   }
               }
               
           ).ToList());
           return new ChatMsgViewModel()
           {
               UserName = "AI",
               Markdown = string.Join( "" ,r.Value.Content.Select(p=>p.Text)),
               Role = ChatRole.Assistant
           };
    }
    
    
    
    
    
}