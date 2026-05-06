using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.ViewModels;
using OpenAI;
using OpenAI.Responses;

#pragma warning disable OPENAI001

namespace DesktopAIShortcut.Adapter;

public class ResponsesAdapter
{
    private readonly ResponsesClient client;
    private readonly string model;

    public ResponsesAdapter(ApplicationConfig.Provider provider, string model)
    {
        this.model = model;
        client = new ResponsesClient(
            new ApiKeyCredential(provider.Key),
            new OpenAIClientOptions
            {
                Endpoint = new Uri(provider.Endpoint)
            });
    }

    public async Task SendResponseStreamingAsync(
        List<ChatMsgViewModel> request,
        Action<string> onTextChunk,
        Action<string>? onReasoningChunk = null)
    {
        try
        {
            var options = new CreateResponseOptions
            {
                Model = model,
                StreamingEnabled = true,
                ReasoningOptions = new ResponseReasoningOptions
                {
                    ReasoningEffortLevel = ResponseReasoningEffortLevel.Medium
                }
            };

            foreach (var msg in request)
            {
                var item = msg.Role.Value switch
                {
                    "user" => ResponseItem.CreateUserMessageItem(msg.Markdown),
                    "system" => ResponseItem.CreateSystemMessageItem(msg.Markdown),
                    "assistant" => ResponseItem.CreateAssistantMessageItem(msg.Markdown),
                    _ => ResponseItem.CreateUserMessageItem(msg.Markdown)
                };
                options.InputItems.Add(item);
            }

            await foreach (var update in client.CreateResponseStreamingAsync(options))
            {
                switch (update)
                {
                    case StreamingResponseReasoningTextDeltaUpdate reasoningDelta:
                        onReasoningChunk?.Invoke(reasoningDelta.Delta);
                        break;

                    case StreamingResponseOutputTextDeltaUpdate textDelta:
                        onTextChunk(textDelta.Delta);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            onTextChunk($"\n\n**错误**: {ex.Message}");
        }
    }
}
