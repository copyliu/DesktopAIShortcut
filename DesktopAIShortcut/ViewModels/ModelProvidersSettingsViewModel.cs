using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DesktopAIShortcut.Models;
using Avalonia.Threading;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut.ViewModels;

public partial class ModelProvidersSettingsViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ModelProvider> _providers;
    private ModelProvider _selectedProvider;
    private bool _isTesting;

    public ObservableCollection<ApplicationConfig.AIEndpointType> ProviderTypes { get; }=
    [
        ApplicationConfig.AIEndpointType.OpenAI,
        ApplicationConfig.AIEndpointType.Gemini,
        ApplicationConfig.AIEndpointType.OLLAMA
    ];
        
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

         

    public ModelProvidersSettingsViewModel()
    {
        // 初始化提供商列表 
        Providers = new ObservableCollection<ModelProvider>();
            
        foreach (var templateProvider in ApplicationConfig.Provider.ProviderTemplates)
        {
            var selected = ApplicationConfig.Instance.Providers.FirstOrDefault(p => p.ProviderName == templateProvider.Key);
                
            var modelProvider = new ModelProvider
            {
                ProviderName=templateProvider.Value.ProviderName,
                Name = templateProvider.Value.Name,
                IsEnabled = selected?.Enabled==true,
                ApiEndpoint = selected?.Endpoint ?? templateProvider.Value.Endpoint,
                AvailableModels = new ObservableCollection<string>(templateProvider.Value.Model.Concat(selected?.Model ?? new string[] { })),
                SelectedModels = new ObservableCollection<string>(selected?.Model ?? new string[] { }),
                ApiKey = selected?.Key ?? "",
                EndpointType = templateProvider.Value.Type  ,
                FromTemplate =true,
            };
                
            Providers.Add(modelProvider);
        }

        foreach (var provider in ApplicationConfig.Instance.Providers.Where(p=>!ApplicationConfig.Provider.ProviderTemplates.Keys.Contains(p.ProviderName)))
        {
            var modelProvider = new ModelProvider
            {
                ProviderName= provider.ProviderName,
                Name = provider.Name,
                IsEnabled = provider.Enabled==true,
                ApiEndpoint = provider.Endpoint  ,
                AvailableModels = new ObservableCollection<string>( provider.Model ?? new string[] { }),
                SelectedModels = new ObservableCollection<string>(provider.Model ?? new string[] { }),
                ApiKey = provider.Key,
                EndpointType = provider.Type  ,
                FromTemplate =false,
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
             
    }
    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (SelectedProvider == null) return;

        IsTesting = true;
        // 这里应该实现实际的API测试逻辑
        await Task.Delay(1000); // 模拟测试过程
        IsTesting = false;
    }
    [RelayCommand]
    private void SaveSettings()
    {
        // 保存设置的逻辑
        var providerList=new List<ApplicationConfig.Provider>();
        foreach (var provider in Providers)
        {
            ApplicationConfig.Provider.ProviderTemplates.TryGetValue(provider.ProviderName, out var template);
            var selectedProvider = new ApplicationConfig.Provider
            {
                ProviderName = provider.ProviderName,
                Endpoint = provider.ApiEndpoint,
                Key = provider.ApiKey,
                Model = provider.SelectedModels.ToArray(),
                Enabled = provider.IsEnabled,
                Name = provider.Name,
                Type = template?.Type?? ApplicationConfig.AIEndpointType.OpenAI,
                DefaultModel = provider.SelectedModels.FirstOrDefault()??"",
                  
            };
                
            providerList.Add(selectedProvider);
        }
        ApplicationConfig.Instance.Providers = providerList;
        ApplicationConfig.Instance.SaveSettings();
    }
    [RelayCommand]
    private void AddCustomModel()
    {
        if (SelectedProvider == null || string.IsNullOrWhiteSpace(SelectedProvider.CustomModelInput)) 
            return;
                
        var modelName = SelectedProvider.CustomModelInput.Trim();
            
        // 确保不重复添加
        if (!SelectedProvider.AvailableModels.Contains(modelName))
        {
            SelectedProvider.AvailableModels.Add(modelName);
            this.ToggleModelSelection(modelName);
        }
            
        // 清空输入框
        SelectedProvider.CustomModelInput = string.Empty;
    }
    [RelayCommand]
    private void RemoveSelectedModel(string modelName)
    {
        if (SelectedProvider == null || string.IsNullOrEmpty(modelName)) 
            return;
                
        SelectedProvider.SelectedModels.Remove(modelName);
    }
    [RelayCommand]
    private void ToggleModelSelection(string modelName)
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