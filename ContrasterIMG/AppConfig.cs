using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

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

    public class AppConfig
    {
        public AlgorithmSetting Algorithm { get; set; } = AlgorithmSetting.Cpp;
        public NoThreadsSetting NoThreads { get; set; } = NoThreadsSetting.Off;
        public TimeMeasureSetting Time { get; set; } = TimeMeasureSetting.Off;
        public AutosaveSetting Autosave { get; set; } = AutosaveSetting.Off;

        public string OutputDirectory { get; set; } = "";
    };

    public class AppConfigManager
    {
        public AppConfig Config = new AppConfig();

        private JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public AppConfigManager()
        {
            options.Converters.Add(new JsonStringEnumConverter());
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
        }

        public void UpdateConfigOutputDir(string new_dir)
        {
            Config.OutputDirectory = new_dir;
        }
    };
}