using System;
using System.Text.Encodings.Web;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;

namespace DesktopAIShortcut
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private ApplicationViewModel context;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                context = new ApplicationViewModel();
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
                context?.ShowWindow();

#if WINDOWS
                var screen = desktop.MainWindow.Screens.Primary;
                var scaling = screen.Scaling;  // 使用新的 Scaling 属性
                var p = System.Windows.Forms.Cursor.Position;

                // 考虑缩放后的窗口实际尺寸
                double scaledWidth = 500 * scaling;
                double scaledHeight = 300 * scaling;

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

                    context?.SetWindowPosition((int)(windowX), (int)(windowY));
                }
                else
                {
                    //任务栏在顶部
                    double windowY = screen.WorkingArea.Y + 10;  // 添加10像素的边距
                    double windowX = p.X - (actualWidth / 2);  // 居中显示

                    // 确保窗口不会超出屏幕右边界
                    if (windowX + scaledWidth > screen.WorkingArea.Right)
                    {
                        windowX = screen.WorkingArea.Right - actualWidth - 10;
                    }

                    // 确保窗口不会超出屏幕左边界
                    if (windowX < screen.WorkingArea.X)
                    {
                        windowX = screen.WorkingArea.X + 10;
                    }

                    context?.SetWindowPosition((int)(windowX), (int)(windowY));
                }
#endif
            }
        }

        private void Setting_clicked(object? sender, EventArgs e)
        {
            var settingWindow = new SettingWindow();
            settingWindow.Show();
        }
    }
}