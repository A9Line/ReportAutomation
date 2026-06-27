using System;

namespace ReportAutomation
{
    // Управляющий класс процесса формирования отчёта.
    // Координирует работу классов получения данных, их обработки
    // и формирования документов, а также ведение журнала операций.
    public class ReportController
    {
        // Запускает полный цикл формирования отчётной документации
        // по параметрам, заданным оператором.
        // Возвращает результат с признаком успеха и журналом операций.
        public ReportResult Run(ReportRequest request)
        {
            // Загрузка правил проверки и параметров из конфигурации.
            AppConfig config = AppConfig.Load();
            Logger logger = new Logger(config.OutputDirectory);
            ReportResult result = new ReportResult();

            logger.WriteInfo("Запуск формирования отчёта за период с " +
                request.PeriodStart.ToString("dd.MM.yyyy") + " по " +
                request.PeriodEnd.ToString("dd.MM.yyyy"));

            try
            {
                // Этап получения исходных данных из источников.
                DataLoader loader = new DataLoader(logger);
                var departments =
                    loader.LoadDepartments(request.DepartmentsFilePath);
                var indicators =
                    loader.LoadIndicators(request.IndicatorsFilePath);

                if (indicators.Count == 0)
                {
                    logger.WriteError("Исходные данные не получены");
                    return Fail(result, logger,
                        "Источник данных недоступен или не содержит данных");
                }

                // Этап проверки, сведения и расчёта показателей.
                DataProcessor processor = new DataProcessor(logger, config);
                indicators = processor.Validate(indicators);
                var merged = processor.Merge(indicators, departments);

                if (merged.Count == 0)
                {
                    logger.WriteError(
                        "После проверки данных не осталось корректных записей");
                    return Fail(result, logger,
                        "Обнаружены некорректные данные");
                }

                var summary = processor.Calculate(merged);

                // Этап формирования итоговых отчётных документов.
                ReportGenerator generator =
                    new ReportGenerator(logger, config);
                string excelPath = generator.BuildExcel(summary,
                    request.PeriodStart, request.PeriodEnd);
                generator.BuildWord(summary,
                    request.PeriodStart, request.PeriodEnd);

                // Завершение процесса и сохранение журнала операций.
                logger.WriteInfo("Формирование отчёта завершено");
                logger.SaveToFile();

                result.Success = true;
                result.OutputFilePath = excelPath;
                result.Log = logger.GetText();
                return result;
            }
            catch (Exception exception)
            {
                // Обработка исключительных ситуаций без аварийного
                // завершения работы программного средства.
                logger.WriteError("Ошибка формирования отчёта: " +
                    exception.Message);
                return Fail(result, logger, "Ошибка формирования отчёта");
            }
        }

        // Формирует результат с признаком неуспешного завершения,
        // сохраняет журнал и возвращает сообщение об ошибке оператору.
        private ReportResult Fail(ReportResult result, Logger logger,
            string message)
        {
            logger.SaveToFile();
            result.Success = false;
            result.ErrorMessage = message;
            result.Log = logger.GetText();
            return result;
        }
    }
}
