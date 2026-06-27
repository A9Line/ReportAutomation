namespace ReportAutomation
{
    // Производственный показатель по одному подразделению
    // за отчётный период. Значения поступают из учётной системы.
    public class Indicator
    {
        // Код подразделения, к которому относится показатель.
        public string DepartmentCode { get; set; } = string.Empty;

        // Наименование подразделения (заполняется при сведении данных).
        public string DepartmentName { get; set; } = string.Empty;

        // Объём передачи электрической энергии, тыс. кВт*ч.
        public double EnergyTransmitted { get; set; }

        // Объём полезного отпуска электрической энергии, тыс. кВт*ч.
        public double EnergyUseful { get; set; }

        // Потери электрической энергии в сетях, тыс. кВт*ч.
        public double EnergyLosses { get; set; }

        // Признак того, что запись прошла проверку корректности.
        public bool IsValid { get; set; } = true;
    }
}
