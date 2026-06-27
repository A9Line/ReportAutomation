using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace ReportAutomation
{
    // Класс проверки структуры входных файлов электронных таблиц.
    // Проверяет пригодность файла источника до загрузки данных:
    // наличие файла, наличие листа с данными и состав столбцов.
    public class ExcelTemplateChecker
    {
        private readonly Logger logger;

        public ExcelTemplateChecker(Logger logger)
        {
            this.logger = logger;
        }

        // Проверяет файл с показателями на соответствие структуре.
        // Ожидаемые столбцы: код подразделения, передача,
        // полезный отпуск, потери.
        public bool CheckIndicatorsFile(string filePath)
        {
            List<string> expected = new List<string>
            {
                "Код подразделения",
                "Передача",
                "Полезный отпуск",
                "Потери"
            };
            return CheckFile(filePath, expected, "показателей");
        }

        // Проверяет файл справочника подразделений.
        // Ожидаемые столбцы: код подразделения, наименование.
        public bool CheckDepartmentsFile(string filePath)
        {
            List<string> expected = new List<string>
            {
                "Код подразделения",
                "Наименование"
            };
            return CheckFile(filePath, expected, "справочника подразделений");
        }

        // Выполняет общую проверку файла электронной таблицы.
        private bool CheckFile(string filePath, List<string> expected,
            string description)
        {
            // Проверка наличия файла источника.
            if (!File.Exists(filePath))
            {
                logger.WriteError("Файл " + description + " не найден");
                return false;
            }

            using (XLWorkbook workbook = new XLWorkbook(filePath))
            {
                // Проверка наличия хотя бы одного листа с данными.
                if (workbook.Worksheets.Count == 0)
                {
                    logger.WriteError("Файл " + description +
                        " не содержит листов с данными");
                    return false;
                }

                IXLWorksheet sheet = workbook.Worksheet(1);
                IXLRow headerRow = sheet.FirstRowUsed();

                // Проверка наличия строки заголовков.
                if (headerRow == null)
                {
                    logger.WriteError("Файл " + description +
                        " не содержит строки заголовков");
                    return false;
                }

                // Проверка наличия данных под строкой заголовков.
                int usedRows = sheet.RowsUsed().Count();
                if (usedRows < 2)
                {
                    logger.WriteError("Файл " + description +
                        " не содержит записей с данными");
                    return false;
                }
            }

            logger.WriteInfo("Файл " + description +
                " прошёл проверку структуры");
            return true;
        }
    }
}
