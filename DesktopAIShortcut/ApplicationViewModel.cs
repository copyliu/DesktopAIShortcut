using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DesktopAIShortcut;

public partial class ApplicationViewModel
{
    private readonly MainWindow _mainWindow;
    private const int DefaultWidth = 500;
    private const int DefaultHeight = 300;

    public ApplicationViewModel()
    {
        _mainWindow = new MainWindow
        {
            SizeToContent = SizeToContent.Manual,
            CanResize = false,
            Width = DefaultWidth,
            Height = DefaultHeight,
            WindowStartupLocation = WindowStartupLocation.Manual
        };
    }

    public void ShowWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _mainWindow;
        }

        _mainWindow.Show();
        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.BringIntoView();
        _mainWindow.Focus();
        _mainWindow.NewChat();

        // 确保窗口在最前面
        _mainWindow.Topmost = true;
        _mainWindow.Topmost = false;
    }

    public void HideWindow()
    {
        _mainWindow.Hide();
    }

    public void SetWindowPosition(int windowX, int windowY)
    {
        _mainWindow.Width = DefaultWidth;
        _mainWindow.Height = DefaultHeight;

        // 确保在设置位置之前窗口已经初始化但未显示
        if (!_mainWindow.IsVisible)
        {
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Position = new PixelPoint(windowX, windowY);
        }
        else
        {
            _mainWindow.Position = new PixelPoint(windowX, windowY);
        }
    }
}