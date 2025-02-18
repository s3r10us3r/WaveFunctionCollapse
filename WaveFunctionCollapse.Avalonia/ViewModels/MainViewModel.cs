using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using OverlappingModel;
using ReactiveUI;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using SkiaSharp;
using WaveFunctionCollapse.Avalonia.Helpers;
using WaveFunctionCollapse.Interfaces;

namespace WaveFunctionCollapse.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        ChooseImageCommand = ReactiveCommand.CreateFromTask<Window>(ChooseImage);
        BeginCommand = ReactiveCommand.CreateFromTask(Begin);
        ResetCommand = ReactiveCommand.Create(ResetImage);
        SaveImageCommand = ReactiveCommand.CreateFromTask<Window>(SaveImage);
    }

    //Settings
    private decimal _n = 2;
    private decimal? _seed;
    private decimal _width;
    private decimal _height;
    private bool _rotationsEnabled;
    private bool _reflectionsEnabled;
    private bool _lockTop;
    private bool _lockBottom;
    private bool _lockLeft;
    private bool _lockRight;
    public decimal? N { get => _n; set
        {
            var val = value ?? 2;
            if (val >= 2)
            {
                this.RaiseAndSetIfChanged(ref _n, val); 
            } else
            {
                this.RaiseAndSetIfChanged(ref _n, 2); 
            }
        }
    }
    public decimal? Seed { get => _seed; set => this.RaiseAndSetIfChanged(ref _seed, value); }
    public decimal? Width { get => _width; set => this.RaiseAndSetIfChanged(ref _width, value ?? 0); }
    public decimal? Height { get => _height; set => this.RaiseAndSetIfChanged(ref _height, value ?? 0); }
    public bool RotationsEnabled { get => _rotationsEnabled; set => this.RaiseAndSetIfChanged(ref _rotationsEnabled, value); }
    public bool ReflectionsEnabled { get => _reflectionsEnabled; set => this.RaiseAndSetIfChanged(ref _reflectionsEnabled, value); }
    public bool LockTop { get => _lockTop; set => this.RaiseAndSetIfChanged(ref _lockTop, value); }
    public bool LockBottom { get => _lockBottom; set => this.RaiseAndSetIfChanged(ref _lockBottom, value); }
    public bool LockLeft { get => _lockLeft; set => this.RaiseAndSetIfChanged(ref _lockLeft, value); }
    public bool LockRight { get => _lockRight; set => this.RaiseAndSetIfChanged(ref _lockRight, value); }

    private State _state = State.NotStarted;

    private State RunningState
    {
        get => _state;
        set
        {
            this.RaiseAndSetIfChanged(ref _state, value);
            this.RaisePropertyChanged(nameof(CanReset));
            this.RaisePropertyChanged(nameof(HasStarted));
            this.RaisePropertyChanged(nameof(IsInitializing));
            this.RaisePropertyChanged(nameof(IsStarted));
            this.RaisePropertyChanged(nameof(HasFinished));
        }
    }
    public bool CanReset => HasStarted && !IsInitializing && !IsStarted;
    public bool HasStarted => RunningState is State.HasStartedNotRunning or State.IsRunning;
    public bool IsStarted => RunningState is State.IsRunning;
    public bool IsInitializing => RunningState is State.Initializing;
    public bool HasFinished => RunningState is State.HasFinished;
    
    
    private string _initializingText = "Initializing...";
    public string InitializingText { get => _initializingText; set => this.RaiseAndSetIfChanged(ref _initializingText, value); }

    private Bitmap? _inputPreview;
    public Bitmap? InputPreview { get => _inputPreview; set => this.RaiseAndSetIfChanged(ref _inputPreview, value); }
    public bool IsImageChosen => InputPreview is not null;

    public ReactiveCommand<Window, Unit> ChooseImageCommand { get; }
    public ReactiveCommand<Unit, Unit> BeginCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }
    public ReactiveCommand<Window, Unit> SaveImageCommand { get; }

    Bitmap? _outputPreview;
    public Bitmap? OutputPreview { get => _outputPreview; set => this.RaiseAndSetIfChanged(ref _outputPreview, value); }

    private WfcModelManager modelManager;

    private async Task Begin()
    {
        if (!HasStarted)
        {
            if (await CheckIfSettingsAreValid())
            {
                RunningState = State.Initializing;
                var wfcModel = await InitializeModel();
                modelManager = new WfcModelManager(wfcModel, UpdateOutputImage, (bool didSucceed) =>
                {
                    if (!didSucceed)
                        DisplayDialog();
                    RunningState = State.HasFinished;
                });
                modelManager.Speed = 10;
                modelManager.Start();
                RunningState = State.IsRunning;
            }
        }
        else if (RunningState is State.IsRunning)
        {
            modelManager!.Stop();
            RunningState = State.HasStartedNotRunning;
        }
        else
        {
            modelManager!.Start();
            RunningState = State.IsRunning;
        }
    }

    private async Task DisplayDialog()
    {
        var messageBox = MessageBoxManager
            .GetMessageBoxStandard("Error", "The algorithm ran into a contradiction so it can not proceed further", 
                ButtonEnum.Ok);

        await messageBox.ShowAsync();
    }
    
    private OverlappingModelBuilder SetUpBuilder()
    {
        var builder = new OverlappingModelBuilder();
        builder.Bitmap = ToSkiaSharpBitmap(InputPreview!);
        builder.N = (int)_n;
        builder.Width = (int)Width!;
        builder.Height = (int)Height!;
        builder.RotationsEnabled = RotationsEnabled;
        builder.ReflectionsEnabled = ReflectionsEnabled;
        builder.LockTop = LockTop;
        builder.LockBottom = LockBottom;
        builder.LockLeft = LockLeft;
        builder.LockRight = LockRight;
        return builder;
    }

    private async Task<IWaveFunctionCollapseModel> InitializeModel()
    {
        RunningState = State.Initializing;
        var builder = SetUpBuilder();
        var runningTask = Task.Run(() => builder.Build());
        int dots = 0;

        while (!runningTask.IsCompleted)
        {
            dots += 1;
            dots %= 4;
            InitializingText = "Initializing" + new string('.', dots);
            await Task.Delay(300);
        }
        return await runningTask;
    }

    private void UpdateOutputImage(SKBitmap bitmap)
    {
        OutputPreview = ToAvaloniaBitmap(bitmap);
    }

    private Bitmap ToAvaloniaBitmap(SKBitmap bitmap)
    {
        using var stream = new MemoryStream();
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(stream);
        stream.Position = 0;
        return new Bitmap(stream);
    }

    private SKBitmap ToSkiaSharpBitmap(Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream);
        stream.Position = 0;
        var skBitmap = SKBitmap.Decode(stream);
        return skBitmap;
    }

    private async Task<bool> CheckIfSettingsAreValid()
    {
        if (N != null && N <= 1)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "N must be greater than 1").ShowAsync();
            return false;
        }

        if (_inputPreview is null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "Please choose an image").ShowAsync();
            return false;
        }

        if (Width < N || Height < N)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", "Width and Height must be greater or equal to N").ShowAsync();
            return false;
        }

        return true;
    }

    private async Task ChooseImage(Window parentWindow)
    {
        if (IsStarted)
        {
            return;
        }

        var options = new FilePickerOpenOptions
        {
            Title = "Choose an image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Image Files")
                {
                    Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"],
                    MimeTypes = ["image/png", "image/jpeg", "image/bmp"]
                }
            ]
        };

        var result = await parentWindow.StorageProvider.OpenFilePickerAsync(options);
        
        if (result.Count > 0)
        {
            var selectedFile = result[0];
            await SetInputImage(selectedFile);
        }
    }

    public async Task SetInputImage(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        var bitmap = new Bitmap(stream);
        InputPreview = bitmap;
        this.RaisePropertyChanged(nameof(IsImageChosen));
    }

    private void ResetImage()
    {
        modelManager?.Reset();
        RunningState = State.HasStartedNotRunning;
    }

    private async Task SaveImage(Window parentWindow)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "Save Image",
            InitialFileName = "wfc.png",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "PNG", Extensions = new List<string> { "png" } },
                new FileDialogFilter { Name = "JPEG", Extensions = new List<string> { "jpg", "jpeg" } },
                new FileDialogFilter { Name = "Bitmap", Extensions = new List<string> { "bmp" } }
            }
        };

        var result = await saveFileDialog.ShowAsync(parentWindow);
        if (!string.IsNullOrEmpty(result) && OutputPreview != null)
        {
            OutputPreview.Save(result);
        }
    }

    private enum State
    {
        NotStarted, Initializing, HasStartedNotRunning, IsRunning, HasFinished
    }
}

