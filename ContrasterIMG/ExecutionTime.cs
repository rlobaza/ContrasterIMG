using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ContrasterIMG
{
    public class ExecutionTimeMeasurer
    {
        private MainWindow window_;
        private Stopwatch stopwatch = new Stopwatch();

        public int NoMeasurements;
        private int?[] measurements_;
        private int next_index = 0;
        private String elapsed_time_string = "N/A";
        private int elapsed_time_ms = 0;

        public ExecutionTimeMeasurer(MainWindow window, int no_measurements)
        {
            window_ = window;
            measurements_ = new int?[Enum.GetValues<AlgorithmSetting>().Length * Enum.GetValues<NoThreadsSetting>().Length * no_measurements];
            NoMeasurements = no_measurements;
            ClearMeasurements();
        }

        public void Restart()
        {
            elapsed_time_string = "N/A";
            stopwatch.Restart();
        }

        public void Stop()
        {
            stopwatch.Stop();
            elapsed_time_ms = (int)stopwatch.ElapsedMilliseconds;
            elapsed_time_string = stopwatch.ElapsedMilliseconds.ToString() + " ms";
        }

        public void Post()
        {
            if (!window_.Dispatcher.CheckAccess())
            {
                window_.Dispatcher.Invoke(() => Post());
                return;
            }

            window_.AlgorithmStopwatch.Text = elapsed_time_string;
        }

        public void SaveMeasurement()
        {
            if(next_index < measurements_.Count())
            {
                measurements_[next_index] = elapsed_time_ms;
                next_index++;
            }
        }

        public void ClearMeasurements()
        {
            for(int i = 0; i < measurements_.Count(); i++)
            {
                measurements_[i] = null;
            }
            next_index = 0;
        }

        //public void ExportMeasurements()
        //{
        //    String output = "";
        //    int i = 0;

        //    foreach (AlgorithmSetting algorithm in Enum.GetValues<AlgorithmSetting>())
        //    {
        //        foreach (NoThreadsSetting no_threads in Enum.GetValues<NoThreadsSetting>())
        //        {
        //            output += "alg: " + algorithm + ", threads: " + no_threads + " [ ";

        //            for (int j = i; j < i + NoMeasurements; j++)
        //            {
        //                output += measurements_[j].ToString() + " ";
        //            }

        //            output += " ]\n";

        //            i += NoMeasurements;
        //        }
        //    }

        //    System.Windows.MessageBox.Show(
        //        output,
        //        "Output",
        //        MessageBoxButton.OK,
        //        MessageBoxImage.Information
        //    );
        //}

        public void ExportMeasurements()
        {
            String output = "execution time measurement\n";

            output += ";;;;";
            for (int k = 0; k < NoMeasurements; k++)
            {
                output += "sample" + k + ";";
            }
            output += "\n";

            int i = 0;

            foreach (AlgorithmSetting algorithm in Enum.GetValues<AlgorithmSetting>())
            {
                foreach (NoThreadsSetting no_threads in Enum.GetValues<NoThreadsSetting>())
                {
                    output += "alg:;" + algorithm + ";threads:;" + no_threads;
                    for (int j = i; j < i + NoMeasurements; j++)
                    {
                        output += ";" + measurements_[j].ToString();
                    }

                    output += "\n";

                    i += NoMeasurements;
                }
            }

            // --- DODANA CZĘŚĆ: Zapis do pliku ---
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "measurements_report"; // Domyślna nazwa pliku
            dialog.DefaultExt = ".csv";             // Domyślne rozszerzenie
            dialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Jeśli użytkownik kliknie "Zapisz"
            if (dialog.ShowDialog() == true)
            {
                // Zapisujemy cały tekst do wybranego pliku (kodowanie UTF-8 ładnie obsłuży znaki)
                System.IO.File.WriteAllText(dialog.FileName, output, Encoding.UTF8);

                System.Windows.MessageBox.Show(
                    "Measurements exported successfully!",
                    "Success - ContrasterIMG",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
