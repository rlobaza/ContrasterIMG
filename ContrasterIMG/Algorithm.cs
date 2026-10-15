using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ContrasterIMG
{
    public class AlgorithmManager
    {
        private MainWindow window_;
        private AppConfigManager config_manager_;

        public AlgorithmManager(MainWindow window, AppConfigManager config_manager)
        {
            window_ = window;
            config_manager_ = config_manager;
        }

        [DllImport("ContrasterIMG.Algorithm.Cpp.dll", EntryPoint = "changeContrast", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ChangeContrastCpp([In, Out] byte[] pixels, int img_width, int img_height, int no_threads, double contrast, ref int progress, int start_row, int end_row);

        private BitmapSource ConvertToBitmap(byte[] pixels, int width, int height)
        {
            int stride = width * 4;

            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);
            return bitmap;
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

        public async Task<BitmapSource> ChangeContrast(BitmapSource source_image, bool generate_info)
        {
            int stride = source_image.PixelWidth * 4;

            byte[] pixels = new byte[source_image.PixelHeight * stride];
            source_image.CopyPixels(pixels, stride, 0);

            int width = source_image.PixelWidth;
            int height = source_image.PixelHeight;
            double contrast = SliderToContrast(window_.ContrastSlider.Value);
            int pixels_size = stride * source_image.PixelHeight;

            Stopwatch stopwatch = new Stopwatch();

            int progress = 0;
            int no_threads = (int)config_manager_.Config.NoThreads;
            int rows_per_thread = (int)(height / no_threads);
            int remainder = height % no_threads;
            int start_row = 0;  

            ///////////////////////////////////////////////////////////////////////////////////////////

            List<Task> tasks = new List<Task>();
            int[] local_progresses = new int[no_threads];

            for(int i = 0; i < no_threads; i++)
            {
                if (start_row >= height) break;

                // Zabezpieczenie: jeśli wiersze się skończyły (np. liczba wątków > wysokość obrazka)
                if (start_row >= height) break;

                // Pierwsze 'remainder' wątków dostaje 1 dodatkowy wiersz, żeby pokryć resztę
                int current_rows = rows_per_thread + (i < remainder ? 1 : 0);
                int end_row = start_row + current_rows - 1; // Zakres inkluzywny dla C++

                local_progresses[i] = 0;

                // KLUCZOWE: Tworzymy lokalne kopie zmiennych dla lambdy. 
                // Dzięki temu każdy wątek dostanie swoje własne, niezmienne numery wierszy!
                int threadIndex = i;
                int sRow = start_row;
                int eRow = end_row;

                Task algorithm_task = Task.Run(() =>
                {
                    ChangeContrastCpp(pixels, width, height, 1, contrast, ref local_progresses[threadIndex], sRow, eRow);
                });

                tasks.Add(algorithm_task);

                start_row = eRow + 1;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            
            Task all_tasks = Task.WhenAll(tasks);

            stopwatch.Restart();

            while (!all_tasks.IsCompleted)
            {
                await Task.Delay(8);

                if (generate_info)
                {
                    progress = 0;

                    for (int i = 0; i < tasks.Count; i++)
                    {
                        progress += local_progresses[i];
                    }

                    window_.UpdateProgressIndicator(progress, pixels_size);
                }
            }

            stopwatch.Stop();

            if (generate_info)
            {
                window_.UpdateProgressIndicator(progress, pixels_size);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////

            if (config_manager_.Config.Time == TimeMeasureSetting.On && generate_info)
            {
                window_.AlgorithmStopwatch.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
            }
            else if (generate_info)
            {
                window_.AlgorithmStopwatch.Text = "N/A";
            }

            return ConvertToBitmap(pixels, source_image.PixelWidth, source_image.PixelHeight);
        }
    }
}

    
