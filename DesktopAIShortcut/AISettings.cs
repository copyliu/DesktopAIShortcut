using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut
{
    public class AISettings : INotifyPropertyChanged
    {
        public static AISettings Instance { get; } = new AISettings();
        
        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;

        // 用于触发属性改变通知的辅助方法
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool _enableAdvancedSettings = true;
        // 各个配置项
        public string AIName { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string Key { get; set; } = "";
        public string SysPrompt { get; set; } = "";
        public string Model { get; set; } = "";
        public List<ContextMessage> ContextMessages { get; set; } = new List<ContextMessage>();

        // 新增的AI参数配置
        public float Temperature { get; set; } = 0.7f;  // 默认温度0.7
        public int MaxTokens { get; set; } = 2000;      // 默认最大生成长度2000
        public float TopP { get; set; } = 1.0f;         // 默认top-p为1.0
        public int TopK { get; set; } = 40;             // 默认top-k为40
        public double RepetitionPenalty { get; set; } = 1.0;  // 默认重复惩罚为1.0
        public float FrequencyPenalty { get; set; } = 0.0f;   // 默认频率惩罚为0
        public float PresencePenalty { get; set; } = 0.0f;    // 默认存在惩罚为0
        public int ContextWindowSize { get; set; } = 4096;    // 默认上下文窗口大小为4096

        public bool EnableAdvancedSettings
        {
            get => _enableAdvancedSettings;
            set
            {
                if (_enableAdvancedSettings != value)
                {
                    _enableAdvancedSettings = value;
                    OnPropertyChanged(); // 触发属性改变通知
                }
            }
        }
        public bool EnableLogging { get; set; } = false;

        // 保存配置文件的路径
        private static string savefilename;

        static AISettings()
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
            savefilename = Path.Combine(savefolder, "settings.json");
        }

        // 保存设置到配置文件
        public void SaveSettings()
        {
            // 序列化 AISettings 对象为 JSON 格式，并写入文件
            File.WriteAllText(savefilename, System.Text.Json.JsonSerializer.Serialize(this, SourceGenerationContext.Default.AISettings));
        }

        // 加载设置从配置文件
        public void LoadSettings()
        {
            // 如果文件存在，则读取文件并反序列化
            if (File.Exists(savefilename))
            {
                string json = File.ReadAllText(savefilename);
                var settings = System.Text.Json.JsonSerializer.Deserialize<AISettings>(json, SourceGenerationContext.Default.AISettings);
                if (settings != null)
                {
                    // 如果反序列化成功，则将设置加载到当前对象
                    AIName = settings.AIName;
                    Endpoint = settings.Endpoint;
                    Key = settings.Key;
                    SysPrompt = settings.SysPrompt;
                    Model = settings.Model;
                    ContextMessages = settings.ContextMessages ?? new List<ContextMessage>();

                    // 加载新增的AI参数
                    EnableAdvancedSettings = settings.EnableAdvancedSettings;
                    EnableLogging = settings.EnableLogging;
                    Temperature = settings.Temperature;
                    MaxTokens = settings.MaxTokens;
                    TopP = settings.TopP;
                    TopK = settings.TopK;
                    RepetitionPenalty = settings.RepetitionPenalty;
                    FrequencyPenalty = settings.FrequencyPenalty;
                    PresencePenalty = settings.PresencePenalty;
                    ContextWindowSize = settings.ContextWindowSize;
                }
            }
            else
            {
                // 如果文件不存在，则创建一个空的设置文件
                File.WriteAllText(savefilename, "{}");
            }
        }
    }
}