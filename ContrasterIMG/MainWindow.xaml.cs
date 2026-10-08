using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContrasterIMG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                InputImage.Source = new BitmapImage(new Uri(dialog.FileName));
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

        private void InputPath_Changed(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(InputPathTextBox.Text))
            {
                InputImage.Source = new BitmapImage(new Uri(InputPathTextBox.Text));
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InputPathTextBox.Text = string.Empty;
            InputImage.Source = null;
            OutputImage.Source = null;
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
    }
}