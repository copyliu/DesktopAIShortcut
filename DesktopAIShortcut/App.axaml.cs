using System;
using System.Text.Encodings.Web;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace DesktopAIShortcut;

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
            desktop.ShutdownMode =ShutdownMode.OnExplicitShutdown;
            ;
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
            Avalonia.Platform.Screen screen = desktop.MainWindow.Screens.Primary;
            
            var p = System.Windows.Forms.Cursor.Position;
            if (p.Y > screen.WorkingArea.Y)
            {
                //任务栏在底部
                var windowY = screen.WorkingArea.Bottom - 300;
                var windowX = p.X - 150;
               
                if (windowX + 500 > screen.WorkingArea.Right)
                {
                    windowX -= windowX + 500 - screen.WorkingArea.Right;
                }
                if (windowX < 0)
                {
                    windowX+=-windowX;
                }
                context?.SetWindowPosition(windowX, windowY);
            }
            else
            {
                //任务栏在顶部
                var windowY = screen.WorkingArea.Y ;
                var windowX = p.X - 150;
                if (windowX + 500 > screen.WorkingArea.Right)
                {
                    windowX -= windowX + 500 - screen.WorkingArea.X;
                }
                if (windowX < 0)
                {
                    windowX+=-windowX;
                }
                context?.SetWindowPosition(windowX, windowY);
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