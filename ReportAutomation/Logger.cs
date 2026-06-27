using System;
using System.IO;
using System.Text;

namespace ReportAutomation
{
    // Класс ведения журнала операций.
    // Накапливает записи о ходе работы и ошибках, выводит их
    // оператору и сохраняет в текстовый файл журнала.
    public class Logger
    {
        // Накопитель записей журнала для вывода в главное окно.
        private readonly StringBuilder records = new StringBuilder();

        // Каталог, в котором сохраняется файл журнала.
        private readonly string outputDirectory;

        public Logger(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }

        // Записывает информационное сообщение о ходе работы.
        public void WriteInfo(string message)
        {
            AddRecord("ИНФО", message);
        }

        // Записывает сообщение об ошибке.
        public void WriteError(string message)
        {
            AddRecord("ОШИБКА", message);
        }

        // Формирует одну запись журнала с отметкой времени.
        private void AddRecord(string level, string message)
        {
            string timeMark = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            records.AppendLine(timeMark + "  " + level + "  " + message);
        }

        // Возвращает накопленный текст журнала для вывода оператору.
        public string GetText()
        {
            return records.ToString();
        }

        // Сохраняет журнал операций в текстовый файл.
        public void SaveToFile()
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string fileName = "Журнал_операций_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string path = Path.Combine(outputDirectory, fileName);
            File.WriteAllText(path, records.ToString(), Encoding.UTF8);
        }
    }
}
