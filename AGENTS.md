# AI 编码工作流指令

> 本文档为 Kilo AI 编码助手提供在 DesktopAIShortcut 项目中的工作规范与上下文，遵循以下指令可确保生成的代码风格一致、符合项目架构、且能通过构建。

---

## 1. 项目概览

**DesktopAIShortcut** 是一款跨平台桌面 AI 聊天客户端，基于 .NET 10.0 + Avalonia UI 11.3 构建，通过 OpenAI SDK 对接多种 AI 模型提供商。支持流式对话、思考过程显示、Markdown 渲染、多模型管理、问答模式、聊天历史持久化等功能。

**许可证**: MPL 2.0

**目标平台**: Windows / macOS / Linux 桌面端 (AOT 兼容)

---

## 2. 技术栈

| 组件 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 运行时与 SDK |
| Avalonia UI | 11.3.11 | 跨平台 UI 框架 (Fluent 主题) |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM 工具包 (数据绑定、命令、消息) |
| OpenAI SDK | 2.10.0 | AI API 调用 (Chat Completions / Responses) |
| LiveMarkdown.Avalonia | 1.7.0 | 实时流式 Markdown 渲染 (ObservableStringBuilder) |
| Markdig | 0.44.0 | Markdown 处理 (提取纯文本) |
| Microsoft.Extensions.AI.Abstractions | 10.2.0 | AI 抽象层 |

**语言**: C# (.NET 10.0, file-scoped namespaces, nullable enabled)

**UI 范式**: MVVM (Model-View-ViewModel)，使用 CompiledBindings

---

## 3. 项目架构

```
DesktopAIShortcut/
├── Adapter/                  # AI API 适配器层
├── Models/                   # 数据模型 (纯数据，无逻辑)
├── ViewModels/               # 视图模型 (MVVM 的 VM 层)
├── Services/                 # 业务服务层
├── Commands/                 # 通用命令
├── Converters/               # XAML 值转换器
├── Settings/                 # 各设置页面的 View + ViewModel
├── Assets/                   # 图标与素材
├── App.axaml / App.axaml.cs  # Avalonia 应用入口
├── Program.cs                # .NET Main 入口
├── MainWindow.axaml / .cs    # 主聊天窗口
├── QAModeView.axaml / .cs    # 问答模式界面
├── SettingWindow.axaml / .cs # 设置窗口
└── ChatMsgControl.axaml / .cs # 单条消息气泡控件

DesktopAIShortcutPackaging/   # MSIX 打包项目 (仅 Windows)
```

**分层规则**:
- **Models**: 纯 C# 自动属性，无逻辑，可使用 `[JsonIgnore]` 标记序列化排除
- **ViewModels**: 持有状态、实现 `INotifyPropertyChanged`、调用 Service/Adapter，使用 `CommunityToolkit.Mvvm` 的 `[ObservableProperty]` 或手动 `SetField<T>` 模式
- **Services**: 负责持久化、IO、外部交互
- **Adapter**: 封装 OpenAI SDK 调用，处理异常并返回友好错误
- **Views**: `.axaml` 文件，使用 `{CompiledBinding}` 绑定到 VM

---

## 4. 代码规范

### 4.1 命名约定

| 类型 | 规范 | 示例 |
|------|------|------|
| 命名空间 | `DesktopAIShortcut.{目录名}` (file-scoped) | `namespace DesktopAIShortcut.ViewModels;` |
| 类名 | PascalCase | `ChatMsgViewModel` |
| 属性 | PascalCase | `public string Markdown { get; set; }` |
| 私有字段 | `_camelCase` 或 `camelCase` | `private string _userName;` |
| 方法 | PascalCase | `public void AppendLine(string line)` |
| 局部变量 | camelCase | `var cleanedMessages = ...` |
| XAML 控件名 | PascalCase | `<TextBox x:Name="MessageInput" />` |

### 4.2 ViewModel 模式

项目中 ViewModel 采用**手动 INotifyPropertyChanged 实现**，以 `ChatMsgViewModel` 为典型模板：

```csharp
public class ChatMsgViewModel : INotifyPropertyChanged
{
    private string _fieldName;

    public string FieldName
    {
        get => _fieldName;
        set
        {
            if (value == _fieldName) return;
            SetField(ref _fieldName, value);
            OnPropertyChanged();
            // 如需联动更新其他属性，在此处添加
        }
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
```

**关键点**:
- 新 ViewModel 应**模仿此模式**，保持项目内风格一致
- 不要在 ViewModel 中混用 `CommunityToolkit.Mvvm` 的 `[ObservableProperty]` 源码生成器（项目已有手动模式）
- 需要序列化排除的属性加 `[JsonIgnore]`

### 4.3 异常处理

- 所有 AI API 调用必须 try-catch，返回包含 `**错误**: {ex.Message}` 的友好消息
- 文件 IO 操作必须 try-catch，`Console.WriteLine` 记录错误

### 4.4 注释

- 使用 XML 文档注释 (`///`) 描述公开 API
- 不需要为简单 getter/setter 写注释
- 避免无意义的注释（如 `// 设置属性`）

### 4.5 using 排序

保持 System 命名空间在前，第三方中间，项目命名空间在后：
```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.AI;
using DesktopAIShortcut.Models;
```

---

## 5. Git 提交规范

### 5.1 提交消息格式

```
[动词] [简短描述]: [可选的详细说明]
```

动词分类:
- `新增` - 新功能、新文件
- `修复` - Bug 修复
- `实现` - 完成某个特性的实现
- `升级` - 依赖版本升级
- `优化` - 性能或代码结构优化
- `重构` - 无功能变化的代码重组
- `移除` - 删除功能或文件

### 5.2 提交消息示例

```
新增 ResponsesAdapter 支持 OpenAI Responses API 流式推理
修复推理过程不更新界面 + 思考完毕折叠而非隐藏
优化聊天历史加载性能，延迟加载消息体
```

- **使用简体中文**
- 一行标题控制在 50 字以内，详细信息换行写
- 不要写 "stuff"、"fix" 等无意义消息
- 提交信息末尾须包含 `Co-authored-by: {模型名称} <{模型名称}@{工具名称}>`

---

## 6. 构建与运行

### 6.1 命令

| 命令 | 说明 |
|------|------|
| `dotnet build DesktopAIShortcut/DesktopAIShortcut.csproj` | 编译项目 |
| `dotnet run --project DesktopAIShortcut/DesktopAIShortcut.csproj` | 运行应用 (Windows) |
| `dotnet publish DesktopAIShortcut/DesktopAIShortcut.csproj -c Release` | 发布 Release |

### 6.2 注意事项

- 在 Windows 上编译目标为 `net10.0-windows`，macOS/Linux 上为 `net10.0`
- 项目配置了 `IsAotCompatible=true`，避免使用不被 AOT 支持的特性（如 `MakeGenericType` 等动态反射）
- 已废弃的 `SettingWindowOld.axaml` / `.cs` 已在 `.csproj` 中排除编译
- `SettingWindowOld.axaml.cs` 不再使用，`Settings/` 目录下是新的分页设置窗口
- `DesktopAIShortcutPackaging/` 为 Windows 专属 MSIX 打包项目，日常开发不需改它

---

## 7. AI 编码工作流

### 7.1 理解需求

1. 先阅读与本需求相关的现有代码（Model、ViewModel、View、Service）
2. 确定修改范围：是否只需要改 ViewModel？是否需要新增 Service？是否需要改 .axaml？
3. 遵循现有分层，不要跨层写逻辑（如在 View 中调用 API）

### 7.2 实现步骤

1. **先查后写**: 使用 `grep` / `glob` 搜索相关代码，理解现有实现
2. **遵循模式**: 新代码严格模仿现有同类代码的模式（如新增 Adapter 就参照 `OpenAIAdapter.cs`）
3. **命名一致**: 变量、方法、属性命名与现有代码风格保持一致
4. **最小改动**: 只改必要的部分，避免顺手重构无关代码
5. **处理异常**: 每处可能出错的地方都要有 try-catch 或相应的错误处理
6. **UI 联动**: 修改 ViewModel 属性时，确保正确调用了 `OnPropertyChanged`

### 7.3 验证

1. **编译通过**: 修改完成后运行 `dotnet build`
2. **无警告**: 确保无编译警告（特别是 nullable 相关）
3. **不破坏现有功能**: 不修改已有接口签名（除非必要）
4. **没有死代码**: 不要保留注释掉的代码片段

### 7.4 文件操作

- **优先编辑已有文件**，除非确实需要新建文件
- 新建文件放在对应的目录下（Adapters → `Adapter/`，ViewModels → `ViewModels/`）
- 不要在 `.kilo/` 目录下修改或新建文件（这是 Kilo 自身配置区域）

### 7.5 XAML / Avalonia 注意事项

- 使用 `{CompiledBinding}` 绑定
- 控件样式遵循 Fluent 主题规范
- 新增控件需要对应的 `.axaml.cs` code-behind，逻辑尽量写回 ViewModel
- 事件处理使用 `RelayCommand` 而非 code-behind 内联逻辑

---

## 8. 常见陷阱

1. **Nullable 启用**: 引用类型默认可为 null，需正确处理 null 检查
2. **ChatRole 比较**: 使用 `ChatRole` 的 `.Value` 属性（字符串）比较角色，如 `p.Role.Value == "assistant"`
3. **ObservableStringBuilder**: 这是 LiveMarkdown.Avalonia 提供的流式更新机制，修改 Markdown 时需要通过 `MarkdownStringBuilder` 而非直接赋值来触发 UI 增量更新
4. **JSON 序列化**: 使用 `System.Text.Json`，非 `Newtonsoft.Json`；需要源码生成的场景使用 `SourceGenerationContext.cs`
5. **AOT 兼容**: 项目设置 `IsAotCompatible=true`，不能使用 `MakeGenericType` 等动态反射
6. **打包项目**: `DesktopAIShortcutPackaging/` 是独立的 Windows Application Packaging Project，仅用于 MSIX 发布，日常开发不需改它
7. **跨平台路径**: 使用 `Path.Combine` 而非硬编码 `\` 或 `/`，确保在 Windows/macOS/Linux 上均正确
8. **平台特定 API**: 避免使用 Windows 专有 API（如 Windows Forms、WinRT），Avalonia 提供跨平台替代方案
9. **文件存储路径**: 使用 `Environment.GetFolderPath(Environment.SpecialFolder.xxx)` 获取系统数据目录，该模式已在 `ChatHistoryService` 中使用
10. **条件编译**: 如需平台特定代码，使用 `#if WINDOWS` / `#if MACOS` / `#if LINUX` 预处理器指令隔离

---

> 最后更新: 2026-05-07 · 生成模型: deepseek-v4-pro
