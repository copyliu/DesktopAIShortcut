using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopAIShortcut.Models
{
    public enum ChatRole
    {
        
        System,
        
        Assistant,
        User,
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private ChatRole _role = ChatRole.User;
        private string _content = "";

        public ChatRole Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
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
