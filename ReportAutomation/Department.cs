namespace ReportAutomation
{
    // Запись справочника подразделений предприятия.
    // Соответствует филиалам и районам электрических сетей.
    public class Department
    {
        // Код подразделения (ключевое поле для сведения данных).
        public string Code { get; set; } = string.Empty;

        // Полное наименование подразделения.
        public string Name { get; set; } = string.Empty;
    }
}
