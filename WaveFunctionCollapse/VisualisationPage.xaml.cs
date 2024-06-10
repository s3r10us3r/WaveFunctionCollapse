using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaveFunctionCollapse.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using WaveFunctionCollapse.Interfaces.Errors;

namespace WaveFunctionCollapse
{
    /// <summary>
    /// Interaction logic for VisualisationPage.xaml
    /// </summary>
    public partial class VisualisationPage : Page
    {
        private bool didFinish = false;

        private CancellationTokenSource tokenSource;
        private Thread runningThread;
        private bool isRunning = false;
        private int speed = 1;
        private IWaveFunctionCollapseModel model;
        private int allCollapses;
        private int newWidth;
        private int newHeight;

        public VisualisationPage(IWaveFunctionCollapseModel model)
        {
            InitializeComponent();

            Application.Current.Exit += OnApplicationExit;

            tokenSource = new CancellationTokenSource();
            runningThread = new Thread(() => RunCollapse(tokenSource.Token));
            this.model = model;
            allCollapses = model.CollapsesLeft;
            UpdatePercantageText();
            Bitmap bitmap = model.Image;
            double scalingFactor = Math.Min(ImagePane.Width / bitmap.Width, ImagePane.Height / bitmap.Height);
            newWidth = (int)(scalingFactor * bitmap.Width);
            newHeight = (int)(scalingFactor * bitmap.Height);
            ConvertBitmapToBitmapSource(bitmap);
        }

        private void CollapseOnce(object sender, RoutedEventArgs e)
        {
            CollapseNTimes(1);
        }

        private void Collapse10Times(object sender, RoutedEventArgs e)
        {
            CollapseNTimes(10);
        }
        
        private void Collapse100Times(object sender, RoutedEventArgs e)
        {
            CollapseNTimes(100);
        }

        private void ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);


                using (Graphics graphics = Graphics.FromImage(scaledBitmap))
                {

                    if (newWidth > bitmap.Width && newHeight > bitmap.Height)
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    }
                    else
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    }

                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                }

                scaledBitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                Dispatcher.Invoke(() => ImagePane.Source = bitmapImage);
            }
        }
        
        private void StartCollapsing1(object sender, RoutedEventArgs e)
        {
            SetButtonsLock(true);
            LockCollapseButttons(false);
            SaveButton.IsEnabled = false;
            Run1.IsEnabled = false; 
            StartCollapsing(1);
        }

        private void StartCollapsing2(object sender, RoutedEventArgs e)
        {
            SetButtonsLock(true);
            LockCollapseButttons(false);
            SaveButton.IsEnabled = false;
            Run2.IsEnabled = false;
            StartCollapsing(10);
        }

        private void StartCollapsing3(object sender, RoutedEventArgs e)
        {

            SetButtonsLock(true);
            LockCollapseButttons(false);
            SaveButton.IsEnabled = false;
            Run3.IsEnabled = false;
            StartCollapsing(100);
        }

        private void StartCollapsing(int newSpeed)
        {
            speed = newSpeed;
            if (!isRunning)
            {
                isRunning = true;
                tokenSource = new();
                CancellationToken token = tokenSource.Token;
                runningThread = new Thread(() => RunCollapse(token));
                runningThread.Start();
            }
        }

        private void StopCollapsing(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            SetButtonsLock(true);
            StopStart.IsEnabled = false;
        }

        private async void Reset(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            didFinish = false;
            ErrorTextBox.Text = "";
            SetButtonsLock(false);
            await Task.Run(() => {
                if (runningThread.ThreadState == System.Threading.ThreadState.Running)
                    runningThread.Join();
                model.Reset();
            });
            ConvertBitmapToBitmapSource(model.Image);
            SetButtonsLock(true);
            UpdatePercantageText();
        }


        private void RunCollapse(CancellationToken token)
        {
            try
            {
                while (isRunning && !didFinish && !token.IsCancellationRequested)
                {
                    try
                    {
                        model.CollapseNTimes(speed);
                        Dispatcher.Invoke(() =>
                        {
                            ConvertBitmapToBitmapSource(model.Image);
                            UpdatePercantageText();
                        });
                    }
                    catch (NoPossibleCollapseException e)
                    {
                        Dispatcher.Invoke(HandleContradiction);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                isRunning = false;
            }
        }

        private async void CollapseNTimes(int n)
        {
            SetButtonsLock(false);
            await Task.Run(() =>
            {
                try
                {
                    model.CollapseNTimes(n);
                }
                catch (NoPossibleCollapseException e)
                {
                    HandleContradiction();
                }
            });
            UpdatePercantageText();
            ConvertBitmapToBitmapSource(model.Image);
            SetButtonsLock(true);
        }

        private void SetButtonsLock(bool lockState)
        {
            Collapse1.IsEnabled = lockState;
            Collapse10.IsEnabled = lockState;
            Collapse100.IsEnabled = lockState;
            StopStart.IsEnabled = lockState;
            Run1.IsEnabled = lockState;
            Run2.IsEnabled = lockState;
            Run3.IsEnabled = lockState;
            ResetButton.IsEnabled = lockState;
            SaveButton.IsEnabled = lockState;
        }

        private void LockCollapseButttons(bool lockState)
        {
            Collapse1.IsEnabled = lockState;
            Collapse10.IsEnabled = lockState;
            Collapse100.IsEnabled = lockState;
        }

        private void UpdatePercantageText()
        {
            int percantege = (int)((allCollapses - model.CollapsesLeft) / (double)allCollapses * 100);
            PercantageTextBox.Text = $"{percantege}%";
            if (model.CollapsesLeft == 0)
            {
                didFinish = true;
                SetButtonsLock(false);
                SaveButton.IsEnabled = true;
                ResetButton.IsEnabled = true;
            }
        }

        private void HandleContradiction()
        {
            isRunning = false;
            didFinish = false;
            SetButtonsLock(false);
            ResetButton.IsEnabled = true;
            ErrorTextBox.Text = "Algorithm ran into a contradiction! Try resetting.";
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            isRunning = false;
            tokenSource?.Cancel();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    SaveCurrentImage(filename);
                }
            });
        }

        private void SaveCurrentImage(string filename)
        {
            Bitmap bitmap = model.Image;
            bitmap.Save(filename);
        }
    }
}
