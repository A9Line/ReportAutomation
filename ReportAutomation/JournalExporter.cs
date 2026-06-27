using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace ReportAutomation
{
    // Класс экспорта журнала операций в табличный вид.
    // Сохраняет записи журнала в файл электронной таблицы
    // для последующего анализа результатов при сопровождении.
    public class JournalExporter
    {
        private readonly string outputDirectory;

        public JournalExporter(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }

        // Экспортирует записи журнала в файл электронной таблицы.
        // Каждая запись разбирается на отметку времени, уровень
        // и текст сообщения.
        public string Export(string journalText)
        {
            EnsureDirectory();

            string fileName = "Журнал_операций_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            string path = Path.Combine(outputDirectory, fileName);

            List<string> lines = SplitLines(journalText);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet sheet =
                    workbook.Worksheets.Add("Журнал операций");

                sheet.Cell(1, 1).Value = "Дата и время";
                sheet.Cell(1, 2).Value = "Уровень";
                sheet.Cell(1, 3).Value = "Сообщение";

                int currentRow = 2;
                foreach (string line in lines)
                {
                    WriteRecord(sheet, currentRow, line);
                    currentRow++;
                }

                sheet.Columns().AdjustToContents();
                workbook.SaveAs(path);
            }

            return path;
        }

        // Разбивает текст журнала на отдельные непустые строки.
        private List<string> SplitLines(string journalText)
        {
            List<string> result = new List<string>();
            string[] rawLines =
                journalText.Split('\n');

            foreach (string raw in rawLines)
            {
                string line = raw.Trim();
                if (line.Length > 0)
                {
                    result.Add(line);
                }
            }
            return result;
        }

        // Записывает одну строку журнала в таблицу,
        // разбирая её на отметку времени, уровень и сообщение.
        private void WriteRecord(IXLWorksheet sheet, int row, string line)
        {
            // Формат записи: "дата время  уровень  сообщение".
            string[] parts = line.Split(
                new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 3)
            {
                sheet.Cell(row, 1).Value = parts[0];
                sheet.Cell(row, 2).Value = parts[1];
                sheet.Cell(row, 3).Value = parts[2];
            }
            else
            {
                sheet.Cell(row, 3).Value = line;
            }
        }

        // Проверяет наличие каталога и создаёт его при отсутствии.
        private void EnsureDirectory()
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
        }
    }
}
