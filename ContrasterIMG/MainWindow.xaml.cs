using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContrasterIMG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        AppConfigManager config_manager = new AppConfigManager();

        public MainWindow()
        {
            InitializeComponent();
            Config_Load();
        }

        public void Config_Change(object sender, RoutedEventArgs e)
        {
            config_manager.UpdateConfigOutputDir(OutputPathTextBox.Text);

            if (sender is RadioButton button)
            {
                config_manager.UpdateConfigSettings(button.Name.ToString());
            }

            config_manager.SaveConfig();
        }

        private void Config_Load()
        {
            config_manager.LoadConfig();
            ////////////////////////////////////////////////////////////////////////
            if (config_manager.Config.Algorithm == AlgorithmSetting.Cpp)
            {
                AlgCpp.IsChecked = true;
            }
            else if (config_manager.Config.Algorithm == AlgorithmSetting.Assembly)
            {
                AlgAsm.IsChecked = true;
            }
            ////////////////////////////////////////////////////////////////////////
            if (config_manager.Config.NoThreads == NoThreadsSetting.Off)
            {
                ThOff.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th2)
            {
                Th2.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th4)
            {
                Th4.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th8)
            {
                Th8.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th16)
            {
                Th16.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th32)
            {
                Th32.IsChecked = true;
            }
            else if (config_manager.Config.NoThreads == NoThreadsSetting.Th64)
            {
                Th64.IsChecked = true;
            }
            ////////////////////////////////////////////////////////////////////////
            if (config_manager.Config.Time == TimeMeasureSetting.On)
            {
                TimeOn.IsChecked = true;
            }
            else if (config_manager.Config.Time == TimeMeasureSetting.Off)
            {
                TimeOff.IsChecked = true;
            }
            ////////////////////////////////////////////////////////////////////////
            if (config_manager.Config.Autosave == AutosaveSetting.On)
            {
                AutosaveOn.IsChecked = true;
            }
            else if (config_manager.Config.Autosave == AutosaveSetting.Off)
            {
                AutosaveOff.IsChecked = true;
            }
            ////////////////////////////////////////////////////////////////////////
            OutputPathTextBox.Text = config_manager.Config.OutputDirectory;
            ////////////////////////////////////////////////////////////////////////
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
                "ContrasterIMG\n\n" +
                "An application for changing image contrast.\n\n" +
                "The application allows you to:\n" +
                "• load images,\n" +
                "• change image contrast,\n" +
                "• compare the original and processed images,\n" +
                "• select the algorithm implementation (C++ / Assembly),\n" +
                "• use multithreading,\n" +
                "• measure the execution time of operations.\n\n" +
                "Project developed as part of the course\n" +
                "\"Assembly Languages\".\n\n" +
                "Version: 1.0",
                "About - ContrasterIMG",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                InputPathTextBox.Text = dialog.FileName;
            }
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog dialog = new Microsoft.Win32.OpenFolderDialog();

            if(dialog.ShowDialog() == true)
            {
                OutputPathTextBox.Text = dialog.FolderName;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InputPathTextBox.Text = string.Empty;
            InputImage.Source = null;
            OutputImage.Source = null;
            ApplyButton.IsEnabled = false;
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(
                "Credits\n\n" +
                "Application:\n" +
                "ContrasterIMG\n\n" +
                "Developed as part of the course\n" +
                "\"Assembly Languages\".\n\n" +
                "Technologies:\n" +
                "• C# / WPF\n" +
                "• C++\n" +
                "• x64 Assembly\n\n" +
                "Icons:\n" +
                "Icons made by Magnific from www.flaticon.com.\n\n" +
                "Thank you for using ContrasterIMG!",
                "Credits - ContrasterIMG",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void Slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            double new_value = Math.Round(ContrastSlider.Value, 2);
            SliderPositionBox.Text = new_value.ToString(CultureInfo.InvariantCulture);

            if(new_value != 0)
            {
                CenterButton.IsEnabled = true;
                CenterIcon.Opacity = 1;

                if(InputImage.Source != null)
                {
                    ApplyButton.IsEnabled = true;
                }
            }
        }

        private void Center_Click(object sender, RoutedEventArgs e)
        {
            CenterButton.IsEnabled = false;
            CenterIcon.Opacity = 0.2;
            ApplyButton.IsEnabled = false;
            ContrastSlider.Value = 0;
        }

        double SliderToContrast(double slider_pos)
        {
            if (slider_pos < 0)
            {
                return 1.0 + slider_pos / 100.0;
            }
            else
            {
                return 1.0 + slider_pos * 4.0 / 100.0;
            }
        }

        int AdjustUnits(int bytes, out String unit)
        {
            if(bytes < 1024)
            {
                unit = "B";
                return bytes;
            }
            else if(bytes < 1024 * 1024)
            {
                unit = "KB";
                return bytes / 1024;
            }
            else if (bytes < 1024 * 1024 * 1024)
            {
                unit = "MB";
                return bytes / (1024 * 1024);
            }
            else
            {
                unit = "GB";
                return bytes / (1024 * 1024 * 1024);
            }
                
        }

        void UpdateProgressIndicator(int progress, int pixels_size)
        {
            double progress_bar_value = (double)progress / (double)pixels_size * 100.0;
            int progress_percent = (int)progress_bar_value;
            ProgressIndicatorBar.Value = progress_bar_value;
            String unit = "";

            if(progress_bar_value != 100)
            {
                ProgressIndicatorString.Text = "status: working... (" + progress_percent + "%) [ " + AdjustUnits(progress, out unit) + " / " + AdjustUnits(pixels_size, out unit) + " " + unit + " ]";
            }
            else
            {
                ProgressIndicatorString.Text = "status: finished " + " (" + progress_percent + "%) [ " + AdjustUnits(progress, out unit) + " " + unit + " ]";
            }
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton.IsEnabled = false;

            if(InputImage.Source != null)
            {
                BitmapSource source_image = (BitmapSource)InputImage.Source;

                int stride = source_image.PixelWidth * 4;

                byte[] pixels = new byte[source_image.PixelHeight * stride];
                source_image.CopyPixels(pixels, stride, 0);

                int progress = 0;
                int width = source_image.PixelWidth;
                int height = source_image.PixelHeight;
                double contrast = SliderToContrast(ContrastSlider.Value);
                int pixels_size = stride * source_image.PixelHeight;

                Stopwatch stopwatch = new Stopwatch();

                ///////////////////////////////////////////////////////////////////////////////////////////

                Task algorithm_task = Task.Run(() =>
                {
                    if(config_manager.Config.Time == TimeMeasureSetting.On)
                    {
                        stopwatch.Restart();
                        AlgorithmCpp.ChangeContrast(pixels, width, height, 1, contrast, ref progress);
                        stopwatch.Stop();
                    }
                    else
                    {
                        AlgorithmCpp.ChangeContrast(pixels, width, height, 1, contrast, ref progress);
                    }
                });

                ///////////////////////////////////////////////////////////////////////////////////////////

                while (!algorithm_task.IsCompleted)
                {
                    await Task.Delay(128);
                    UpdateProgressIndicator(progress, pixels_size);
                }

                ///////////////////////////////////////////////////////////////////////////////////////////

                if (config_manager.Config.Time == TimeMeasureSetting.On)
                {
                    AlgorithmStopwatch.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
                }
                else
                {
                    AlgorithmStopwatch.Text = "N/A";
                }

                OutputImage.Source = ConvertToBitmap(pixels, source_image.PixelWidth, source_image.PixelHeight);
            }
        }

        private BitmapSource ConvertToBitmap(byte[] pixels, int width, int height)
        {
            int stride = width * 4; // 4 bajty na piksel w BGRA32

            BitmapSource bitmap = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                pixels,
                stride
            );

            return bitmap;
        }

        private void SliderPositionBox_TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string str = SliderPositionBox.Text;
                double new_value = 0;

                if (double.TryParse(str, CultureInfo.InvariantCulture, out new_value))
                {
                    ContrastSlider.Value = new_value;
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        "Invalid value.",
                        "ERROR - ContrasterIMG",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(InputPathTextBox.Text))
            {
                if(IsValidImage(InputPathTextBox.Text))
                {
                    InputImage.Source = new BitmapImage(new Uri(InputPathTextBox.Text));
                    UploadButton.IsEnabled = false;
                    Upload_Icon.Opacity = 0.2;
                    if(ContrastSlider.Value != 0)
                    {
                        ApplyButton.IsEnabled = true;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(
                    "Please select valid image file.",
                    "ERROR - ContrasterIMG",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                }
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "Cannot open file.",
                    "ERROR - ContrasterIMG",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(OutputPathTextBox.Text))
            {
                Process.Start("explorer.exe", OutputPathTextBox.Text);
            }
            else
            {
                System.Windows.MessageBox.Show(
                    "Cannot open directory.",
                    "ERROR - ContrasterIMG",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void InputPath_Changed(object sender, RoutedEventArgs e)
        {
            if(InputPathTextBox.Text != null && InputPathTextBox.Text != "" && System.IO.File.Exists(InputPathTextBox.Text))
            {
                UploadButton.IsEnabled = true;
                Upload_Icon.Opacity = 1;
            }
            else
            {
                UploadButton.IsEnabled = false;
                Upload_Icon.Opacity = 0.2;
            }
        }

        private void OutputPath_Changed(object sender, RoutedEventArgs e)
        {
            Config_Change(sender, e);

            if (OutputPathTextBox.Text != null && OutputPathTextBox.Text != "" && System.IO.Directory.Exists(OutputPathTextBox.Text))
            {
                OpenButton.IsEnabled = true;
                Open_Icon.Opacity = 1;
            }
            else
            {
                OpenButton.IsEnabled = false;
                Open_Icon.Opacity = 0.2;
            }
        }

        private bool IsValidImage(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return false;

            try
            {
                BitmapDecoder decoder = BitmapDecoder.Create(
                    new Uri(filePath, UriKind.Absolute),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad
                );

                return decoder.Frames.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}