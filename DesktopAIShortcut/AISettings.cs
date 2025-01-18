using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut;

public class AISettings
{
    public static AISettings Instance { get; } = new AISettings();

    public string AIName { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string Key { get; set; } = "";
    public string SysPrompt { get; set; } = "";
    public string Model { get; set; } = "";
    public List<ContextMessage> ContextMessages { get; set; } = new List<ContextMessage>();

    static AISettings()
    {
    }

    public void SaveSettings()
    {
        System.IO.File.WriteAllText(
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"),
            System.Text.Json.JsonSerializer.Serialize(this, SourceGenerationContext.Default.AISettings));
    }

    public void LoadSettings()
    {
        if (System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json")))
        {
            string json = System.IO.File.ReadAllText(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"));
            var settings = System.Text.Json.JsonSerializer.Deserialize(json, SourceGenerationContext.Default.AISettings);
            if (settings != null)
            {
                AIName = settings.AIName;
                Endpoint = settings.Endpoint;
                Key = settings.Key;
                SysPrompt = settings.SysPrompt;
                Model = settings.Model;
                ContextMessages = settings.ContextMessages;
            }
        }
        else
        {
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"), "{}");
        }
    }
}