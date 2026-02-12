using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DesktopAIShortcut.Models;
using DesktopAIShortcut.Services;
using OpenAI;
using OpenAI.Chat;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using DesktopAIShortcut.ViewModels;
using Microsoft.Extensions.AI;

namespace DesktopAIShortcut;

public partial class MainWindow : Window
{
    private ChatClient? client;
    private ChatHistoryService _historyService = new ChatHistoryService();
    private bool _isAutoSaving = false; // 添加标志位避免循环保存
     public MainWindow()
    {
        InitializeComponent();
        if (Design.IsDesignMode) return;
       

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
           )
        {
            this.Background = Brushes.Transparent;
        }
        
    }

   


    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        this.Hide();
    }

    private async void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && e.KeyModifiers==KeyModifiers.Control)
        {
               (this.DataContext  as ApplicationViewModel )?.ChatHistoryListViewModel?.CurrentChatHistoryViewModel? .AddUserMessageAsync();
        }
    }



    private void ModelList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox;
        if (cb == null)
        {
            return;
            
        }

        var value = cb.SelectedItem as ModelSelectionViewModel;
        if (value==null)
        {
            return;
        }

        var selectedmodel=ApplicationConfig.Instance.Providers.Where(p => p.ProviderId == value.SelectedProvider.ProviderId)
            .FirstOrDefault();
        if (selectedmodel == null)
        {
            return;
        }

        (this.DataContext  as ApplicationViewModel )?.ChatHistoryListViewModel?.CurrentChatHistoryViewModel.ProviderId = selectedmodel.ProviderId;
        (this.DataContext  as ApplicationViewModel )?.ChatHistoryListViewModel?.CurrentChatHistoryViewModel.ModelName = value.SelectedModel;

    }

    private void HistoryItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DeleteHistory_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}