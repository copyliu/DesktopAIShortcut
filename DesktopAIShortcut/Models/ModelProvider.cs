using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DesktopAIShortcut.Models
{
    public class ModelProvider : INotifyPropertyChanged
    {
        private string _name;
        private string _apiKey;
        private bool _isEnabled;
        private string _apiEndpoint;
        private ObservableCollection<string> _selectedModels;
        private ObservableCollection<string> _availableModels;
        private string _customModelInput;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string ApiEndpoint
        {
            get => _apiEndpoint;
            set => SetProperty(ref _apiEndpoint, value);
        }

        public ObservableCollection<string> SelectedModels
        {
            get => _selectedModels;
            set => SetProperty(ref _selectedModels, value);
        }

        public ObservableCollection<string> AvailableModels
        {
            get => _availableModels;
            set => SetProperty(ref _availableModels, value);
        }

        public string CustomModelInput
        {
            get => _customModelInput;
            set => SetProperty(ref _customModelInput, value);
        }

        public ModelProvider()
        {
            SelectedModels = new ObservableCollection<string>();
            AvailableModels = new ObservableCollection<string>();
            CustomModelInput = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
