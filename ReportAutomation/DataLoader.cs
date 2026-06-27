using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace ReportAutomation
{
    // Класс получения исходных данных.
    // Читает данные из файлов электронных таблиц учётных систем:
    // справочник подразделений и значения производственных показателей.
    public class DataLoader
    {
        private readonly Logger logger;

        public DataLoader(Logger logger)
        {
            this.logger = logger;
        }

        // Читает справочник подразделений из файла электронной таблицы.
        // Ожидаемые столбцы: код подразделения, наименование.
        public List<Department> LoadDepartments(string filePath)
        {
            List<Department> departments = new List<Department>();

            if (!File.Exists(filePath))
            {
                logger.WriteError("Файл справочника подразделений не найден");
                return departments;
            }

            using (XLWorkbook workbook = new XLWorkbook(filePath))
            {
                IXLWorksheet sheet = workbook.Worksheet(1);

                // Первая строка считается заголовком и пропускается.
                bool isHeader = true;
                foreach (IXLRow row in sheet.RowsUsed())
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    string code = row.Cell(1).GetString().Trim();
                    string name = row.Cell(2).GetString().Trim();

                    if (code.Length == 0)
                    {
                        continue;
                    }

                    departments.Add(new Department { Code = code, Name = name });
                }
            }

            logger.WriteInfo("Загружен справочник подразделений: записей " +
                departments.Count);
            return departments;
        }

        // Читает значения производственных показателей из файла.
        // Ожидаемые столбцы: код подразделения, передача,
        // полезный отпуск, потери.
        public List<Indicator> LoadIndicators(string filePath)
        {
            List<Indicator> indicators = new List<Indicator>();

            if (!File.Exists(filePath))
            {
                logger.WriteError("Файл с показателями не найден");
                return indicators;
            }

            using (XLWorkbook workbook = new XLWorkbook(filePath))
            {
                IXLWorksheet sheet = workbook.Worksheet(1);

                bool isHeader = true;
                foreach (IXLRow row in sheet.RowsUsed())
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    string code = row.Cell(1).GetString().Trim();
                    if (code.Length == 0)
                    {
                        continue;
                    }

                    Indicator indicator = new Indicator
                    {
                        DepartmentCode = code,
                        EnergyTransmitted = ReadNumber(row.Cell(2)),
                        EnergyUseful = ReadNumber(row.Cell(3)),
                        EnergyLosses = ReadNumber(row.Cell(4))
                    };

                    indicators.Add(indicator);
                }
            }

            logger.WriteInfo("Загружены значения показателей: записей " +
                indicators.Count);
            return indicators;
        }

        // Безопасно читает числовое значение из ячейки.
        // При некорректном значении возвращает ноль.
        private double ReadNumber(IXLCell cell)
        {
            if (cell.TryGetValue(out double number))
            {
                return number;
            }
            return 0;
        }
    }
}
