using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ContrasterIMG
{
    public class AlgorithmManager
    {
        private MainWindow window_;
        private AppConfigManager config_manager_;

        private ExecutionTimeMeasurer time_measurer_;

        public AlgorithmManager(MainWindow window, AppConfigManager config_manager, ExecutionTimeMeasurer time_measurer)
        {
            window_ = window;
            config_manager_ = config_manager;
            time_measurer_ = time_measurer;
        }

        [DllImport("ContrasterIMG.Algorithm.Cpp.dll", EntryPoint = "changeContrast", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ChangeContrastCpp([In, Out] byte[] pixels, int img_width, int img_height, double contrast, ref int progress, int start_row, int end_row);

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

        public async Task<BitmapSource> ChangeContrast(BitmapSource source_image, bool generate_info, double contrast_slider_value)
        {
            int stride = source_image.PixelWidth * 4;

            byte[] pixels = new byte[source_image.PixelHeight * stride];
            source_image.CopyPixels(pixels, stride, 0);

            int width = source_image.PixelWidth;
            int height = source_image.PixelHeight;
            double contrast = SliderToContrast(contrast_slider_value);
            int pixels_size = stride * source_image.PixelHeight;

            int progress = 0;
            int no_threads = (int)config_manager_.Config.NoThreads;
            int rows_per_thread = (int)(height / no_threads);
            int remainder = height % no_threads;
            int start_row = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////

            Thread[] threads = new Thread[no_threads];
            int[] local_progresses = new int[no_threads];

            time_measurer_.Restart();

            for (int i = 0; i < no_threads; i++)
            {
                int current_rows = rows_per_thread + (i < remainder ? 1 : 0);
                int end_row = start_row + current_rows - 1;

                local_progresses[i] = 0;

                int threadIndex = i;
                int sRow = start_row;
                int eRow = end_row;

                threads[i] = new Thread(() =>
                {
                    ChangeContrastCpp(pixels, width, height, contrast, ref local_progresses[threadIndex], sRow, eRow);
                });

                threads[threadIndex].Start();

                start_row = eRow + 1;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////

            while (threads.Any(t => t.IsAlive))
            {
                await Task.Delay(1);

                if (generate_info)
                {
                    progress = 0;

                    for (int i = 0; i < no_threads; i++)
                    {
                        progress += local_progresses[i];
                    }

                    window_.UpdateProgressIndicator(progress, pixels_size);
                }
            }

            time_measurer_.Stop();

            if (generate_info)
            {
                window_.UpdateProgressIndicator(progress, pixels_size);
                time_measurer_.Post();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////

            return ConvertToBitmap(pixels, source_image.PixelWidth, source_image.PixelHeight);
        }
    }
}

    
