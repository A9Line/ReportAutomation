using System;

namespace ReportAutomation
{
    // Параметры запуска формирования отчёта.
    // Заполняются в главном окне и передаются управляющему классу.
    public class ReportRequest
    {
        // Дата начала отчётного периода.
        public DateTime PeriodStart { get; set; }

        // Дата окончания отчётного периода.
        public DateTime PeriodEnd { get; set; }

        // Путь к файлу с производственными показателями.
        public string IndicatorsFilePath { get; set; } = string.Empty;

        // Путь к файлу справочника подразделений.
        public string DepartmentsFilePath { get; set; } = string.Empty;
    }
}
