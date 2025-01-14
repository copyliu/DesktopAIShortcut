using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DesktopAIShortcut;
//TODO: 多AI支持
public   class AISettings
{
    public static AISettings Instance { get; } = new AISettings();

    public   string AIName { get; set; }
    public    string Endpoint { get; set; }
    public    string Key { get; set; }
    public    string SysPrompt { get; set; }
    public   string Model { get; set; }
    static AISettings()
    {
    }

   

    public void SaveSettings()
    {
        System.IO.File.WriteAllText("settings.json", System.Text.Json.JsonSerializer.Serialize(this));
    }
    public   void LoadSettings()
    { 
        if (System.IO.File.Exists("settings.json"))
        {
            string json =  System.IO.File.ReadAllText("settings.json");
            var settings = System.Text.Json.JsonSerializer.Deserialize<AISettings>(json);
            if (settings != null)
            {
                AIName = settings.AIName;
                Endpoint = settings.Endpoint;
                Key = settings.Key;
                SysPrompt = settings.SysPrompt;
                Model = settings.Model;
                    
            }
        }
        else
        {
               System.IO.File.WriteAllText("settings.json", "{}");
        }
    }
}