using System;
using System.IO;

namespace ReportAutomation
{
    // Конфигурация правил проверки исходных данных.
    // Правила вынесены из кода и могут изменяться без его правки.
    // При наличии файла config.txt параметры читаются из него.
    public class AppConfig
    {
        // Минимально допустимый объём передачи электрической энергии.
        public double MinEnergyTransmitted { get; set; } = 0;

        // Максимально допустимая доля потерь, процент.
        // Записи с превышением помечаются как требующие контроля.
        public double MaxLossesShare { get; set; } = 25;

        // Каталог для сохранения сформированных документов.
        public string OutputDirectory { get; set; } =
            Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments), "Отчёты");

        // Читает параметры из файла конфигурации.
        // Если файл отсутствует, применяются значения по умолчанию.
        public static AppConfig Load()
        {
            AppConfig config = new AppConfig();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "config.txt");

            if (!File.Exists(path))
            {
                return config;
            }

            // Файл конфигурации содержит строки вида "ключ=значение".
            foreach (string line in File.ReadAllLines(path))
            {
                string current = line.Trim();
                if (current.Length == 0 || current.StartsWith("#"))
                {
                    continue;
                }

                int separator = current.IndexOf('=');
                if (separator <= 0)
                {
                    continue;
                }

                string key = current.Substring(0, separator).Trim();
                string value = current.Substring(separator + 1).Trim();
                config.ApplyParameter(key, value);
            }

            return config;
        }

        // Применяет одно правило из файла конфигурации.
        private void ApplyParameter(string key, string value)
        {
            switch (key)
            {
                case "MinEnergyTransmitted":
                    if (double.TryParse(value, out double minValue))
                    {
                        MinEnergyTransmitted = minValue;
                    }
                    break;
                case "MaxLossesShare":
                    if (double.TryParse(value, out double maxShare))
                    {
                        MaxLossesShare = maxShare;
                    }
                    break;
                case "OutputDirectory":
                    if (value.Length > 0)
                    {
                        OutputDirectory = value;
                    }
                    break;
            }
        }
    }
}
