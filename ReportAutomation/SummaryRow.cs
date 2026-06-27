namespace ReportAutomation
{
    // Строка итогового сводного отчёта по подразделению.
    // Формируется на этапе расчёта сводных показателей.
    public class SummaryRow
    {
        // Наименование подразделения.
        public string DepartmentName { get; set; } = string.Empty;

        // Объём передачи электрической энергии, тыс. кВт*ч.
        public double EnergyTransmitted { get; set; }

        // Объём полезного отпуска электрической энергии, тыс. кВт*ч.
        public double EnergyUseful { get; set; }

        // Потери электрической энергии в сетях, тыс. кВт*ч.
        public double EnergyLosses { get; set; }

        // Доля потерь от объёма передачи, процент.
        public double LossesShare { get; set; }
    }
}
