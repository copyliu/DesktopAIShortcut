using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DesktopAIShortcut.Models
{
    public class PromptTemplate : INotifyPropertyChanged
    {
        private string _name;
        private string _systemPrompt;
        private ObservableCollection<ChatMessage> _prefixMessages;
        private ObservableCollection<ChatMessage> _suffixMessages;
        private double _temperature = 0.7;
        private int _maxTokens = 2000;
        private bool _isDefault;

        public PromptTemplate()
        {
            PrefixMessages = new ObservableCollection<ChatMessage>();
            SuffixMessages = new ObservableCollection<ChatMessage>();
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
