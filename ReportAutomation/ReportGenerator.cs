using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReportAutomation
{
    // Класс формирования отчётных документов.
    // Формирует сводный отчёт в формате электронной таблицы
    // и пояснительный текстовый документ.
    public class ReportGenerator
    {
        private readonly Logger logger;
        private readonly AppConfig config;

        public ReportGenerator(Logger logger, AppConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        // Формирует сводный отчётный документ в формате таблицы.
        // Возвращает путь к сохранённому файлу.
        public string BuildExcel(List<SummaryRow> rows,
            DateTime periodStart, DateTime periodEnd)
        {
            EnsureOutputDirectory();

            string fileName = "Сводный_отчёт_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            string path = Path.Combine(config.OutputDirectory, fileName);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet sheet = workbook.Worksheets.Add("Сводный отчёт");

                // Заголовок документа с указанием отчётного периода.
                sheet.Cell(1, 1).Value = "Сводный отчёт за период с " +
                    periodStart.ToString("dd.MM.yyyy") + " по " +
                    periodEnd.ToString("dd.MM.yyyy");
                sheet.Range(1, 1, 1, 5).Merge();

                // Строка заголовков столбцов.
                int headerRow = 3;
                sheet.Cell(headerRow, 1).Value = "Подразделение";
                sheet.Cell(headerRow, 2).Value = "Передача, тыс. кВт*ч";
                sheet.Cell(headerRow, 3).Value = "Полезный отпуск, тыс. кВт*ч";
                sheet.Cell(headerRow, 4).Value = "Потери, тыс. кВт*ч";
                sheet.Cell(headerRow, 5).Value = "Доля потерь, %";

                // Заполнение строк сводными данными по подразделениям.
                int currentRow = headerRow + 1;
                double totalTransmitted = 0;
                double totalUseful = 0;
                double totalLosses = 0;

                foreach (SummaryRow row in rows)
                {
                    sheet.Cell(currentRow, 1).Value = row.DepartmentName;
                    sheet.Cell(currentRow, 2).Value = row.EnergyTransmitted;
                    sheet.Cell(currentRow, 3).Value = row.EnergyUseful;
                    sheet.Cell(currentRow, 4).Value = row.EnergyLosses;
                    sheet.Cell(currentRow, 5).Value =
                        Math.Round(row.LossesShare, 2);

                    totalTransmitted += row.EnergyTransmitted;
                    totalUseful += row.EnergyUseful;
                    totalLosses += row.EnergyLosses;
                    currentRow++;
                }

                // Итоговая строка по предприятию в целом.
                sheet.Cell(currentRow, 1).Value = "Итого по предприятию";
                sheet.Cell(currentRow, 2).Value = totalTransmitted;
                sheet.Cell(currentRow, 3).Value = totalUseful;
                sheet.Cell(currentRow, 4).Value = totalLosses;
                if (totalTransmitted > 0)
                {
                    sheet.Cell(currentRow, 5).Value =
                        Math.Round(totalLosses / totalTransmitted * 100, 2);
                }

                sheet.Columns().AdjustToContents();
                workbook.SaveAs(path);
            }

            logger.WriteInfo("Сформирован сводный отчётный документ");
            return path;
        }

        // Формирует пояснительный текстовый документ в формате Word.
        // Возвращает путь к сохранённому файлу.
        public string BuildWord(List<SummaryRow> rows,
            DateTime periodStart, DateTime periodEnd)
        {
            EnsureOutputDirectory();

            string fileName = "Пояснительный_отчёт_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".docx";
            string path = Path.Combine(config.OutputDirectory, fileName);

            using (WordprocessingDocument document =
                WordprocessingDocument.Create(path,
                    WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Заголовок документа.
                AddParagraph(body, "Пояснительная часть отчёта");

                // Сведения об отчётном периоде.
                AddParagraph(body, "Отчётный период: с " +
                    periodStart.ToString("dd.MM.yyyy") + " по " +
                    periodEnd.ToString("dd.MM.yyyy"));

                // Краткое описание результатов по подразделениям.
                int count = rows.Count;
                AddParagraph(body, "Сводные показатели сформированы по " +
                    count + " подразделениям предприятия.");

                foreach (SummaryRow row in rows)
                {
                    string line = row.DepartmentName +
                        ": передача " + row.EnergyTransmitted +
                        " тыс. кВт*ч, потери " +
                        Math.Round(row.LossesShare, 2) + " процента.";
                    AddParagraph(body, line);
                }

                mainPart.Document.Save();
            }

            logger.WriteInfo("Сформирован пояснительный текстовый документ");
            return path;
        }

        // Добавляет в текстовый документ один абзац с заданным текстом.
        private void AddParagraph(Body body, string text)
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Text(text));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }

        // Проверяет наличие каталога сохранения и создаёт его
        // при отсутствии.
        private void EnsureOutputDirectory()
        {
            if (!Directory.Exists(config.OutputDirectory))
            {
                Directory.CreateDirectory(config.OutputDirectory);
            }
        }
    }
}
