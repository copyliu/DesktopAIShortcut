using System;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut.ViewModels;

public class ModelSelectionViewModel(ApplicationConfig.Provider selectedProvider, string selectedModel)
{
    public ApplicationConfig.Provider SelectedProvider { get; set; } = selectedProvider;
    public string SelectedModel { get; set; } = selectedModel;

    public override bool Equals(object? obj)
    {
        if (obj is ModelSelectionViewModel other)
            return SelectedProvider.ProviderId == other.SelectedProvider.ProviderId
                && SelectedModel == other.SelectedModel;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SelectedProvider.ProviderId, SelectedModel);
    }
}