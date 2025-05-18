using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Data.Converters;
using DesktopAIShortcut.Models;
using Avalonia.Threading;
using System.Linq;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut.ViewModels
{
    public class ModelProvidersSettingsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ModelProvider> _providers;
        private ModelProvider _selectedProvider;
        private bool _isTesting;

        public ObservableCollection<ModelProvider> Providers
        {
            get => _providers;
            set => SetProperty(ref _providers, value);
        }

        public ModelProvider SelectedProvider
        {
            get => _selectedProvider;
            set => SetProperty(ref _selectedProvider, value);
        }

        public bool IsTesting
        {
            get => _isTesting;
            set => SetProperty(ref _isTesting, value);
        }

        
        
        public ICommand TestConnection { get; }
        public ICommand SaveSettings { get; }
        public ICommand AddCustomModel { get; }
        public ICommand RemoveSelectedModel { get; }
        public ICommand ToggleModelSelection { get; }

        public ModelProvidersSettingsViewModel()
        {
            // 初始化提供商列表 
            Providers = new ObservableCollection<ModelProvider>();
            
            foreach (var provider in ApplicationConfig.Provider.ProviderTemplates)
            {
                var selected = ApplicationConfig.Instance.Providers.FirstOrDefault(p => p.ProviderName == provider.Key);
                
                var modelProvider = new ModelProvider
                {
                    Name = provider.Value.Name,
                    IsEnabled = selected?.Enabled==true,
                    ApiEndpoint = selected?.Endpoint ?? provider.Value.Endpoint,
                    AvailableModels = new ObservableCollection<string>(provider.Value.Model),
                    SelectedModels = new ObservableCollection<string>(selected?.Model ?? new string[] { }),
                    ApiKey = selected?.Key ?? "",
                    
                };
                
                Providers.Add(modelProvider);
            }
            
            //
            // {
            //     new ModelProvider 
            //     { 
            //         Name = "OpenAI", 
            //         IsEnabled = true,
            //         ApiEndpoint = "https://api.openai.com/v1",
            //         AvailableModels = new ObservableCollection<string> 
            //         { 
            //             "gpt-3.5-turbo", 
            //             "gpt-4", 
            //             "gpt-4-turbo"
            //         }
            //     },
            //     new ModelProvider 
            //     { 
            //         Name = "Anthropic", 
            //         IsEnabled = false,
            //         ApiEndpoint = "https://api.anthropic.com/v1",
            //         AvailableModels = new ObservableCollection<string> 
            //         { 
            //             "claude-1", 
            //             "claude-2", 
            //             "claude-instant"
            //         }
            //     }
            // };

            SelectedProvider = Providers.FirstOrDefault();
            
            // 初始化命令
            TestConnection = new RelayCommand(async _ => await TestConnectionAsync());
            SaveSettings = new RelayCommand(_ => SaveSettingsExecute());
            AddCustomModel = new RelayCommand(_ => AddCustomModelExecute());
            RemoveSelectedModel = new RelayCommand(param => RemoveSelectedModelExecute(param as string));
            ToggleModelSelection = new RelayCommand(param => ToggleModelSelectionExecute(param as string));
        }

        private async Task TestConnectionAsync()
        {
            if (SelectedProvider == null) return;

            IsTesting = true;
            // 这里应该实现实际的API测试逻辑
            await Task.Delay(1000); // 模拟测试过程
            IsTesting = false;
        }

        private void SaveSettingsExecute()
        {
            // 保存设置的逻辑
            var providerList=new List<ApplicationConfig.Provider>();
            foreach (var provider in Providers)
            {
                ApplicationConfig.Provider.ProviderTemplates.TryGetValue(provider.Name, out var template);
                var selectedProvider = new ApplicationConfig.Provider
                {
                    ProviderName = provider.Name,
                    Endpoint = provider.ApiEndpoint,
                    Key = provider.ApiKey,
                    Model = provider.SelectedModels.ToArray(),
                    Enabled = provider.IsEnabled,
                    Name = provider.Name,
                    Type = template?.Type?? ApplicationConfig.AIEndpointType.OpenAI,//TODO
                    DefaultModel = provider.SelectedModels.FirstOrDefault()??"",
                    
                };
                
                providerList.Add(selectedProvider);
            }
            ApplicationConfig.Instance.Providers = providerList;
            ApplicationConfig.Instance.SaveSettings();
        }

        private void AddCustomModelExecute()
        {
            if (SelectedProvider == null || string.IsNullOrWhiteSpace(SelectedProvider.CustomModelInput)) 
                return;
                
            var modelName = SelectedProvider.CustomModelInput.Trim();
            
            // 确保不重复添加
            if (!SelectedProvider.AvailableModels.Contains(modelName))
            {
                SelectedProvider.AvailableModels.Add(modelName);
             ToggleModelSelection.Execute(modelName);
            }
            
            // 清空输入框
            SelectedProvider.CustomModelInput = string.Empty;
        }

        private void RemoveSelectedModelExecute(string modelName)
        {
            if (SelectedProvider == null || string.IsNullOrEmpty(modelName)) 
                return;
                
            SelectedProvider.SelectedModels.Remove(modelName);
        }

        private void ToggleModelSelectionExecute(string modelName)
        {
            if (SelectedProvider == null || string.IsNullOrEmpty(modelName)) 
                return;
                
            // 根据选中状态添加或移除模型
            if (SelectedProvider.SelectedModels.Contains(modelName))
            {
                SelectedProvider.SelectedModels.Remove(modelName);
            }
            else
            {
                SelectedProvider.SelectedModels.Add(modelName);
            }
       
        }

        // 检查模型是否被选中的辅助方法
   

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 简单的Command实现
    public class RelayCommand : ICommand
    {
        private readonly System.Action<object> _execute;
        private readonly System.Func<object, bool> _canExecute;

        public RelayCommand(System.Action<object> execute, System.Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event System.EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
    }
}

// 添加一个用于模型选择状态转换的转换器
public class ModelSelectionMultiConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        var selectedModels = values[0] as ObservableCollection<string>;
        var modelName = values[1] as string;
        return selectedModels != null && modelName != null && selectedModels.Contains(modelName);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
