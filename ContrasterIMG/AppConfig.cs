using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ContrasterIMG
{
    public enum AlgorithmSetting
    {
        Cpp = 1,
        Assembly = 2
    }

    public enum NoThreadsSetting
    {
        Off = 1,
        Th2 = 2,
        Th4 = 4,
        Th8 = 8,
        Th16 = 16,
        Th32 = 32,
        Th64 = 64
    }

    public enum TimeMeasureSetting
    {
        Off = 0,
        On = 1
    }

    public enum AutosaveSetting
    {
        Off = 0,
        On = 1
    }

    public enum PreviewQualitySetting
    {
        Low = 128,
        Medium = 256,
        High = 512
    }

    public enum PreviewModeSetting
    {
        Pixelated = 0,
        Fast = 1,
        HQ = 2
    }

    public class AppConfig
    {
        public AlgorithmSetting Algorithm { get; set; } = AlgorithmSetting.Cpp;
        public NoThreadsSetting NoThreads { get; set; } = NoThreadsSetting.Off;
        public TimeMeasureSetting Time { get; set; } = TimeMeasureSetting.Off;
        public AutosaveSetting Autosave { get; set; } = AutosaveSetting.Off;
        public PreviewQualitySetting PreviewQuality { get; set; } = PreviewQualitySetting.Medium;
        public PreviewModeSetting PreviewMode { get; set; } = PreviewModeSetting.Pixelated;

        public string OutputDirectory { get; set; } = "";
    };

    public class AppConfigManager
    {
        MainWindow window_;

        public AppConfig Config = new AppConfig();

        private JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public AppConfigManager(MainWindow window)
        {
            options.Converters.Add(new JsonStringEnumConverter());
            window_ = window;
        }

        public void SaveConfig()
        {
            string local_app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string serialized = JsonSerializer.Serialize(Config, options);

            if(!System.IO.Directory.Exists(local_app_data_path + "\\ContrasterIMG"))
            {
                System.IO.Directory.CreateDirectory(local_app_data_path + "\\ContrasterIMG");
            }

            File.WriteAllText(local_app_data_path + "\\ContrasterIMG\\Config.json", serialized);
        }

        public void LoadConfig()
        {
            string local_app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (System.IO.File.Exists(local_app_data_path + "\\ContrasterIMG\\Config.json"))
            {
                AppConfig? new_config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(local_app_data_path + "\\ContrasterIMG\\Config.json"), options);

                if (new_config != null)
                {
                    Config = new_config;

                    if (Config.PreviewMode == PreviewModeSetting.Pixelated)
                    {
                        RenderOptions.SetBitmapScalingMode(window_.InputImage, BitmapScalingMode.NearestNeighbor);
                        RenderOptions.SetBitmapScalingMode(window_.OutputImage, BitmapScalingMode.NearestNeighbor);
                    }
                    if (Config.PreviewMode == PreviewModeSetting.Fast)
                    {
                        RenderOptions.SetBitmapScalingMode(window_.InputImage, BitmapScalingMode.LowQuality);
                        RenderOptions.SetBitmapScalingMode(window_.OutputImage, BitmapScalingMode.LowQuality);
                    }
                    if (Config.PreviewMode == PreviewModeSetting.HQ)
                    {
                        RenderOptions.SetBitmapScalingMode(window_.InputImage, BitmapScalingMode.HighQuality);
                        RenderOptions.SetBitmapScalingMode(window_.OutputImage, BitmapScalingMode.HighQuality);
                    }
                }
            }
        }

        public void UpdateConfigSettings(string sender)
        {
            if (sender == "AlgCpp")
            {
                Config.Algorithm = AlgorithmSetting.Cpp;
            }
            if (sender == "AlgAsm")
            {
                Config.Algorithm = AlgorithmSetting.Assembly;
            }
            if (sender == "ThOff")
            {
                Config.NoThreads = NoThreadsSetting.Off;
            }
            if (sender == "Th2")
            {
                Config.NoThreads = NoThreadsSetting.Th2;
            }
            if (sender == "Th4")
            {
                Config.NoThreads = NoThreadsSetting.Th4;
            }
            if (sender == "Th8")
            {
                Config.NoThreads = NoThreadsSetting.Th8;
            }
            if (sender == "Th16")
            {
                Config.NoThreads = NoThreadsSetting.Th16;
            }
            if (sender == "Th32")
            {
                Config.NoThreads = NoThreadsSetting.Th32;
            }
            if (sender == "Th64")
            {
                Config.NoThreads = NoThreadsSetting.Th64;
            }
            if (sender == "TimeOn")
            {
                Config.Time = TimeMeasureSetting.On;
            }
            if (sender == "TimeOff")
            {
                Config.Time = TimeMeasureSetting.Off;
            }
            if (sender == "AutosaveOn")
            {
                Config.Autosave = AutosaveSetting.On;
            }
            if (sender == "AutosaveOff")
            {
                Config.Autosave = AutosaveSetting.Off;
            }
            if (sender == "PreviewLQ")
            {
                Config.PreviewQuality = PreviewQualitySetting.Low;
            }
            if (sender == "PreviewMQ")
            {
                Config.PreviewQuality = PreviewQualitySetting.Medium;
            }
            if (sender == "PreviewHQ")
            {
                Config.PreviewQuality = PreviewQualitySetting.High;
            }
            if (sender == "PreviewModePixel")
            {
                Config.PreviewMode = PreviewModeSetting.Pixelated;
            }
            if (sender == "PreviewModeFast")
            {
                Config.PreviewMode = PreviewModeSetting.Fast;
            }
            if (sender == "PreviewModeBest")
            {
                Config.PreviewMode = PreviewModeSetting.HQ;
            }
        }

        public void UpdateConfigOutputDir(string new_dir)
        {
            Config.OutputDirectory = new_dir;
        }
    };
}