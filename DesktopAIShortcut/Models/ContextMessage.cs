using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace DesktopAIShortcut.Models
{
    public class ContextMessage : INotifyPropertyChanged
    {
        private string _role = "system";
        private string _content = "";
        private bool _isBeforeChat = true;
        private int _order;

        public string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged(nameof(Role));
                    OnPropertyChanged(nameof(RoleIndex));
                }
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public bool IsBeforeChat
        {
            get => _isBeforeChat;
            set
            {
                if (_isBeforeChat != value)
                {
                    _isBeforeChat = value;
                    OnPropertyChanged(nameof(IsBeforeChat));
                }
            }
        }

        public int Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OnPropertyChanged(nameof(Order));
                }
            }
        }

        [JsonIgnore]
        public int RoleIndex
        {
            get => Role.ToLower() switch
            {
                "system" => 0,
                "assistant" => 1,
                "user" => 2,
                _ => 0
            };
            set
            {
                Role = value switch
                {
                    0 => "System",
                    1 => "Assistant",
                    2 => "User",
                    _ => "System"
                };
            }
        }

        [JsonIgnore]
        public ChatMessage ChatMessage => Role.ToLower() switch
        {
            "system" => new SystemChatMessage(Content),
            "assistant" => new AssistantChatMessage(Content),
            "user" => new UserChatMessage(Content),
            _ => throw new ArgumentException("Invalid role")
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}