using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace DesktopAIShortcut;

public partial class ApplicationViewModel
{
    private readonly MainWindow _mainWindow;

    public ApplicationViewModel()
    {
        _mainWindow = new MainWindow
        {
            // DataContext = new MainWindowViewModel(),
        };

    }

    
    public void ShowWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow ??= _mainWindow;
        }

        _mainWindow.WindowState = WindowState.Normal;
        _mainWindow.Show();
        _mainWindow.BringIntoView();
        _mainWindow.Focus();
        _mainWindow.NewChat();
        _mainWindow.Topmost = true;
        _mainWindow.Topmost = false;

    }

    
    public void HideWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow ??= _mainWindow;
        }

        _mainWindow.Hide();
    }

    public void SetWindowPosition(int windowX, int windowY)
    {
        _mainWindow.Position = new PixelPoint(windowX, windowY);
        _mainWindow.Height = 300;
        _mainWindow.Width = 500;
       _mainWindow.UpdateLayout();
    }
}