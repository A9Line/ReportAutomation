using System.Collections.Generic;

namespace ReportAutomation
{
    // Класс обработки данных.
    // Выполняет проверку полноты и корректности исходных данных,
    // сведение показателей со справочником подразделений
    // и расчёт сводных показателей отчёта.
    public class DataProcessor
    {
        private readonly Logger logger;
        private readonly AppConfig config;

        public DataProcessor(Logger logger, AppConfig config)
        {
            this.logger = logger;
            this.config = config;
        }

        // Проверяет полноту и корректность набора показателей
        // с помощью набора правил проверки.
        // Записи с нарушениями помечаются как недействительные.
        public List<Indicator> Validate(List<Indicator> indicators)
        {
            RuleSet ruleSet = new RuleSet();
            int invalidCount = 0;

            foreach (Indicator indicator in indicators)
            {
                ValidationResult result = ruleSet.CheckAll(indicator, config);
                indicator.IsValid = result.IsValid;

                if (!result.IsValid)
                {
                    invalidCount++;
                    logger.WriteError("Запись по подразделению " +
                        indicator.DepartmentCode + ": " + result.Message);
                }
            }

            logger.WriteInfo("Проверка данных завершена. Некорректных записей " +
                invalidCount);
            return indicators;
        }

        // Сводит показатели со справочником подразделений
        // по ключевому полю — коду подразделения.
        // В обработку поступают только корректные записи.
        public List<Indicator> Merge(List<Indicator> indicators,
            List<Department> departments)
        {
            // Справочник подразделений приводится к виду "код — наименование"
            // для быстрого поиска при сведении данных.
            Dictionary<string, string> directory =
                new Dictionary<string, string>();
            foreach (Department department in departments)
            {
                if (!directory.ContainsKey(department.Code))
                {
                    directory.Add(department.Code, department.Name);
                }
            }

            List<Indicator> merged = new List<Indicator>();
            foreach (Indicator indicator in indicators)
            {
                if (!indicator.IsValid)
                {
                    continue;
                }

                if (directory.ContainsKey(indicator.DepartmentCode))
                {
                    indicator.DepartmentName =
                        directory[indicator.DepartmentCode];
                }
                else
                {
                    // Подразделение отсутствует в справочнике,
                    // запись передаётся на контроль оператору.
                    indicator.DepartmentName = "Подразделение не определено";
                    logger.WriteError("Код подразделения " +
                        indicator.DepartmentCode +
                        " отсутствует в справочнике");
                }

                merged.Add(indicator);
            }

            logger.WriteInfo("Сведение данных завершено. Записей в обработке " +
                merged.Count);
            return merged;
        }

        // Рассчитывает сводные показатели по каждому подразделению
        // и формирует строки итогового отчёта.
        public List<SummaryRow> Calculate(List<Indicator> indicators)
        {
            List<SummaryRow> rows = new List<SummaryRow>();

            foreach (Indicator indicator in indicators)
            {
                SummaryRow row = new SummaryRow
                {
                    DepartmentName = indicator.DepartmentName,
                    EnergyTransmitted = indicator.EnergyTransmitted,
                    EnergyUseful = indicator.EnergyUseful,
                    EnergyLosses = indicator.EnergyLosses
                };

                // Доля потерь от объёма передачи, процент.
                if (indicator.EnergyTransmitted > 0)
                {
                    row.LossesShare = indicator.EnergyLosses /
                        indicator.EnergyTransmitted * 100;
                }
                else
                {
                    row.LossesShare = 0;
                }

                // Превышение установленной доли потерь фиксируется
                // в журнале для последующего контроля оператором.
                if (row.LossesShare > config.MaxLossesShare)
                {
                    logger.WriteInfo("Подразделение " + row.DepartmentName +
                        ": доля потерь превышает установленное значение");
                }

                rows.Add(row);
            }

            logger.WriteInfo("Расчёт сводных показателей завершён. Строк " +
                rows.Count);
            return rows;
        }
    }
}
