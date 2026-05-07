using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace DesktopAIShortcut.Models
{
    public class PromptTemplate : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<ChatMessage> _prefixMessages = [];
        private ObservableCollection<ChatMessage> _suffixMessages = [];
        private double _temperature = 0.7;
        private int _maxTokens = 2000;
        private bool _isDefault;

        public PromptTemplate()
        {
            _name = "新模板";
        }

        public PromptTemplate(ApplicationConfig.ChatPreset chatPreset)
        {
            _name = chatPreset.Name;
            _temperature = chatPreset.Temperature;
            _maxTokens = chatPreset.MaxTokens;
            _isDefault = chatPreset.IsDefault;

            _prefixMessages = new ObservableCollection<ChatMessage>(
                chatPreset.ContextMessages
                    .Where(p => !p.IsBeforeChat)
                    .OrderBy(p => p.Order)
                    .Select(p => new ChatMessage()
                    {
                        Content = p.Content,
                        Role = new ChatRole(p.Role)
                    }));

            _suffixMessages = new ObservableCollection<ChatMessage>(
                chatPreset.ContextMessages
                    .Where(p => p.IsBeforeChat)
                    .OrderBy(p => p.Order)
                    .Select(p => new ChatMessage()
                    {
                        Content = p.Content,
                        Role = new ChatRole(p.Role)
                    }));
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


      

        public ObservableCollection<ChatMessage> PrefixMessages
        {
            get => _prefixMessages;
            set => SetProperty(ref _prefixMessages, value);
        }

        public ObservableCollection<ChatMessage> SuffixMessages
        {
            get => _suffixMessages;
            set => SetProperty(ref _suffixMessages, value);
        }

        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        public int MaxTokens
        {
            get => _maxTokens;
            set => SetProperty(ref _maxTokens, value);
        }

        public bool IsDefault
        {
            get => _isDefault;
            set => SetProperty(ref _isDefault, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is PromptTemplate other)
                return _name == other._name;
            return false;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}
