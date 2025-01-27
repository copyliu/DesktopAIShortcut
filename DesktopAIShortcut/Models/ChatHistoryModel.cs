using System;
using System.Collections.Generic;

namespace DesktopAIShortcut.Models
{
    public class ChatHistoryModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;
        public List<ChatMsgModel> Messages { get; set; } = new List<ChatMsgModel>();
    }
}