using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DesktopAIShortcut.Models;

namespace DesktopAIShortcut
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AISettings))]
    [JsonSerializable(typeof(ContextMessage))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
   
    }
}
