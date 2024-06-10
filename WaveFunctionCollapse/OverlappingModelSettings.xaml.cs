using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OverlappingModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Diagnostics;
using WaveFunctionCollapse.Interfaces;

namespace WaveFunctionCollapse
{
    /// <summary>
    /// Interaction logic for OverlappingModelSettings.xaml
    /// </summary>
    public partial class OverlappingModelSettings : Page
    {
        private OverlappingModelBuilder builder;
        private string path;
        private int n;
        private int width;
        private int height;
        private int? seed;

        private string prevNText;
        private int prevNSelectionStart;

        private string prevWidthText;
        private int prevWidthSelectionStart;

        private string prevHeightText;
        private int prevHeightSelectionStart;

        private string prevSeedText;
        private int prevSeedSelectionStart;

        public OverlappingModelSettings()
        {
            InitializeComponent();
            builder = new();
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            PickFileButton.IsEnabled = false;
            NInput.IsEnabled = false;
            WidthInput.IsEnabled = false;
            HeightInput.IsEnabled = false;
            SeedTextBox.IsEnabled = false;

            if (seed is not null)
                builder.SetSeed((int)seed);

            builder.SetHeight(height);
            builder.SetN(n);
            builder.SetWidth(width);

            VisualisationPage? page = await Task.Run(() =>
            {
                IWaveFunctionCollapseModel model = builder.Build();
                VisualisationPage newPage = null;
                Dispatcher.Invoke(() =>
                {
                    newPage = new VisualisationPage(model);
                });
                return newPage;
            });

            if (page is null)
            {
                throw new InvalidOperationException("Failed to create VisualisationPage");
            }
            NavigationService.Navigate(page);
        }

        private void NTextPreview(object sender, TextCompositionEventArgs e)
        {
            prevNText = NInput.Text;
            prevNSelectionStart = NInput.SelectionStart;
        }

        private void WidthTextPreview(object sender, TextCompositionEventArgs e)
        {
            prevWidthText = WidthInput.Text;
            prevWidthSelectionStart = WidthInput.SelectionStart;
        }

        private void HeightTextPreview(object sender, TextCompositionEventArgs e)
        {
            prevHeightText = HeightInput.Text;
            prevHeightSelectionStart = HeightInput.SelectionStart;
        }

        private void SeedTextPreview(object sender, TextCompositionEventArgs e)
        {
            prevSeedText = SeedTextBox.Text;
            prevSeedSelectionStart = SeedTextBox.SelectionStart;
        }

        private void NTextChange(object sender, TextChangedEventArgs e)
        {
            if (NInput.Text.Length == 0)
            {
                n = 0;
                CheckData();
                return;
            }

            if (NInput.Text.Length <= 1 && int.TryParse(NInput.Text, out int num) && num > 0)
            {
                n = num;
                CheckData();
                return;
            }

            NInput.Text = prevNText;
            NInput.SelectionStart = prevNSelectionStart;
        }

        private void WidthTextChange(object sender, TextChangedEventArgs e)
        {
            if (WidthInput.Text.Length == 0)
            {
                width = 0;
                CheckData();
                return;
            }

            if (WidthInput.Text.Length >= 1 && WidthInput.Text[0] != '0' && int.TryParse(WidthInput.Text, out int num) && num >= 1 && num <= 9999)
            {
                width = num;
                CheckData();
                return;
            }

            WidthInput.Text = prevWidthText;
            WidthInput.SelectionStart = prevWidthSelectionStart;
        }


        private void HeightTextChange(object sender, TextChangedEventArgs e)
        {
            if (HeightInput.Text.Length == 0)
            {
                height = 0;
                CheckData();
                return;
            }

            if (HeightInput.Text.Length >= 1 && HeightInput.Text[0] != '0' && int.TryParse(HeightInput.Text, out int num) && num >= 1 && num <= 9999)
            {
                height = num;
                CheckData();
                return;
            }

            HeightInput.Text = prevHeightText;
            HeightInput.SelectionStart = prevHeightSelectionStart;
        }

        private void SeedTextChange(object sender, TextChangedEventArgs e)
        {
            CheckData();
            if (SeedTextBox.Text.Length == 0 || SeedTextBox.Text == "-")
            {
                seed = null;
                return;
            }

            if (SeedTextBox.Text[0] != '0' && int.TryParse(SeedTextBox.Text, out int num))
            {
                seed = num;
                return;
            }

            SeedTextBox.Text = prevSeedText;
            SeedTextBox.SelectionStart = prevSeedSelectionStart;
        }

        private void PickFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "PNG Files (*.png)|*.png";
            openFileDialog.FilterIndex = 1;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                Bitmap bitmap = new Bitmap(selectedFilePath);
                path = selectedFilePath;
                PathTextBlock.Text = selectedFilePath;
                builder.SetBitmap(bitmap);
            }
        }

        private void ReflectionChecked(object sender, RoutedEventArgs e)
        {
            builder.SetReflectionsEnabled(true);
        }

        private void ReflectionUnchecked(object sender, RoutedEventArgs e)
        {
            builder.SetReflectionsEnabled(false);
        }

        private void RotationChecked(object sender, RoutedEventArgs e)
        {
            builder.SetRotationsEnabled(true);
        }

        public void RotationUnchecked(object sender, RoutedEventArgs e)
        {
            builder.SetRotationsEnabled(false);
        }

        public void LockTopChecked(object sender, RoutedEventArgs e)
        {
            builder.LockTop(true);
        }

        public void LockTopUnchecked(object sender, RoutedEventArgs e)
        {
            builder.LockTop(false);
        }

        public void LockBottomChecked(object sender, RoutedEventArgs e)
        {
            builder.LockBottom(true);
        }

        public void LockBottomUnchecked(object sender, RoutedEventArgs e)
        {
            builder.LockBottom(false);
        }

        public void LockLeftChecked(object sender, RoutedEventArgs e)
        {
            builder.LockLeft(true);
        }

        public void LockLeftUnchecked(object sender, RoutedEventArgs e)
        {
            builder.LockLeft(false);
        }

        public void LockRightChecked(object sender, RoutedEventArgs e)
        {
            builder.LockRight(true);
        }

        public void LockRightUnchecked(object sender, RoutedEventArgs e)
        {
            builder.LockRight(false);
        }

        private void CheckData()
        {
            Debug.WriteLine($"{path} {width} {height} {n} {SeedTextBox.Text}");
            StartButton.IsEnabled = (path != null && width > 0 && height > 0 && n > 0 && SeedTextBox.Text != "-" && width >= n && height >= n);
        }
    }
}
