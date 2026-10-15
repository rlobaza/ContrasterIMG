using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ContrasterIMG
{

    public class ImageManager
    {
        private MainWindow window_;
        private AppConfigManager config_manager_;
        private AlgorithmManager algorithm_manager_;

        public BitmapSource? InputImage;
        public BitmapSource? PreviewImage;
        public BitmapSource? OutputImage;

        public ImageManager(MainWindow window, AppConfigManager config_manager, AlgorithmManager algorithm_manager)
        {
            window_ = window;
            config_manager_ = config_manager;
            algorithm_manager_ = algorithm_manager;
        }

        private BitmapSource ResizeImage(BitmapSource source_image, int target_width)
        {
            if (source_image.PixelWidth > target_width)
            {
                double scale = (double)target_width / source_image.PixelWidth;
                int target_height = (int)Math.Round(source_image.PixelHeight * scale);

                // Tworzymy wizualny element rysujący i skalujący obraz do małych wymiarów
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(source_image, new Rect(0, 0, target_width, target_height));
                }

                // Renderujemy to fizycznie do małej bitmapy w pamięci (raz!)
                RenderTargetBitmap resizedBitmap = new RenderTargetBitmap(
                    target_width,
                    target_height,
                    96,
                    96,
                    PixelFormats.Pbgra32
                );
                resizedBitmap.Render(drawingVisual);
                resizedBitmap.Freeze(); // Zamrażamy dla optymalizacji wątków w WPF

                return resizedBitmap;
            }

            return source_image;
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

        private BitmapImage LoadBitmapSafely(string file_path)
        {
            BitmapImage bitmap = new BitmapImage();

            // Otwieramy plik tylko na moment odczytu bajtów
            using (var stream = new System.IO.FileStream(file_path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Wczytaj wszystko do pamięci teraz
                bitmap.StreamSource = stream;                  // Pobierz ze strumienia, a nie z globalnego URI
                bitmap.EndInit();
            } // Tutaj strumień jest automatycznie zamykany i zwalniany

            bitmap.Freeze(); // Zamrażamy dla optymalizacji
            return bitmap;
        }

        public bool LoadInputImage()
        {
            String path = window_.InputPathTextBox.Text;

            if (System.IO.File.Exists(path))
            {
                if (IsValidImage(path))
                {
                    window_.UploadButton.IsEnabled = false;
                    window_.Upload_Icon.Opacity = 0.2;

                    InputImage = LoadBitmapSafely(path);

                    if (window_.ContrastSlider.Value != 0)
                    {
                        window_.ApplyButton.IsEnabled = true;
                    }

                    UpdateInputImage();

                    LoadPreviewImage();
                    UpdatePreviewImage();

                    return true;
                }
                else
                {
                    System.Windows.MessageBox.Show(
                    "Please select valid image file.",
                    "ERROR - ContrasterIMG",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                    return false;
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

                return false;
            }
        }

        public void LoadPreviewImage()
        {
            if(InputImage != null)
            {
                PreviewImage = ResizeImage(InputImage, (int)config_manager_.Config.PreviewQuality);
            }
        }

        public async void CreateOutputImage()
        {
            if(InputImage != null)
            {
                OutputImage = await algorithm_manager_.ChangeContrast(InputImage, true);
                UpdatePreviewImage();
            }
        }

        public void UpdateInputImage()
        {
            if(InputImage != null)
            {
                window_.InputImage.Source = InputImage;
            }
        }

        public async void UpdatePreviewImage()
        {
            if(OutputImage != null)
            {
                window_.OutputImage.Source = OutputImage;
            }
            else
            {
                if (PreviewImage != null)
                {
                    window_.OutputImage.Source = await algorithm_manager_.ChangeContrast(PreviewImage, false);
                }
            }
        }

        public void DiscardOutputImage()
        {
            OutputImage = null;
        }

        public void Clear()
        {
            window_.InputImage.Source = null;
            window_.OutputImage.Source = null;
            InputImage = null;
            PreviewImage = null;
            OutputImage = null;
        }
    }
}
