using DesktopAIShortcut.Models;

namespace DesktopAIShortcut.ViewModels;

public class ModelSelectionViewModel(ApplicationConfig.Provider selectedProvider, string selectedModel)
{
    public ApplicationConfig.Provider SelectedProvider { get; set; } = selectedProvider;
    public string SelectedModel { get; set; } = selectedModel;
}