using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut;
//TODO: 多AI支持
public   class AISettings
{
    public static AISettings Instance { get; } = new AISettings();

    public string AIName { get; set; } = "";
    public    string Endpoint { get; set; }= "";
    public    string Key { get; set; }= "";
    public    string SysPrompt { get; set; }= "";
    public   string Model { get; set; }= "";
    public List<ContextMessage> ContextMessages { get; set; } = new List<ContextMessage>();

    private static string savefilename;

    static AISettings()
    {
        var savefolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(savefolder))
        {
            savefolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        if (string.IsNullOrEmpty(savefolder))
        {
            savefolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        savefolder = System.IO.Path.Combine(savefolder, "DesktopAIShortcut");
        if (!System.IO.Directory.Exists(savefolder))
        {
            System.IO.Directory.CreateDirectory(savefolder);
        }
        savefilename = System.IO.Path.Combine(savefolder, "settings.json");
    }

   

    public void SaveSettings()
    {
        System.IO.File.WriteAllText(savefilename, System.Text.Json.JsonSerializer.Serialize(this, SourceGenerationContext.Default.AISettings));
    }
    public   void LoadSettings()
    { 
        if (System.IO.File.Exists(savefilename))
        {
            string json =  System.IO.File.ReadAllText(savefilename);
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
               System.IO.File.WriteAllText(savefilename, "{}");
        }
    }
}