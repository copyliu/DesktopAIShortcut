using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut.ViewModels
{
    public class ModelProvidersSettingsViewModel : INotifyPropertyChanged
    {
        private ModelProvider _selectedProvider;
        private bool _isTesting;

        public ModelProvidersSettingsViewModel()
        {
            Providers = new ObservableCollection<ModelProvider>();
            InitializeProviders();
        }

        public ObservableCollection<ModelProvider> Providers { get; }

        public ModelProvider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider != value)
                {
                    _selectedProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsTesting
        {
            get => _isTesting;
            set
            {
                if (_isTesting != value)
                {
                    _isTesting = value;
                    OnPropertyChanged();
                }
            }
        }

        private void InitializeProviders()
        {
            // 添加常见的大模型提供商
            Providers.Add(new ModelProvider
            {
                Name = "OpenAI",
                IsEnabled = true,
                ApiEndpoint = "https://api.openai.com/v1",
                AvailableModels = new List<string> { "gpt-4o", "gpt-4", "gpt-3.5-turbo" }
            });

            Providers.Add(new ModelProvider
            {
                Name = "Anthropic",
                IsEnabled = false,
                ApiEndpoint = "https://api.anthropic.com/v1",
                AvailableModels = new List<string> { "claude-3-opus", "claude-3-sonnet", "claude-3-haiku" }
            });

            Providers.Add(new ModelProvider
            {
                Name = "智谱AI",
                IsEnabled = false,
                ApiEndpoint = "https://open.bigmodel.cn/api/paas/v4",
                AvailableModels = new List<string> { "glm-4", "glm-3-turbo" }
            });

            Providers.Add(new ModelProvider
            {
                Name = "百度文心",
                IsEnabled = false,
                ApiEndpoint = "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/chat",
                AvailableModels = new List<string> { "ernie-bot-4", "ernie-bot" }
            });

            // 设置默认选中的提供商
            SelectedProvider = Providers[0];
        }

        public async void TestConnection()
        {
            if (SelectedProvider == null || string.IsNullOrEmpty(SelectedProvider.ApiKey))
                return;

            IsTesting = true;
            
            // TODO: 实现实际的API连接测试逻辑
            await Task.Delay(1000); // 模拟测试过程

            // 测试完成后显示结果
            // 在实际应用中，应该实现真正的连接测试逻辑
            
            IsTesting = false;
        }

        public void SaveSettings()
        {
            // TODO: 保存设置到配置文件
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
