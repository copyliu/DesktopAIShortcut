using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopAIShortcut.Models
{
    [Obsolete("弃用",true)]
    public class ChatHistoryModel:INotifyPropertyChanged
    {
        private string _title = "";
        private DateTime _createTime = DateTime.Now;
        private string _id = Guid.NewGuid().ToString();
        private string _title1 = "";
        private DateTime _createTime1 = DateTime.Now;
        private DateTime _lastUpdateTime = DateTime.Now;
        private List<ChatMsgModel> _messages = new List<ChatMsgModel>();
        private string _providerName;
        private string _modelName;
        private string _promptName;
        private bool _singleAnswerMode;

        public string Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Title
        {
            get => _title1;
            set => SetField(ref _title1, value);
        }

        public DateTime CreateTime
        {
            get => _createTime1;
            set => SetField(ref _createTime1, value);
        }

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set => SetField(ref _lastUpdateTime, value);
        }

        public List<ChatMsgModel> Messages
        {
            get => _messages;
            set => SetField(ref _messages, value);
        }

        public string ProviderName
        {
            get => _providerName;
            set => SetField(ref _providerName, value);
        }

        public string ModelName
        {
            get => _modelName;
            set => SetField(ref _modelName, value);
        }

        public string PromptName
        {
            get => _promptName;
            set => SetField(ref _promptName, value);
        }

        public bool SingleAnswerMode
        {
            get => _singleAnswerMode;
            set => SetField(ref _singleAnswerMode, value);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}