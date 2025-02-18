using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using OverlappingModel;
using ReactiveUI;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
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

    private bool _hasStarted = false;
    private bool _isStarted = false;
    private bool _isInitializing = false;

    public bool CanReset => HasStarted && !IsInitializing;

    public bool HasStarted { get => _hasStarted; set => this.RaiseAndSetIfChanged(ref _hasStarted, value); }
    public bool IsStarted { get => _isStarted; set => this.RaiseAndSetIfChanged(ref _isStarted, value); }
    public bool IsInitializing { get => _isInitializing; set => this.RaiseAndSetIfChanged(ref _isInitializing, value); }

    private string _initializingText = "Initializing...";
    public string InitializingText { get => _initializingText; set => this.RaiseAndSetIfChanged(ref _initializingText, value); }

    Bitmap? _inputPreview;
    public Bitmap? InputPreview { get => _inputPreview; set => this.RaiseAndSetIfChanged(ref _inputPreview, value); }
    public bool IsImageChosen => InputPreview is not null;

    public ReactiveCommand<Window, Unit> ChooseImageCommand { get; }
    public ReactiveCommand<Unit, Unit> BeginCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    Bitmap? _outputPreview;
    public Bitmap? OutputPreview { get => _outputPreview; set => this.RaiseAndSetIfChanged(ref _outputPreview, value); }

    private WfcModelManager modelManager;

    private async Task Begin()
    {
        if (!HasStarted)
        {
            if (await CheckIfSettingsAreValid())
            {
                IsStarted = true;
                HasStarted = true;
                var wfcModel = await InitializeModel();
                modelManager = new WfcModelManager(wfcModel, UpdateOutputImage, () => {
                    IsStarted = false;
                    HasStarted = false;
                });
                modelManager.Speed = 10;
                modelManager.Start();
            }
        }
        else if (IsStarted)
        {
            modelManager!.Stop();
            IsStarted = false;
        }
        else
        {
            modelManager!.Start();
            IsStarted = true;
        }
    }

    private OverlappingModelBuilder SetUpBuilder()
    {
        var builder = new OverlappingModelBuilder();
        builder.Bitmap = ToSystemDrawingBitmap(InputPreview!);
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
        IsInitializing = true;
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

        IsInitializing = false;
        return await runningTask;
    }

    private void UpdateOutputImage(System.Drawing.Bitmap bitmap)
    {
        OutputPreview = ToAvaloniaBitmap(bitmap);
    }

    private global::Avalonia.Media.Imaging.Bitmap ToAvaloniaBitmap(System.Drawing.Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;
        return new global::Avalonia.Media.Imaging.Bitmap(stream);
    }

    private System.Drawing.Bitmap ToSystemDrawingBitmap(global::Avalonia.Media.Imaging.Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream);
        stream.Position = 0;
        return new System.Drawing.Bitmap(stream);
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
        IsStarted = false;
    }
}

