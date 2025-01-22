using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut.Services
{
    /// <summary>
    /// 聊天历史记录服务类
    /// 负责处理聊天历史的保存、加载和管理
    /// </summary>
    public class ChatHistoryService
    {
        // 存储聊天历史记录的文件夹路径
        private readonly string _historyFolder;
        // 应用程序数据文件夹路径
        private readonly string _appDataFolder;

        /// <summary>
        /// 构造函数：初始化服务，创建必要的文件夹
        /// </summary>
        public ChatHistoryService()
        {
            // 获取Windows系统中的AppData文件夹路径，并在其中创建我们的应用文件夹
            _appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DesktopAIShortcut"
            );

            // 确保应用数据文件夹存在
            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }

            // 在应用数据文件夹中创建存储聊天历史的子文件夹
            _historyFolder = Path.Combine(_appDataFolder, "ChatHistory");
            if (!Directory.Exists(_historyFolder))
            {
                Directory.CreateDirectory(_historyFolder);
            }
        }

        /// <summary>
        /// 验证并清理消息列表，合并来自同一角色的连续消息
        /// </summary>
        /// <param name="chat">需要处理的聊天历史记录</param>
        /// <returns>处理后的聊天历史记录</returns>
        private ChatHistoryModel ValidateAndCleanMessages(ChatHistoryModel chat)
        {
            // 如果聊天记录为空，直接返回
            if (chat?.Messages == null || !chat.Messages.Any())
            {
                return chat;
            }

            // 用于存储清理后的消息列表
            var cleanedMessages = new List<ChatMsgModel>();

            // 过滤掉空消息，并获取所有有效消息
            var validMessages = chat.Messages
                .Where(m => !string.IsNullOrWhiteSpace(m.Markdown))
                .ToList();

            // 如果没有有效消息，返回原始聊天记录
            if (!validMessages.Any())
            {
                return chat;
            }

            // 初始化第一条消息
            var currentMessage = new ChatMsgModel
            {
                IsUser = validMessages[0].IsUser,
                UserName = validMessages[0].UserName,
                Markdown = validMessages[0].Markdown
            };

            // 从第二条消息开始处理
            for (int i = 1; i < validMessages.Count; i++)
            {
                var message = validMessages[i];

                // 如果当前消息与上一条消息来自同一角色
                if (currentMessage.IsUser == message.IsUser)
                {
                    // 合并消息内容，确保只添加一次
                    currentMessage.Markdown = currentMessage.Markdown + "\n\n" + message.Markdown;

                    // 如果当前消息没有用户名，使用新消息的用户名
                    if (string.IsNullOrEmpty(currentMessage.UserName))
                    {
                        currentMessage.UserName = message.UserName;
                    }
                }
                else
                {
                    // 如果角色不同，保存当前消息并开始新的消息
                    cleanedMessages.Add(currentMessage);
                    currentMessage = new ChatMsgModel
                    {
                        IsUser = message.IsUser,
                        UserName = message.UserName,
                        Markdown = message.Markdown
                    };
                }
            }

            // 添加最后一条处理的消息
            cleanedMessages.Add(currentMessage);

            // 更新聊天记录中的消息列表
            chat.Messages = cleanedMessages;
            return chat;
        }

        /// <summary>
        /// 保存聊天记录到文件
        /// </summary>
        /// <param name="chat">要保存的聊天记录</param>
        public void SaveChat(ChatHistoryModel chat)
        {
            try
            {
                // 更新最后修改时间为当前时间
                chat.LastUpdateTime = DateTime.Now;

                // 如果聊天标题为空，从第一条消息生成标题
                if (string.IsNullOrEmpty(chat.Title))
                {
                    var firstMsg = chat.Messages.FirstOrDefault()?.Markdown;
                    chat.Title = !string.IsNullOrEmpty(firstMsg)
                        ? firstMsg.Substring(0, Math.Min(30, firstMsg.Length)) // 取前30个字符作为标题
                        : "新对话";
                }

                // 在保存前验证和清理消息
                chat = ValidateAndCleanMessages(chat);

                // 生成保存文件的完整路径
                var filePath = Path.Combine(_historyFolder, $"{chat.Id}.json");

                // 将聊天记录转换为JSON格式（使用缩进使文件更易读）
                var json = JsonSerializer.Serialize(chat, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // 将JSON内容写入文件
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // 如果保存过程中出现错误，将错误信息输出到控制台
                Console.WriteLine($"保存聊天记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载所有聊天记录
        /// </summary>
        /// <returns>按最后更新时间排序的聊天记录列表</returns>
        public List<ChatHistoryModel> LoadAllChats()
        {
            var chats = new List<ChatHistoryModel>();

            try
            {
                // 如果历史记录文件夹不存在，返回空列表
                if (!Directory.Exists(_historyFolder))
                {
                    return chats;
                }

                // 获取文件夹中所有的JSON文件
                foreach (var file in Directory.GetFiles(_historyFolder, "*.json"))
                {
                    try
                    {
                        // 读取JSON文件内容
                        var json = File.ReadAllText(file);
                        // 将JSON转换为聊天记录对象
                        var chat = JsonSerializer.Deserialize<ChatHistoryModel>(json);
                        if (chat != null)
                        {
                            // 验证和清理消息后添加到列表
                            chat = ValidateAndCleanMessages(chat);
                            chats.Add(chat);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 如果单个文件读取失败，记录错误并继续处理其他文件
                        Console.WriteLine($"读取聊天记录文件失败 {file}: {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果整体加载过程失败，记录错误
                Console.WriteLine($"加载聊天记录失败: {ex.Message}");
            }

            // 返回按最后更新时间降序排序的聊天记录列表
            return chats.OrderByDescending(x => x.LastUpdateTime).ToList();
        }

        /// <summary>
        /// 加载指定ID的聊天记录
        /// </summary>
        /// <param name="id">聊天记录的唯一标识符</param>
        /// <returns>加载的聊天记录，如果找不到则返回新的空聊天记录</returns>
        public ChatHistoryModel LoadChat(string id)
        {
            try
            {
                // 构建完整的文件路径
                var filePath = Path.Combine(_historyFolder, $"{id}.json");

                // 如果文件不存在，返回新的空聊天记录
                if (!File.Exists(filePath))
                {
                    return new ChatHistoryModel();
                }

                // 读取并解析JSON文件
                var json = File.ReadAllText(filePath);
                var chat = JsonSerializer.Deserialize<ChatHistoryModel>(json) ?? new ChatHistoryModel();

                // 验证和清理消息后返回
                return ValidateAndCleanMessages(chat);
            }
            catch (Exception ex)
            {
                // 如果加载过程中出现错误，记录错误并返回新的空聊天记录
                Console.WriteLine($"加载聊天记录失败 {id}: {ex.Message}");
                return new ChatHistoryModel();
            }
        }

        /// <summary>
        /// 删除指定ID的聊天记录
        /// </summary>
        /// <param name="id">要删除的聊天记录的唯一标识符</param>
        public void DeleteChat(string id)
        {
            try
            {
                // 构建完整的文件路径
                var filePath = Path.Combine(_historyFolder, $"{id}.json");

                // 如果文件存在则删除
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // 如果删除过程中出现错误，记录错误
                Console.WriteLine($"删除聊天记录失败 {id}: {ex.Message}");
            }
        }
    }
}