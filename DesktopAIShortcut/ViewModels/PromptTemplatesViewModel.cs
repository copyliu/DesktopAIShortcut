using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DesktopAIShortcut.Commands;
using DesktopAIShortcut.Models;
using Microsoft.Extensions.AI;
using ChatMessage = DesktopAIShortcut.Models.ChatMessage;

namespace DesktopAIShortcut.ViewModels
{
    public partial class PromptTemplatesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection< ChatRole> ChatRoles { get; }=
        [
            ChatRole.System,
           ChatRole.Assistant,
           ChatRole.User,
            ChatRole.Tool
        ];
        private PromptTemplate _selectedTemplate;
        private string _newTemplateName;
        private ChatMessage _selectedPrefixMessage;
        private ChatMessage _selectedSuffixMessage;

        public PromptTemplatesViewModel()
        {
            PromptTemplates = new ObservableCollection<PromptTemplate>();

            foreach (var instanceChatPreset in ApplicationConfig.Instance.ChatPresets)
            {
                var template = new PromptTemplate
                {
                    Name = instanceChatPreset.Name,
                   PrefixMessages = new ObservableCollection<ChatMessage>( instanceChatPreset.ContextMessages.Where(p=>!p.IsBeforeChat)
                       .OrderBy(p=>p.Order)
                       
                       .Select(p=>new ChatMessage()
                       {
                           Content = p.Content,
                           Role = new ChatRole(p.Role)
                           
                       })),
                    Temperature = instanceChatPreset.Temperature,
                    MaxTokens = instanceChatPreset.MaxTokens,
                    IsDefault = instanceChatPreset.IsDefault,
                 SuffixMessages =  new ObservableCollection<ChatMessage>( instanceChatPreset.ContextMessages.Where(p=>p.IsBeforeChat)
                     .OrderBy(p=>p.Order)
                       
                     .Select(p=>new ChatMessage()
                     {
                         Content = p.Content,
                         Role = new ChatRole(p.Role)
                           
                     })),
                };
                PromptTemplates.Add(template);
            }
            
            
            
             



            if (PromptTemplates.Count == 0)
            {
                AddDefault();
                
            }
            
            
            
            
            SelectedTemplate = PromptTemplates[0];
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

      
        [RelayCommand]
        private void AddDefault()
        {
            // 添加一些默认模板
            var generalTemplate = new PromptTemplate
            {
                Name = "通用助手",
                IsDefault = true
            };
            generalTemplate.PrefixMessages.Add(new ChatMessage
                { Role = ChatRole.System, Content = "你是一个通用的AI助手，能够回答各种问题。" });
            
            PromptTemplates.Add(generalTemplate);

            var codeTemplate = new PromptTemplate
            {
                Name = "代码专家",
                Temperature = 0.3,
                MaxTokens = 3000
            };
            codeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.System, Content = "你是一个专业的编程助手，擅长编写和解释代码，并提供详细的代码示例。" });
           
            PromptTemplates.Add(codeTemplate);

            var creativeTemplate = new PromptTemplate
            {
                Name = "创意写作",
                Temperature = 0.9,
                MaxTokens = 2500
            };
            creativeTemplate.PrefixMessages.Add(new ChatMessage { Role = ChatRole.System, Content = "你是一个创意写作助手，可以帮助用户创作故事、诗歌和其他创意内容。" });
           
            PromptTemplates.Add(creativeTemplate);

        }

        [RelayCommand]
        public void AddNewTemplate()
        {
            if (string.IsNullOrWhiteSpace(NewTemplateName))
                return;

            var newTemplate = new PromptTemplate
            {
                Name = NewTemplateName,
            };

            PromptTemplates.Add(newTemplate);
            SelectedTemplate = newTemplate;
            NewTemplateName = string.Empty;
        }
        [RelayCommand]
        public void DeleteSelectedTemplate()
        {
            if (SelectedTemplate == null || SelectedTemplate.IsDefault)
                return;

            PromptTemplates.Remove(SelectedTemplate);
            SelectedTemplate = PromptTemplates.Count > 0 ? PromptTemplates[0] : null;
        }
        [RelayCommand]
        public void AddPrefixMessage()
        {
            if (SelectedTemplate == null) return;
            
            var newMessage = new ChatMessage { Role = ChatRole.User, Content = "" };
            SelectedTemplate.PrefixMessages.Add(newMessage);
            SelectedPrefixMessage = newMessage;
        }
        [RelayCommand]
        public void RemovePrefixMessage()
        {
            if (SelectedTemplate == null || SelectedPrefixMessage == null) return;
            
            SelectedTemplate.PrefixMessages.Remove(SelectedPrefixMessage);
            SelectedPrefixMessage = null;
        }
        [RelayCommand]
        public void AddSuffixMessage()
        {
            if (SelectedTemplate == null) return;
            
            var newMessage = new ChatMessage { Role = ChatRole.User, Content = "" };
            SelectedTemplate.SuffixMessages.Add(newMessage);
            SelectedSuffixMessage = newMessage;
        }
        [RelayCommand]
        public void RemoveSuffixMessage()
        {
            if (SelectedTemplate == null || SelectedSuffixMessage == null) return;
            
            SelectedTemplate.SuffixMessages.Remove(SelectedSuffixMessage);
            SelectedSuffixMessage = null;
        }
        [RelayCommand]
        public void SaveTemplates()
        {
            // TODO: 实现模板保存到配置文件的逻辑
            var promptlist=new List<ApplicationConfig.ChatPreset>();
            foreach (var template in PromptTemplates)
            {
                var chatPreset = new ApplicationConfig.ChatPreset
                {
                    Name = template.Name,
                    Temperature = template.Temperature,
                    MaxTokens = template.MaxTokens,
                    IsDefault = template.IsDefault,
                    ContextMessages =  new List<ContextMessage>()
                };

                int order = 0;
                foreach (var msg in template.PrefixMessages)
                {
                    chatPreset.ContextMessages.Add(new  ContextMessage
                    {
                        Role = msg.Role.ToString(),
                        Content = msg.Content,
                        IsBeforeChat = false,
                        Order = order++
                    });
                }
                foreach (var msg in template.SuffixMessages)
                {
                    chatPreset.ContextMessages.Add(new ContextMessage
                    {
                        Role = msg.Role.ToString(),
                        Content = msg.Content,
                        IsBeforeChat = true,
                        Order = order++
                    });
                }

                promptlist.Add(chatPreset);
            }
            ApplicationConfig.Instance.ChatPresets = promptlist;
            ApplicationConfig.Instance.SaveSettings();
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
