using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System;
using System.Diagnostics;
using System.Linq;
using WaveFunctionCollapse.Avalonia.ViewModels;

namespace WaveFunctionCollapse.Avalonia.Views;

public partial class MainView : UserControl
{
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".bmp"];

    public MainView()
    {
        InitializeComponent();
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Debug.WriteLine("Choose image PRESSED");
        if (DataContext is MainViewModel vm)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            if (parentWindow is null)
                Console.WriteLine("Could not choose image, parent window is null");
            else 
                vm.ChooseImageCommand.Execute(parentWindow);
        }
    }

    private void Border_DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles()?.ToList();
            if (files != null && files.Count != 0 && IsImageFile(files.First().Path.ToString()))
            {
                e.DragEffects = DragDropEffects.Copy;
                return;
            }
        }
        e.DragEffects = DragDropEffects.None;
    }

    private void Border_Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles()?.ToList();
            if (files != null && files.Count != 0)
            {
                var file = files.First();
                var filePath = file.Path.ToString();
                if (IsImageFile(filePath) && DataContext is MainViewModel vm && file != null)
                {
                    var notNullFile = file as IStorageFile;
                    vm.SetInputImage(notNullFile);
                }
            }
        }
    }

    private bool IsImageFile(string path) =>
        AllowedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
}
