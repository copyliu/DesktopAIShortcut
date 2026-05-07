using System;
using System.Text.Encodings.Web;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.ViewModels;

namespace DesktopAIShortcut
{
    public partial class App : Application
    {
        private   MainWindow _mainWindow;
        public const int DefaultWidth = 900;
        public const int DefaultHeight = 650;
        public override void Initialize()
        {
           ApplicationConfig.LoadSettings();
            AvaloniaXamlLoader.Load(this);
            _mainWindow = new MainWindow
            {
                SizeToContent = SizeToContent.Manual,
                CanResize = true,
                Width = DefaultWidth,
                Height = DefaultHeight,
                WindowStartupLocation = WindowStartupLocation.Manual,
                DataContext = new ApplicationViewModel()
            };
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
               
            }
            base.OnFrameworkInitializationCompleted();
        }

        private void Exit_OnClick(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private void TrayIcon_OnClicked(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _mainWindow;
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.BringIntoView();
                _mainWindow.Focus();

                // 确保窗口在最前面
                _mainWindow.Topmost = true;
                _mainWindow.Topmost = false;

#if WINDOWS
                var screen = desktop.MainWindow.Screens.Primary;
                var scaling = screen.Scaling;  // 使用新的 Scaling 属性
                var p = System.Windows.Forms.Cursor.Position;

                // 考虑缩放后的窗口实际尺寸
                // double scaledWidth = 500 * scaling;
                // double scaledHeight = 300 * scaling;


                // 获取窗口的实际像素尺寸
                var windowBounds = desktop.MainWindow.Bounds;
                
                double actualWidth = windowBounds.Width * scaling;
                double actualHeight = windowBounds.Height * scaling;

                if (p.Y > screen.WorkingArea.Y)
                {
                    //任务栏在底部
                    double windowY = (screen.WorkingArea.Bottom - actualHeight);  // 
                    //double windowX = p.X - (scaledWidth / 2);  // 居中显示
                    double windowX = p.X * scaling;

                    // 确保窗口不会超出屏幕右边界
                    if ((windowX + actualWidth) > (screen.WorkingArea.Right))
                    {
                        windowX = (screen.WorkingArea.Right - actualWidth);
                    }

                    // 确保窗口不会超出屏幕左边界
                    if (windowX < screen.WorkingArea.X)
                    {
                        windowX = screen.WorkingArea.X + 10;
                    }

                    SetWindowPosition((int)(windowX), (int)(windowY));
                }
                else
                {
                    //任务栏在顶部
                    double windowY = screen.WorkingArea.Y + 10;  // 添加10像素的边距
                    double windowX = p.X - (actualWidth / 2);  // 居中显示

                    // 确保窗口不会超出屏幕右边界
                    if (windowX + actualWidth > screen.WorkingArea.Right)
                    {
                        windowX = screen.WorkingArea.Right - actualWidth - 10;
                    }

                    // 确保窗口不会超出屏幕左边界
                    if (windowX < screen.WorkingArea.X)
                    {
                        windowX = screen.WorkingArea.X + 10;
                    }

                    SetWindowPosition((int)(windowX), (int)(windowY));
                }
#endif
            }
        }

        public void HideWindow()
        {
            _mainWindow.Hide();
        }

        public void SetWindowPosition(int windowX, int windowY)
        {
            // _mainWindow.Width = DefaultWidth;
            // _mainWindow.Height = DefaultHeight;

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

        public event EventHandler? SettingWindowClosed;

        private void Setting_clicked(object? sender, EventArgs e)
        {
            var settingWindow = new SettingWindow();
            settingWindow.Show();
            settingWindow.Closed += (s, args) =>
            {
                SettingWindowClosed?.Invoke(this, EventArgs.Empty);
                ApplicationConfig.LoadSettings();
                
            };
        }
    }
}