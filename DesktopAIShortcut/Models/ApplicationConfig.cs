using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut.Models;




public class ApplicationConfig
{
    // 配置保存成功事件
    public event EventHandler? OnSettingsSaved;

    public static ApplicationConfig Instance => _instance;

    public enum AIEndpointType
    {
        [Description("OpenAI兼容")] OpenAI,
        [Description("Google Gemini兼容")] Gemini,
        [Description("Ollama 本地模型兼容")] OLLAMA,
    }

    public class Provider
    {
        public static Dictionary<string, Provider> ProviderTemplates = new Dictionary<string, Provider>()
        {
            {
                "OpenAI", new Provider()
                {
                    ProviderId = "OpenAI",
                    Type = AIEndpointType.OpenAI,
                    Name = "OpenAI",
                    Endpoint = "https://api.openai.com/v1",
                    Key = "",
                    Model = new string[] { "gpt-3.5-turbo", "gpt-4" },
                    DefaultModel = "gpt-3.5-turbo"

                }
            },
            {
                "Gemini", new Provider()
                {
                    ProviderId = "Gemini",
                    Type = AIEndpointType.Gemini,
                    Name = "Google Gemini",
                    Endpoint = "https://generativelanguage.googleapis.com/",
                    Key = "",
                    Model = new string[] { "gemini-pro", "gemini-pro-vision", "gemini-1.5-pro", "gemini-1.5-flash" },
                    DefaultModel = "gemini-pro"
                }
            },
            {
                "OpenRouter", new Provider()
                {
                    ProviderId = "OpenRouter",
                    Type = AIEndpointType.OpenAI,
                    Name = "OpenRouter",
                    Endpoint = "https://openrouter.ai/api/v1",
                    Key = "",
                    Model = new string[] { "gemini-pro", "gemini-pro-vision", "gemini-1.5-pro", "gemini-1.5-flash" },
                    DefaultModel = "gemini-pro"
                }
            },
            {
                "Azure", new Provider()
                {
                    ProviderId = "Azure",
                    Type = AIEndpointType.OpenAI,
                    Name = "Azure OpenAI",
                    Endpoint = "https://YOUR_RESOURCE_NAME.openai.azure.com",
                    Key = "",
                    Model = new string[] { "gpt-35-turbo", "gpt-4", "gpt-4-turbo" },
                    DefaultModel = "gpt-35-turbo"
                }
            },
            {
                "Moonshot", new Provider()
                {
                    ProviderId = "Moonshot",
                    Type = AIEndpointType.OpenAI,
                    Name = "月之暗面",
                    Endpoint = "https://api.moonshot.cn/v1",
                    Key = "",
                    Model = new string[] { "moonshot-v1-8k", "moonshot-v1-32k", "moonshot-v1-128k" },
                    DefaultModel = "moonshot-v1-8k"
                }
            },
            {
                "ChatGLM", new Provider()
                {
                    ProviderId = "ChatGLM",
                    Type = AIEndpointType.OpenAI,
                    Name = "智谱AI",
                    Endpoint = "https://open.bigmodel.cn/api/paas/v3",
                    Key = "",
                    Model = new string[] { "glm-3-turbo", "glm-4", "glm-4v" },
                    DefaultModel = "glm-3-turbo"
                }
            },
            {
                "Qwen", new Provider()
                {
                    ProviderId = "Qwen",
                    Type = AIEndpointType.OpenAI,
                    Name = "通义千问",
                    Endpoint = "https://dashscope.aliyuncs.com/api/v1",
                    Key = "",
                    Model = new string[] { "qwen-turbo", "qwen-plus", "qwen-max" },
                    DefaultModel = "qwen-turbo"
                }
            },
            {
                "SparkDesk", new Provider()
                {
                    ProviderId = "SparkDesk",
                    Type = AIEndpointType.OpenAI,
                    Name = "讯飞星火",
                    Endpoint = "https://spark-api.xf-yun.com/v1",
                    Key = "",
                    Model = new string[] { "spark-v2.0", "spark-v3.0", "spark-v3.5" },
                    DefaultModel = "spark-v3.0"
                }
            },
            {
                "Claude", new Provider()
                {
                    ProviderId = "Claude",
                    Type = AIEndpointType.OpenAI,
                    Name = "Claude AI",
                    Endpoint = "https://api.anthropic.com/v1",
                    Key = "",
                    Model = new string[]
                        { "claude-3-opus-20240229", "claude-3-sonnet-20240229", "claude-3-haiku-20240307" },
                    DefaultModel = "claude-3-sonnet-20240229"
                }
            },
            {
                "TogetherAI", new Provider()
                {
                    ProviderId = "TogetherAI",
                    Type = AIEndpointType.OpenAI,
                    Name = "Together AI",
                    Endpoint = "https://api.together.xyz/v1",
                    Key = "",
                    Model = new string[] { "mistral-7b-instruct", "llama-2-70b-chat", "openchat-7b" },
                    DefaultModel = "mistral-7b-instruct"
                }
            },
            {
                "OLLAMA", new Provider()
                {
                    ProviderId = "OLLAMA",
                    Type = AIEndpointType.OLLAMA,
                    Name = "LLAMA 本地模型",
                    Endpoint = "http://localhost:8080",
                    Key = "",
                    Model = new string[] { "llama-2-7b", "llama-2-13b", "llama-2-70b", "llama-3-8b", "llama-3-70b" },
                    DefaultModel = "llama-2-7b"
                }
            }
        };

        public string ProviderId { get; set; } = "";
        public AIEndpointType Type { get; set; } = AIEndpointType.OpenAI;
        public string Name { get; set; } = "自定义";
        public string Endpoint { get; set; } = "";
        public string Key { get; set; } = "";
        public string[] Model { get; set; } = [];
        public string DefaultModel { get; set; } = "";
        public bool Enabled { get; set; }
        

        public Provider Clone()
        {
            return new Provider
            {
                ProviderId = this.ProviderId,
                Type = this.Type,
                Name = this.Name,
                Endpoint = this.Endpoint,
                Key = this.Key,
                Model = this.Model,
                DefaultModel = this.DefaultModel
            };
        }
    }

    public class ChatPreset
    {
        public string Name { get; set; } = "默认";
        public List<ContextMessage> ContextMessages { get; set; } = new List<ContextMessage>();
        public double Temperature { get; set; } = 0.7f; // 默认温度0.7
        public int MaxTokens { get; set; } = 2000; // 默认最大生成长度2000
        public float TopP { get; set; } = 1.0f; // 默认top-p为1.0
        public int TopK { get; set; } = 40; // 默认top-k为40
        public double RepetitionPenalty { get; set; } = 1.0; // 默认重复惩罚为1.0
        public float FrequencyPenalty { get; set; } = 0.0f; // 默认频率惩罚为0
        public float PresencePenalty { get; set; } = 0.0f; // 默认存在惩罚为0
        public int ContextWindowSize { get; set; } = 4096; // 默认上下文窗口大小为4096
        public bool IsDefault { get; set; } = false; // 是否为默认预设
    }

    public class RemoteMCPServer
    {
        public string Name { get; set; } = "";
        public string Endpoint { get; set; } = "";
    }

    static ApplicationConfig()
    {
        // 获取保存配置的文件夹路径，优先使用 LocalApplicationData，如果为空则使用其他路径
        var savefolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(savefolder))
        {
            savefolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        if (string.IsNullOrEmpty(savefolder))
        {
            savefolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        // 设置配置文件夹路径并创建文件夹（如果不存在的话）
        savefolder = Path.Combine(savefolder, "DesktopAIShortcut");
        if (!Directory.Exists(savefolder))
        {
            Directory.CreateDirectory(savefolder);
        }

        // 设定最终的配置文件路径
        savefilename = Path.Combine(savefolder, "settingsV2.json");
    }

    // 保存配置文件的路径
    private static string savefilename;
    private static ApplicationConfig _instance = new ApplicationConfig();
    public List<Provider> Providers { get; set; } = new List<Provider>();
    public List<ChatPreset> ChatPresets { get; set; } = new List<ChatPreset>();
    public List<RemoteMCPServer> RemoteMCPServers { get; set; } = new List<RemoteMCPServer>();
    public void SaveSettings()
    {
        // 序列化 AISettings 对象为 JSON 格式，并写入文件
        File.WriteAllText(savefilename,
            System.Text.Json.JsonSerializer.Serialize(this, SourceGenerationContext.Default.ApplicationConfig));
        
        // 触发配置保存成功事件
        OnSettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    // 加载设置从配置文件
    public static void LoadSettings()
    {
        // 如果文件存在，则读取文件并反序列化
        if (File.Exists(savefilename))
        {
            string json = File.ReadAllText(savefilename);
            var settings =
                System.Text.Json.JsonSerializer.Deserialize<ApplicationConfig>(json,
                    SourceGenerationContext.Default.ApplicationConfig);
            _instance = settings ?? new ApplicationConfig();
        }
        else
        {
            _instance = new ApplicationConfig();
        }

        EnsureDefaults();
    }

    private static void EnsureDefaults()
    {
        if (_instance.ChatPresets != null && _instance.ChatPresets.Count > 0)
            return;

        _instance.ChatPresets = new List<ChatPreset>
        {
            new ChatPreset
            {
                Name = "通用助手",
                IsDefault = true,
                Temperature = 0.7,
                MaxTokens = 2000,
                ContextMessages = new List<ContextMessage>
                {
                    new ContextMessage { Role = "System", Content = "你是一个通用的AI助手，能够回答各种问题。", IsBeforeChat = false, Order = 0 }
                }
            },
            new ChatPreset
            {
                Name = "代码专家",
                Temperature = 0.3,
                MaxTokens = 3000,
                ContextMessages = new List<ContextMessage>
                {
                    new ContextMessage { Role = "System", Content = "你是一个专业的编程助手，擅长编写和解释代码，并提供详细的代码示例。", IsBeforeChat = false, Order = 0 }
                }
            },
            new ChatPreset
            {
                Name = "创意写作",
                Temperature = 0.9,
                MaxTokens = 2500,
                ContextMessages = new List<ContextMessage>
                {
                    new ContextMessage { Role = "System", Content = "你是一个创意写作助手，可以帮助用户创作故事、诗歌和其他创意内容。", IsBeforeChat = false, Order = 0 }
                }
            }
        };
    }

}