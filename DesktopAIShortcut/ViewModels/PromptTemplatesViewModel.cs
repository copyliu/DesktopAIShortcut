using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DesktopAIShortcut.Commands;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut.ViewModels
{
    public class PromptTemplatesViewModel : INotifyPropertyChanged
    {
        private PromptTemplate _selectedTemplate;
        private string _newTemplateName;
        private ChatMessage _selectedPrefixMessage;
        private ChatMessage _selectedSuffixMessage;

        public PromptTemplatesViewModel()
        {
            PromptTemplates = new ObservableCollection<PromptTemplate>();
            LoadDefaultTemplates();

            // 命令初始化
            AddPrefixMessageCommand = new RelayCommand(_ => AddPrefixMessage());
            RemovePrefixMessageCommand = new RelayCommand(_ => RemovePrefixMessage(), _ => SelectedPrefixMessage != null);
            AddSuffixMessageCommand = new RelayCommand(_ => AddSuffixMessage());
            RemoveSuffixMessageCommand = new RelayCommand(_ => RemoveSuffixMessage(), _ => SelectedSuffixMessage != null);
        }

        public ObservableCollection<PromptTemplate> PromptTemplates { get; }

        public PromptTemplate SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                if (_selectedTemplate != value)
                {
                    _selectedTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChatMessage SelectedPrefixMessage
        {
            get => _selectedPrefixMessage;
            set
            {
                if (_selectedPrefixMessage != value)
                {
                    _selectedPrefixMessage = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ChatMessage SelectedSuffixMessage
        {
            get => _selectedSuffixMessage;
            set
            {
                if (_selectedSuffixMessage != value)
                {
                    _selectedSuffixMessage = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string NewTemplateName
        {
            get => _newTemplateName;
            set
            {
                if (_newTemplateName != value)
                {
                    _newTemplateName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddPrefixMessageCommand { get; }
        public ICommand RemovePrefixMessageCommand { get; }
        public ICommand AddSuffixMessageCommand { get; }
        public ICommand RemoveSuffixMessageCommand { get; }

        private void LoadDefaultTemplates()
        {
            // 添加一些默认模板
            var generalTemplate = new PromptTemplate
            {
                Name = "通用助手",
                SystemPrompt = "你是一个有用的AI助手，可以回答用户的各种问题。",
                IsDefault = true
            };
            generalTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.User, Content = "你好，能帮我解答一些问题吗？" });
            generalTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.Assistant, Content = "你好！我很乐意帮你解答问题。请问你有什么需要我帮助的呢？" });

            PromptTemplates.Add(generalTemplate);

            var codeTemplate = new PromptTemplate
            {
                Name = "代码专家",
                SystemPrompt = "你是一个专业的编程助手，擅长编写和解释代码，并提供详细的代码示例。",
                Temperature = 0.3,
                MaxTokens = 3000
            };
            codeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.User, Content = "我需要你帮我解决一些编程问题。" });
            codeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.Assistant, Content = "当然，很高兴能帮你解决编程问题。请描述你遇到的问题或者需要什么样的代码帮助。" });

            PromptTemplates.Add(codeTemplate);

            var creativeTemplate = new PromptTemplate
            {
                Name = "创意写作",
                SystemPrompt = "你是一个创意写作助手，可以帮助用户创作故事、诗歌和其他创意内容。",
                Temperature = 0.9,
                MaxTokens = 2500
            };
            creativeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.User, Content = "我想创作一些有创意的内容。" });
            creativeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.Assistant, Content = "太棒了！我很乐意帮你进行创意写作。你有什么特定的题材、风格或类型的创作想法吗？" });

            PromptTemplates.Add(creativeTemplate);

            // 设置默认选中的模板
            SelectedTemplate = PromptTemplates[0];
        }

        public void AddNewTemplate()
        {
            if (string.IsNullOrWhiteSpace(NewTemplateName))
                return;

            var newTemplate = new PromptTemplate
            {
                Name = NewTemplateName,
                SystemPrompt = "你是一个有用的AI助手。"
            };

            PromptTemplates.Add(newTemplate);
            SelectedTemplate = newTemplate;
            NewTemplateName = string.Empty;
        }

        public void DeleteSelectedTemplate()
        {
            if (SelectedTemplate == null || SelectedTemplate.IsDefault)
                return;

            PromptTemplates.Remove(SelectedTemplate);
            SelectedTemplate = PromptTemplates.Count > 0 ? PromptTemplates[0] : null;
        }

        public void AddPrefixMessage()
        {
            if (SelectedTemplate == null) return;
            
            var newMessage = new ChatMessage { Role = ChatRole.User, Content = "" };
            SelectedTemplate.PrefixMessages.Add(newMessage);
            SelectedPrefixMessage = newMessage;
        }

        public void RemovePrefixMessage()
        {
            if (SelectedTemplate == null || SelectedPrefixMessage == null) return;
            
            SelectedTemplate.PrefixMessages.Remove(SelectedPrefixMessage);
            SelectedPrefixMessage = null;
        }

        public void AddSuffixMessage()
        {
            if (SelectedTemplate == null) return;
            
            var newMessage = new ChatMessage { Role = ChatRole.User, Content = "" };
            SelectedTemplate.SuffixMessages.Add(newMessage);
            SelectedSuffixMessage = newMessage;
        }

        public void RemoveSuffixMessage()
        {
            if (SelectedTemplate == null || SelectedSuffixMessage == null) return;
            
            SelectedTemplate.SuffixMessages.Remove(SelectedSuffixMessage);
            SelectedSuffixMessage = null;
        }

        public void SaveTemplates()
        {
            // TODO: 实现模板保存到配置文件的逻辑
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
