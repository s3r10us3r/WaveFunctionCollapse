using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using SimpleModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaveFunctionCollapse.Interfaces;
using SimpleModel.XMLModels;

namespace WaveFunctionCollapse
{
    /// <summary>
    /// Interaction logic for SimpleModelSettings.xaml
    /// </summary>
    public partial class SimpleModelSettings : Page
    {
        private SimpleModelBuilder builder;
        private string path;
        private int width;
        private int height;

        private string prevWidthText = "";
        private int prevWidthSelectionStart = 0;

        private string prevHeightText = "";
        private int prevHeightSelectionStart = 0;

        private string prevSeedText = "";
        private int prevSeedSelectionStart = 0;

        public SimpleModelSettings()
        {
            InitializeComponent();
            builder = new SimpleModelBuilder();
        }

        private async void StartWFC(object Sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            widthBox.IsEnabled = false;
            heightBox.IsEnabled = false;
            seedBox.IsEnabled = false;
            PickFIleButton.IsEnabled = false;
                
            VisualisationPage? page = await Task.Run(() =>
            {
                try
                {
                    IWaveFunctionCollapseModel model = builder.Build();
                    VisualisationPage newPage = null;
                    Dispatcher.Invoke(() => newPage = new VisualisationPage(model));
                    return newPage;
                }
                catch(Exception e) 
                {   
                    return null;
                }
            });
            if (page is null)
            {
                widthBox.IsEnabled = true;
                heightBox.IsEnabled = true;
                seedBox.IsEnabled = true;
                PickFIleButton.IsEnabled = true;
                path = null;
                builder.SetTilesetPath(null);
                ErrorMessage.Text = "Incorrect XMLFile, try a different one!";
            }
            else
            {
                NavigationService.Navigate(page);
            }
        }

        private void PickFileButtonClick(object Sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 1;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                builder.SetTilesetPath(selectedFilePath);
                PathTextBlock.Text = selectedFilePath;
                path = selectedFilePath;
            }
            CheckData();
        }

        private void WidthInputCheck(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            prevWidthText = textBox.Text;
            prevWidthSelectionStart = textBox.SelectionStart;
        }

        private void WidthTextCheck(object sender, TextChangedEventArgs e)
        {
            if (WidthAndHeightParse(widthBox.Text, out int result))
            {
                builder.SetWidth(result);
                width = result;
                CheckData();
                return;
            }

            widthBox.Text = prevWidthText;
            widthBox.SelectionStart = prevWidthSelectionStart;
            prevWidthText = "";
            prevWidthSelectionStart = 0;
        }
        private void HeightInputCheck(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            prevHeightText = textBox.Text;
            prevHeightSelectionStart = textBox.SelectionStart;
        }

        private void HeightTextCheck(object sender, TextChangedEventArgs e)
        {
            if (WidthAndHeightParse(heightBox.Text, out int result))
            {
                builder.SetHeight(result);
                height = result;
                CheckData();
                return;
            }

            heightBox.Text = prevHeightText;
            heightBox.SelectionStart = prevHeightSelectionStart;
            prevHeightText = "";
            prevHeightSelectionStart = 0;
        }

        private bool WidthAndHeightParse(string text, out int result)
        {
            result = 0;

            if (text.Length == 0)
            {
                return true;
            }

            if (text[0] == '0')
            {
                return false;
            }

            if (text.Length > 0 && text[0] == '0')
            {
                return false;
            }

            if (int.TryParse(text, out int num))
            {
                if (num >= 1 && num <= 9999)
                {
                    result = num;
                    return true;
                }
            }

            return false;
        }

        private void SeedInputCheck(object sender, TextCompositionEventArgs e)
        {
            prevSeedText = seedBox.Text;
            prevSeedSelectionStart = seedBox.SelectionStart;
        }
        private void SeedTextCheck(object sender, TextChangedEventArgs e)
        {
            int? result;
            if (SeedParse(seedBox.Text, out result))
            {
                builder.SetSeed(result);
                return;
            }

            seedBox.Text = prevSeedText;
            seedBox.SelectionStart = prevSeedSelectionStart;
            prevSeedText = "";
            prevSeedSelectionStart = 0;
        }

        private bool SeedParse(string text, out int? result)
        {
            result = null;
            if (text.Length == 0)
            {
                return true;
            }

            if (text.Length == 1 && text[0] == '-')
            {
                return true;
            }

            if (text[0] == '0' || (text.Length >= 2 && text[1] == '0'))
            {
                return false;
            }

            if (int.TryParse(text, out int num))
            {
                result = num;
                return true;
            }
                
            return false;
        }

        private void CheckData()
        {
            StartButton.IsEnabled = (path != null && width > 0 && height > 0);
        }
    }
}
