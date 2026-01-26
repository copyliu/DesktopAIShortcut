using System;
using System.Windows.Input;

namespace DesktopAIShortcut.Commands
{
    

    public static class CommandManager
    {
        internal static event EventHandler RequerySuggested;

        public static void AddRequerySuggestedHandler(EventHandler handler)
        {
            RequerySuggested += handler;
        }

        public static void RemoveRequerySuggestedHandler(EventHandler handler)
        {
            RequerySuggested -= handler;
        }

        public static void InvalidateRequerySuggested()
        {
            RequerySuggested?.Invoke(null, EventArgs.Empty);
        }
    }
}
